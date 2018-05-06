namespace ServiceInterfaces
{
    public interface IMessagePublisher
    {
        string HostName { get; set; }
        string ExchangeName { get; set; }
        string DefaultRoutingKey { get; set; }
        void PublishMessage(string message);
        void PublishMessage(string routingKey, string message);
    }
}
