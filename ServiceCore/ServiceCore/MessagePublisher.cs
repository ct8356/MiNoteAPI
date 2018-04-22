using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCore
{
    public class MessagePublisher : IMessagePublisher
    {
        string _hostName;
        string _exchangeName;
        
        public MessagePublisher(string hostName, string exchangeName)
        {
            _hostName = hostName;
            _exchangeName = exchangeName;           
        }

        public void PublishMessage(string routingKey, string message)
        {
            var factory = new ConnectionFactory() { HostName = _hostName };
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
                    exchange: _exchangeName,
                    routingKey: routingKey,
                    basicProperties: null,
                    body: body);
            }
        }

    }
}
