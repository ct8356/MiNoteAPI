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
        protected MongoDbObjectRepository _repo;
        protected RabbitPublisher _publisher;
        protected RabbitAutoConsumerMongoCreator _consumerCreator;
        protected MongoReader _objectReader;
        protected AutoResetEvent _autoResetEvent;

        [OneTimeSetUp]
        public void SetUp()
        {
            var hostName = "localHost";
            var exchangeName = "";
            var queueName = "CreateNote";
            _repo = new MongoDbObjectRepository();
            _repo.Initialize("test");
            _repo.DeleteEverything();

            _publisher = new RabbitPublisher()
            {
                HostName = hostName,
                ExchangeName = exchangeName,
            };

            var autoConsumer = new RabbitAutoConsumer(hostName, queueName);
            _objectReader = new MongoReader()
            {
                Documents = _repo.Documents,
            };
            var objectCreator = new MongoCreator()
            {
                ObjectReader = _objectReader,
                Documents = _repo.Documents,
            };
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

            _publisher.PublishMessage("CreateNote", message);
            _autoResetEvent.WaitOne(); //Wait until message received.

            var jObjects = _objectReader.ReadObjects();
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
