using NUnit.Framework;
using RabbitCore;

namespace IntegrationTests
{
    [TestFixture]
    public class RabbitConsumerPublisherTests
    {

        protected RabbitConsumer _consumer;
        protected RabbitPublisher _publisher;
        protected string _queueName;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var hostName = "localhost";
            _queueName = "hello";
            _consumer = new RabbitConsumer()
            {
                HostName = hostName,
                QueueName = _queueName,
            };
            _publisher = new RabbitPublisher()
            {
                HostName = hostName,
                ExchangeName = "",
            };
        }

        [Test]
        public void PublishMessage_ConsumeMessage_PassesMessage()
        {
            var messageSent = "Hello world!";

            _publisher.PublishMessage(_queueName, messageSent);
            var messageReceived = _consumer.ConsumeMessage();

            Assert.AreEqual(messageSent, messageReceived);
        }

    }
}
