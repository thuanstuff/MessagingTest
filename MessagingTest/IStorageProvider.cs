
namespace MessagingTest
{
    public interface IStorageProvider
    {
        Task SaveMessageAsync(Message message);
        Task SaveEventAsync(Event @event);
        Task<IEnumerable<Message>> GetMessagesAsync(string recipient);
        Task<IEnumerable<Event>> GetEventsAsync(string source);
    }
}
