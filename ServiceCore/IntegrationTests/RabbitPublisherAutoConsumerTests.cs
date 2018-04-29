using NUnit.Framework;
using RabbitCore;
using ServiceInterfaces;
using System.Threading;

namespace IntegrationTests
{
    [TestFixture]
    public class RabbitPublisherAutoConsumerTests
    {

        protected RabbitAutoConsumer _consumer;
        protected RabbitPublisher _publisher;
        protected string _queueName;
        protected AutoResetEvent _autoResetEvent;
        protected string _message;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var hostName = "localhost";
            var exchangeName = "";
            _queueName = "hello";
            _consumer = new RabbitAutoConsumer(hostName, _queueName);
            _publisher = new RabbitPublisher(hostName, exchangeName, _queueName);
            _autoResetEvent = new AutoResetEvent(false);
        }

        [SetUp]
        public void SetUp()
        {
            _message = null;
        }

        [Test]
        public void PublishMessage_ConsumeMessage_PassesMessage()
        {
            var messageSent = "Hello world!";
            _consumer.MessageReceived += _consumer_MessageReceived;
            _consumer.Start();
            //Perhaps try waiting for 10 secs here
            //to make sure it does not crash.

            _publisher.PublishMessage(_queueName, messageSent);

            _autoResetEvent.WaitOne(); //Wait until message received.
            Assert.AreEqual(messageSent, _message);
            _consumer.Stop();
            //This stops it from running continuously. Nice.
            //I guess because once the chanel is killed,
            //It has no context that can wait for a Message to come back,
            //so it stops waiting.
        }

        private void _consumer_MessageReceived(object sender, IMessageEventArgs e)
        {
            _message = e.Message;
            _autoResetEvent.Set();
        }

    }
}
