using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
//using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace MessagingTest
{
    public class CentralHub : ICentalHub
    {
        private readonly IStorageProvider _storageProvider;
        private readonly ILogger<CentralHub> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ConcurrentDictionary<string, IDependentNode> _nodes = new();
        private readonly Channel<Event> _eventChannel = Channel.CreateUnbounded<Event>();

        public CentralHub(IStorageProvider storageProvider, ILogger<CentralHub> logger, ILoggerFactory loggerFactory)
        {
            _storageProvider = storageProvider;
            _logger = logger;
            _loggerFactory = loggerFactory;
        }

        public void RegisterNode(string name, IDependentNode node)
        {
            if (_nodes.TryAdd(name, node))
            {
                _logger.LogInformation($"Node {name} registered successfully.");
                // Add this node as a peer to all existing nodes
                foreach (var existingNode in _nodes.Values.Where(n => n != node))
                {
                    existingNode.AddPeer(node);
                    node.AddPeer(existingNode);
                }
            }
            else
            {
                _logger.LogWarning($"Node {name} already exists.");
            }
        }

        public async Task InitializeNodesAsync(int count)
        {
            for (int i = 1; i <= count; i++)
            {
                var nodeName = $"Node{i}";
                var nodeLogger = _loggerFactory.CreateLogger<DependentNode>();
                var node = new DependentNode(nodeName, _storageProvider, nodeLogger);
                RegisterNode(nodeName, node);
            }
            _logger.LogInformation($"Initialized {count} nodes.");
        }

        public async Task ProcessEventsAsync(CancellationToken cancellationToken)
        {
            await foreach (var @event in _eventChannel.Reader.ReadAllAsync(cancellationToken))
            {
                _logger.LogInformation($"Processing event from {@event.Source}: {@event.Type}");
                // Process the event
                await ProcessEvent(@event);
            }
        }

        public async Task ProcessNodeMessagesAsync(string nodeName, CancellationToken cancellationToken)
        {
            if (_nodes.TryGetValue(nodeName, out var node))
            {
                await node.ProcessMessagesAsync(cancellationToken);
            }
            else
            {
                _logger.LogWarning($"Node {nodeName} not found for message processing.");
            }
        }

        private Task ProcessEvent(Event @event)
        {
            // Implement event processing logic here
            _logger.LogInformation($"Event processed: {@event.Type} from {@event.Source}");
            return Task.CompletedTask;
        }

        public async Task SendCommandToNodeAsync(string nodeName, string command)
        {
            if (_nodes.TryGetValue(nodeName, out var node))
            {
                await node.SendMessageAsync("CentralHub", $"Command: {command}");
                _logger.LogInformation($"Sent command to {nodeName}: {command}");
            }
            else
            {
                _logger.LogWarning($"Node {nodeName} not found.");
            }
        }

    }
}
