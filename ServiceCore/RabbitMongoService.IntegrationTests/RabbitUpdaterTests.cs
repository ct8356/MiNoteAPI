using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RabbitCore;
using System.Linq;

namespace IntegrationTests
{
    [TestFixture]
    public class RabbitUpdaterTests
    {
        protected EntryCreator _creator;
        protected EntryReader _reader;
        protected RabbitUpdater _updater;

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
            _updater = new RabbitUpdater(messagePublisher);
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

            _updater.UpdateObject(jObject);

            var actualObject = _reader.ReadObjects().First(o => (int)o["_id"] == 1);
            Assert.AreEqual(1, actualObject["_id"]);
            Assert.AreEqual("new object", actualObject["Content"]);    
        }

    }
}
