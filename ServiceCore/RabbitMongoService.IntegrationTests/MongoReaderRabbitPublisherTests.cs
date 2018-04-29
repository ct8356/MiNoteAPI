using MongoClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RabbitCore;
using RabbitMongoService;
using System.Collections.Generic;

namespace IntegrationTests
{
    [TestFixture]
    public class MongoReaderRabbitPublisherTests
    {
        protected MongoBroker _broker;
        protected MongoReaderRabbitPublisher _readerPublisher;
        protected RabbitConsumer _consumer;

        [SetUp]
        public void SetUp()
        {
            var hostName = "localHost";
            var exchangeName = "";
            _broker = new MongoBroker();
            _broker.Initialize("test");
            _broker.DeleteEverything();

            var objectReader = new MongoReader()
            {
                Documents = _broker.Documents,
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
        public void ReadObjectsAndPublishMessage_PublishesMessage()
        {
            dynamic @object = new
            {
                _id = 1,
                Content = "new object"
            };
            JObject jObject = JObject.FromObject(@object);
            _broker.CreateObject(jObject);

            _readerPublisher.ReadObjectsAndPublishMessage();

            var message = _consumer.ConsumeMessage();
            var jObjects = JsonConvert.DeserializeObject<List<JObject>>(message);
            var actualJObject = jObjects[0];
            Assert.AreEqual("new object", (string)actualJObject["Content"]);
        }

    }
}
