using NUnit.Framework;
using RabbitCore;
using ServiceInterfaces;
using System.Threading;

namespace IntegrationTests
{
    [TestFixture]
    public class RabbitPublisherAutoConsumerTests
    {

        AutoMessageConsumer AutoConsumer;
        MessagePublisher _publisher;
        AutoResetEvent _autoResetEvent;
        string _message;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var hostName = "localhost";
            var exchangeName = "";
            var queueName = "hello";
            AutoConsumer = new AutoMessageConsumer(hostName, queueName);
            _publisher = new MessagePublisher(hostName, exchangeName, queueName);
            _autoResetEvent = new AutoResetEvent(false);
        }

        [SetUp]
        public void SetUp()
        {
            _message = null;
        }

        [Test]
        public void PublishMessage_AutoConsumeMessage_PassesMessage()
        {
            var messageSent = "Hello world!";
            AutoConsumer.MessageReceived += _consumer_MessageReceived;
            AutoConsumer.Start();
            //Perhaps try waiting for 10 secs here
            //to make sure it does not crash.

            _publisher.PublishMessage(messageSent);

            _autoResetEvent.WaitOne(); //Wait until message received.
            Assert.AreEqual(messageSent, _message);
            AutoConsumer.Stop();
            //This stops it from running continuously. Nice.
            //I guess because once the channel is killed,
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
