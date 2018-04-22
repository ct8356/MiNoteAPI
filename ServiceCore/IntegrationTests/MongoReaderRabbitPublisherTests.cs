using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using ServiceCore;
using System.Linq;

namespace IntegrationTests
{
    [TestFixture]
    public class MongoReaderRabbitPublisherTests
    {
        protected MongoDbObjectRepository _repo;
        protected MongoReaderRabbitPublisher _readerPublisher;
        protected RabbitConsumer _consumer;

        [SetUp]
        public void SetUp()
        {
            var hostName = "localHost";  
            var exchangeName = "";   
            _repo = new MongoDbObjectRepository();
            _repo.Initialize("test");
            var objectReader = new MongoReader()
            {
                Documents = _repo.Documents,
            };
            var messagePublisher = new RabbitPublisher()
            {
                HostName = hostName,
                ExchangeName = exchangeName,
            };
            _readerPublisher = new MongoReaderRabbitPublisher(objectReader, messagePublisher);
            _consumer = new RabbitConsumer()
            {
                HostName = hostName,
                QueueName = "NoteAPI",
            };
        }

        [Test]
        public void ReadObjectsAndPublishMessage_Works()
        {
            dynamic @object = new
            {
                _id = 1,
                Content = "new object"
            };
            JObject jObject = JObject.FromObject(@object);
            _repo.CreateObject(jObject);

            _readerPublisher.ReadObjectsAndPublishMessage();

            var message = _consumer.ConsumeMessage();
            Assert.AreEqual("new object", message);
        }

    }
}
