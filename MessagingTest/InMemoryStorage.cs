using System.Collections.Concurrent;

namespace MessagingTest
{
    public class InMemoryStorage : IStorageProvider
    {
        private readonly ConcurrentBag<Message> _messages = new();
        private readonly ConcurrentBag<Event> _events = new();

        public Task SaveMessageAsync(Message message)
        {
            _messages.Add(message);
            return Task.CompletedTask;
        }

        public Task SaveEventAsync(Event @event)
        {
            _events.Add(@event);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<Message>> GetMessagesAsync(string recipient)
        {
            return Task.FromResult(_messages.Where(m => m.Recipient == recipient));
        }

        public Task<IEnumerable<Event>> GetEventsAsync(string source)
        {
            return Task.FromResult(_events.Where(e => e.Source == source));
        }
    }
}
