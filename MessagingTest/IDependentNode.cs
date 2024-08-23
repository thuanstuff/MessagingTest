
namespace MessagingTest
{
    public interface IDependentNode
    {
        string Name { get; }
        void AddPeer(IDependentNode peer);
        Task SendMessageAsync(string recipient, string content);
        Task ReceiveMessageAsync(Message message);
        Task ProcessMessagesAsync(CancellationToken cancellationToken);
        //Task RaiseEventAsync(string type, object data);
        //IAsyncEnumerable<string> StreamDataAsync(CancellationToken cancellationToken);
    }
}
