using DatabaseInterfaces;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RabbitCore;
using System.Linq;

namespace IntegrationTests
{
    [TestFixture]
    public class RabbitCreatorReaderTests
    {
        protected RabbitCreator _creator;
        protected RabbitReader _reader;  

        [OneTimeSetUp]
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
                ExchangeName = exchangeName,
            };
            _creator = new RabbitCreator(messagePublisher);
            _reader = new RabbitReader(messageConsumer, messagePublisher);     
            /* This will only work if 
             * have a RabbitMongoService or something, running in background.
             * SO these tests should be in RabbitMongoService, probs.
             * 
             */
        }

        [Test]
        public void CreateObject_ReadObjects_Works()
        {
            dynamic @object = new
            {
                _id = 1,
                Content = "new object"
            };
            JObject jObject = JObject.FromObject(@object);

            _creator.CreateObject(jObject);

            var actualObjects = _reader.ReadObjects();
            var actualId = actualObjects.Max(o => o["_id"]);
            Assert.AreEqual(3, actualId);
            var actualObject = actualObjects.First(o => o["_id"] == actualId);
            Assert.AreEqual("new object", actualObject["Content"]);
        }

    }
}
