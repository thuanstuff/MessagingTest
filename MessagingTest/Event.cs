
namespace MessagingTest
{
    public class Event
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Source { get; set; }
        public string Type { get; set; }
        public object Data { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
