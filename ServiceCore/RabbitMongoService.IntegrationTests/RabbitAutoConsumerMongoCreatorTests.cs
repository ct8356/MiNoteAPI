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
    public class RabbitAutoConsumerMongoCreatorReaderTests
    {
        protected MongoBroker _repo;
        protected RabbitPublisher _publisher;
        protected RabbitAutoConsumerMongoCreator _consumerCreator;
        protected MongoCreator Creator;
        protected MongoReader Reader;
        protected AutoResetEvent _autoResetEvent;

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
            _repo = new MongoBroker(Creator, Reader, null, null);
            _repo.Initialize("test");
            _repo.DeleteEverything();

            _publisher = new RabbitPublisher(hostName, exchangeName, queueName);
            var autoConsumer = new RabbitAutoConsumer(hostName, queueName);
            Reader = new MongoReader(databaseName, collectionName);
            var objectCreator = new MongoCreator(Reader, databaseName, collectionName);
            _consumerCreator = new RabbitAutoConsumerMongoCreator(autoConsumer, objectCreator);
            _consumerCreator.EntryCreated += _consumerCreator_EntryCreated;
            _autoResetEvent = new AutoResetEvent(false);
        }

        [Test]
        public void PublishMessage_WithObjectInMessage_CreatesMongoEntry()
        {
            dynamic @object = new
            {
                _id = 1,
                Content = "new object"
            };
            JObject jObject = JObject.FromObject(@object);
            var message = jObject.ToString();
            _consumerCreator.Start();

            _publisher.PublishMessage(message);
            _autoResetEvent.WaitOne(); //Wait until message received.

            var jObjects = Reader.ReadObjects();
            var actualJObject = jObjects.FirstOrDefault();
            Assert.AreEqual("", actualJObject["Content"]);
            _consumerCreator.Stop();
            //Works, but does take some time.
            /* Also, is the risk that if already backlog of messages on the queue,
             * you start the consumerCreator,
             * and it creates loads of objects.
             * Well, actually, that is what you want! ok!
             */
        }

        private void _consumerCreator_EntryCreated(object sender, EventArgs e)
        {
            _autoResetEvent.Set();
        }

    }
}
