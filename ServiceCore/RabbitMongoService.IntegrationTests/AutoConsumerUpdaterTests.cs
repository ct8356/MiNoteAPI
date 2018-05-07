using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RabbitCore;
using RabbitMongoService;
using System.Linq;

namespace IntegrationTests
{
    [TestFixture]
    public class EntryUpdaterTests : RabbitMongoTestBase
    {

        MessagePublisher Publisher;
        AutoConsumerUpdater ConsumerUpdater;

        [OneTimeSetUp]
        public new void OneTimeSetUp()
        {
            base.OneTimeSetUp();

            Publisher = new MessagePublisher(HostName, ExchangeName, UpdaterQueueName);
            ConsumerUpdater = new AutoConsumerUpdater(
                new AutoMessageConsumer(HostName, UpdaterQueueName),
                Updater);

            // Subscribe
            ConsumerUpdater.EntryUpdated += delegate { AutoResetEvent.Set(); };
        }

        [Test]
        public void OnMessageReceived_WithNewtonsoftJObject_UpdatesObject()
        {
            dynamic @object = new
            {
                _id = 1,
                Content = "new object"
            };
            JObject jObject = JObject.FromObject(@object);
            Creator.CreateObject(jObject);

            dynamic uObject = new
            {
                _id = 1,
                Content = "updated object"
            };
            JObject newJObject = JObject.FromObject(uObject);
            var message = newJObject.ToString();
            ConsumerUpdater.Start();

            Publisher.PublishMessage(message);
            AutoResetEvent.WaitOne(); //Wait until entry updated

            var actualObject = Reader.ReadObjects().First(o => (int)o["_id"] == 1);
            Assert.AreEqual(1, (int)actualObject["_id"]);
            Assert.AreEqual("updated object", (string)actualObject["Content"]);
            ConsumerUpdater.Stop();
        }

    }
}
