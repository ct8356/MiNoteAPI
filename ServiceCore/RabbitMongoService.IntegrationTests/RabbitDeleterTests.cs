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
        protected EntryCreator _creator;
        protected EntryReader _reader;
        protected RabbitDeleter _deleter;

        [SetUp]
        public void SetUp()
        {
            var hostName = "localHost";
            var queueName = "CreateNote";
            var exchangeName = "";
            var messageConsumer = new MessageConsumer(hostName, queueName);
            var messagePublisher = new MessagePublisher(hostName, exchangeName, queueName);
            _creator = new EntryCreator(messagePublisher);
            _reader = new EntryReader(messageConsumer, messagePublisher);
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
