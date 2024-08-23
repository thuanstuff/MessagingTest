
namespace MessagingTest
{
    public interface ICentalHub
    {
        void RegisterNode(string name, IDependentNode node);
        Task InitializeNodesAsync(int count);
        Task ProcessEventsAsync(CancellationToken cancellationToken);
        Task ProcessNodeMessagesAsync(string nodeName, CancellationToken cancellationToken);
        Task SendCommandToNodeAsync(string nodeName, string command);
        //Task<IAsyncEnumerable<string>> StreamDataFromAllNodesAsync(CancellationToken cancellationToken);
        //Task RaiseEventAsync(Event @event);
    }
}
