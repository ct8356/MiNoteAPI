using NUnit.Framework;
using RabbitCore;

namespace IntegrationTests
{
    [TestFixture]
    public class RabbitConsumerPublisherTests
    {

        MessageConsumer Consumer;
        MessagePublisher Publisher;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var hostName = "localhost";
            var exchangeName = "";
            var queueName = "hello";
            Consumer = new MessageConsumer(hostName, queueName);
            Publisher = new MessagePublisher(hostName, exchangeName, queueName);
        }

        [Test]
        public void PublishMessage_ConsumeMessage_PassesMessage()
        {
            var messageSent = "Hello world!";

            Publisher.PublishMessage(messageSent);
            var messageReceived = Consumer.ConsumeMessage();

            Assert.AreEqual(messageSent, messageReceived);
        }

    }
}
