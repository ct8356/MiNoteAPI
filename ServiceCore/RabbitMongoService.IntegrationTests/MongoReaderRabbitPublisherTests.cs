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
        MongoCreator Creator { get; set; }
        MongoReader Reader { get; set; }

        [OneTimeSetUp]
        public void SetUp()
        {
            var databaseName = "test";
            var collectionName = "Objects";
            var hostName = "localHost";
            var exchangeName = "";
            var queueName = "NoteAPI";           
            Reader = new MongoReader(databaseName, collectionName);
            Creator = new MongoCreator(Reader, databaseName, collectionName);
            _broker = new MongoBroker(Creator, Reader, null, null);
            _broker.Initialize(databaseName);
            _broker.DeleteEverything();

            var objectReader = new MongoReader(databaseName, collectionName);
            var messagePublisher = new RabbitPublisher(hostName, exchangeName, queueName);
            _readerPublisher = new MongoReaderRabbitPublisher(objectReader, messagePublisher);

            _consumer = new RabbitConsumer(hostName, queueName);
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
