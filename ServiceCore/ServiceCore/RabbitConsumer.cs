using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ServiceInterfaces;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceCore
{
    public class RabbitConsumer : IMessageConsumer
    {
        public string HostName { get; set; }
        public string QueueName { get; set; }
        protected AutoResetEvent _autoResetEvent;

        public event MessageEventHandler MessageReceived;

        public RabbitConsumer()
        {
            //this cannot be inherited through interface!
            //BUT the IOC container must know about this project, so ok!
            //If a class or project needs more than one MessageConsumer,
            //Then should pass it a MessageConsumerFactory!
            //If project gets by with one, just need this class.
            //AND I guess, pass these strings in via IOC container.
            //THE FACTORY implementation, CAN know about this constructor, so ok.
            _autoResetEvent = new AutoResetEvent(false);
        }

        public void Start()
        {

        }

        public void Stop()
        {

        }

        public string ConsumeMessage()
        {
            //Apparently, this should not be async.
            //The async bit (with Task.Run{ Task.ReturnFrom() }) 
            //should be in the UI thread only.
            var factory = new ConnectionFactory() { HostName = HostName };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(
                    queue: QueueName,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,             
                    arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                string message = null;
                consumer.Received += (model, e) =>
                {
                    message = Encoding.UTF8.GetString(e.Body);
                    MessageReceived?.Invoke(this, new MessageEventArgs(message));
                    _autoResetEvent.Set();
                }; //the callback method, once message finally received.

                channel.BasicConsume(
                    queue: QueueName,
                    autoAck: true,
                    consumer: consumer);

                _autoResetEvent.WaitOne(); //Wait until message received.
                return message;
            }
        }

    }
}
