using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MessagingTest
{
    public class Application : IHostedService
    {
        private readonly ICentalHub _centralHub;
        private readonly ILogger<Application> _logger;

        public Application(ICentalHub centralHub, ILogger<Application> logger)
        {
            _centralHub = centralHub;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Application starting...");

            // Initialize nodes
            int nodeCount = 3;
            await _centralHub.InitializeNodesAsync(nodeCount);

            // Start processing events in the central hub
            var centralHubTask = _centralHub.ProcessEventsAsync(cancellationToken);

            // Start processing messages in each node
            var nodeTasks = new List<Task>();
            for (int i = 0; i < nodeCount; i++)
            {
                string nodeName = $"Node{i}";
                nodeTasks.Add(_centralHub.ProcessNodeMessagesAsync(nodeName, cancellationToken));
            }

            // Start user input handling
            var userInputTask = HandleUserInputAsync(cancellationToken);

            // Wait for cancellation or user input task to complete
            await Task.WhenAny(
                Task.Delay(Timeout.Infinite, cancellationToken),
                Task.WhenAll(centralHubTask, userInputTask)
                    .ContinueWith(_ => Task.WhenAll(nodeTasks))
            );

            _logger.LogInformation("Application stopping...");
        }

        private async Task HandleUserInputAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                Console.WriteLine("Enter command (format: 'NodeName Message' or 'exit' to quit):");
                string input = await Console.In.ReadLineAsync();

                if (input.ToLower() == "exit")
                {
                    break;
                }

                string[] parts = input.Split(new[] { ' ' }, 2);
                if (parts.Length == 2)
                {
                    string nodeName = parts[0];
                    string message = parts[1];
                    await _centralHub.SendCommandToNodeAsync(nodeName, message);
                    _logger.LogInformation($"Sent message to {nodeName}: {message}");
                }
                else
                {
                    Console.WriteLine("Invalid input format. Please use 'NodeName Message'.");
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Application stopped.");
            return Task.CompletedTask;
        }
    }
}
