
namespace ServiceInterfaces
{
    public interface IMessagePublisher
    {
        string HostName { get; set; }
        string ExchangeName { get; set; }
        void PublishMessage(string routingKey, string message);
    }
}
