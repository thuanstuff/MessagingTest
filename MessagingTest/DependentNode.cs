using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Threading.Channels;

namespace MessagingTest
{
    public class DependentNode : IDependentNode
    {
        private readonly IStorageProvider _storageProvider;
        private readonly ILogger<DependentNode> _logger;
        private readonly ConcurrentDictionary<string, IDependentNode> _peers = new();
        private readonly Channel<Message> _messageChannel = Channel.CreateUnbounded<Message>();

        public string Name { get; }

        public DependentNode(string name, IStorageProvider storageProvider, ILogger<DependentNode> logger)
        {
            Name = name;
            _storageProvider = storageProvider;
            _logger = logger;
        }

        public void AddPeer(IDependentNode peer)
        {
            _peers.TryAdd(peer.Name, peer);
        }

        public async Task SendMessageAsync(string recipient, string content)
        {
            var message = new Message
            {
                Sender = Name,
                Recipient = recipient,
                Content = content
            };

            await _storageProvider.SaveMessageAsync(message);

            if (_peers.TryGetValue(recipient, out var peer))
            {
                await peer.ReceiveMessageAsync(message);
            }
            else
            {
                _logger.LogWarning($"Recipient {recipient} not found among peers.");
            }
        }

        public async Task ReceiveMessageAsync(Message message)
        {
            await _messageChannel.Writer.WriteAsync(message);
        }

        public async Task ProcessMessagesAsync(CancellationToken cancellationToken)
        {
            await foreach (var message in _messageChannel.Reader.ReadAllAsync(cancellationToken))
            {
                _logger.LogInformation($"{Name} received message from {message.Sender}: {message.Content}");
                // Process the message
                await ProcessMessage(message);
            }
        }

        private Task ProcessMessage(Message message)
        {
            // Implement message processing logic here
            _logger.LogInformation($"{Name} processing message: {message.Content}");
            return Task.CompletedTask;
        }

    }
}
