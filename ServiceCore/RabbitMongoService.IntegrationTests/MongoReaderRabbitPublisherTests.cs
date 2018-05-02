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
        MongoBroker _broker;
        MongoCreator Creator { get; set; } 
        //IF can get away with just a creator,
        //may as well
        MongoReader Reader { get; set; }
        AutoConsumerReader _autoConsumerReader;
        RabbitConsumer _consumer;

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

            var autoConsumer = new RabbitAutoConsumer(hostName, queueName);
            var objectReader = new MongoReader(databaseName, collectionName);
            var messagePublisher = new RabbitPublisher(hostName, exchangeName, queueName);
            _autoConsumerReader = new AutoConsumerReader(autoConsumer, objectReader, messagePublisher);

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

            var message = _consumer.ConsumeMessage();
            var jObjects = JsonConvert.DeserializeObject<List<JObject>>(message);
            var actualJObject = jObjects[0];
            Assert.AreEqual("new object", (string)actualJObject["Content"]);
        }

    }
}
