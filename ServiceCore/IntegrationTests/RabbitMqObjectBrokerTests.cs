using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServiceCore;
using NUnit.Framework;
using System.Linq;

namespace IntegrationTests
{
    [TestFixture]
    public class RabbitMqObjectBrokerTests
    {
        protected RabbitMqObjectBroker _sut;
        //protected MessageConsumer _messageConsumer;
        //protected MessagePublisher _messagePublisher;

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
            _sut = new RabbitMqObjectBroker(messageConsumer, messagePublisher);
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

            var broker = _sut;
            broker.CreateObject(jObject);

            var actualId = broker.ReadObjects().Max(o => o._id);
            Assert.AreEqual(3, actualId);
            var actualObject = broker.ReadObjects().First(o => o._id == actualId);
            Assert.AreEqual("new object", actualObject.Content);
        }

        [Test]
        public void UpdateObject_WithNewtonsoftJObject_UpdatesObject()
        {
            dynamic @object = new
            {
                _id = 1,
                Content = "new object"
            };
            JObject jObject = JObject.FromObject(@object);

            var broker = _sut;
            broker.UpdateObject(jObject);

            var actualObject = broker.ReadObjects().First(o => o._id == 1);
            Assert.AreEqual(1, actualObject._id);
            Assert.AreEqual("new object", actualObject.Content);    
        }

        [Test]
        public void DeleteObject_WithId_DeletesObject()
        {
            var broker = _sut;
            broker.DeleteObject(1);

            var actualObjects = broker.ReadObjects();
            Assert.AreEqual(1, actualObjects.Count());
        }

    }
}
