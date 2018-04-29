using DatabaseInterfaces;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RabbitCore;
using System.Linq;

namespace IntegrationTests
{
    [TestFixture]
    public class RabbitDeleterTests
    {
        protected RabbitCreator _creator;
        protected RabbitReader _reader;
        protected RabbitDeleter _deleter;

        [SetUp]
        public void SetUp()
        {
            var hostName = "localHost";
            var queueName = "CreateNote";
            var exchangeName = "";
            var messageConsumer = new RabbitConsumer() {
                HostName = hostName,
                QueueName = queueName,
            };
            var messagePublisher = new RabbitPublisher()
            {
                HostName = hostName,
                ExchangeName = "",
            };
            _creator = new RabbitCreator(messagePublisher);
            _reader = new RabbitReader(messageConsumer, messagePublisher);
            _deleter = new RabbitDeleter(messagePublisher);
        }

        [Test]
        public void DeleteObject_WithId_DeletesObject()
        {
            _deleter.DeleteObject(1);

            var actualObjects = _reader.ReadObjects();
            Assert.AreEqual(1, actualObjects.Count());
        }

    }
}
