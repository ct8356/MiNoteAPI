using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ServiceInterfaces;
using System.Text;

namespace RabbitCore
{
    public class RabbitAutoConsumer : IAutoMessageConsumer
    {
        IConnectionFactory _factory;
        IConnection _connection;
        IModel _channel;

        public string HostName { get; set; }
        public string QueueName { get; set; }
        public event MessageEventHandler MessageReceived;

        public RabbitAutoConsumer(string hostName, string queueName)
        {
            HostName = hostName;
            QueueName = queueName;
            _factory = new ConnectionFactory() { HostName = HostName };
        }

        public void Start()
        {
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();
            ConsumeMessage(); 
            //Starts the loop.
        }

        public void Stop()
        {
            _channel.Dispose();
            _connection.Dispose();
        }

        private void ConsumeMessage()
        {
        //Apparently, this should not be async.
        //The async bit (with Task.Run{ Task.ReturnFrom() }) 
        //should be in the UI thread only.
            _channel.QueueDeclare(
                queue: QueueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
            string message = null;
            consumer.Received += (model, e) =>
            {
                message = Encoding.UTF8.GetString(e.Body);
                MessageReceived?.Invoke(this, new MessageEventArgs(message));
                //does stuff with the message.
                ConsumeMessage();
                //calls Consume again, to set consumer to active again.
                //So it is always waiting for a message.
            }; //the callback method, once message finally received.

            _channel.BasicConsume(
                queue: QueueName,
                autoAck: true,
                consumer: consumer);
        }

    }
}
