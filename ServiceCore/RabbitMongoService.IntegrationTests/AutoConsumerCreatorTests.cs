using MongoClient;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RabbitCore;
using RabbitMongoService;
using System;
using System.Linq;
using System.Threading;

namespace IntegrationTests
{
    [TestFixture]
    public class AutoConsumerCreatorTests
    {
        MongoBroker Broker;
        MongoCreator Creator;
        MongoReader Reader;
        MessagePublisher Publisher;
        AutoConsumerCreator ConsumerCreator;    
        AutoResetEvent AutoResetEvent;

        [OneTimeSetUp]
        public void SetUp()
        {
            var databaseName = "test";
            var collectionName = "Objects";
            var hostName = "localHost";
            var exchangeName = "";
            var queueName = "CreateNote";

            Reader = new MongoReader(databaseName, collectionName);
            Creator = new MongoCreator(Reader, databaseName, collectionName);
            Broker = new MongoBroker(Creator, Reader, null, null);
            Broker.Initialize("test");
            Broker.DeleteEverything();

            Publisher = new MessagePublisher(hostName, exchangeName, queueName);
            var autoConsumer = new AutoMessageConsumer(hostName, queueName);
            Reader = new MongoReader(databaseName, collectionName);
            var objectCreator = new MongoCreator(Reader, databaseName, collectionName);
            ConsumerCreator = new AutoConsumerCreator(autoConsumer, objectCreator);
            // Subscribe
            ConsumerCreator.EntryCreated += ConsumerCreator_EntryCreated;
            AutoResetEvent = new AutoResetEvent(false);
        }

        [Test]
        public void OnMessageReceived_WithObjectInMessage_CreatesEntry()
        {
            dynamic @object = new
            {
                _id = 1,
                Content = "new object"
            };
            JObject jObject = JObject.FromObject(@object);
            var message = jObject.ToString();
            ConsumerCreator.Start();

            Publisher.PublishMessage(message);
            AutoResetEvent.WaitOne(); //Wait until entry created

            var jObjects = Reader.ReadObjects();
            var actualJObject = jObjects.FirstOrDefault();
            Assert.AreEqual("", actualJObject["Content"]);
            ConsumerCreator.Stop();
        }

        private void ConsumerCreator_EntryCreated(object sender, EventArgs e)
        {
            AutoResetEvent.Set();
        }

    }
}
