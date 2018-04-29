using RabbitMQ.Client;
using ServiceInterfaces;
using System.Text;

namespace RabbitCore
{
    public class RabbitPublisher : IMessagePublisher
    {
        public string HostName { get; set; }
        public string ExchangeName { get; set; }
        public string DefaultRoutingKey { get; set; }

        public RabbitPublisher(string hostName, string exchangeName, string defaultRoutingKey)
        {
            HostName = hostName;
            ExchangeName = exchangeName;
            DefaultRoutingKey = defaultRoutingKey;
        }

        public void PublishMessage(string message)
        {
            var factory = new ConnectionFactory() { HostName = HostName };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(
                    queue: DefaultRoutingKey,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(
                    exchange: ExchangeName,
                    routingKey: DefaultRoutingKey,
                    basicProperties: null,
                    body: body);
            }
        }

        public void PublishMessage(string routingKey, string message)
        {
            var factory = new ConnectionFactory() { HostName = HostName };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(
                    queue: routingKey,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(
                    exchange: ExchangeName,
                    routingKey: routingKey,
                    basicProperties: null,
                    body: body);
            }
        }

    }
}
