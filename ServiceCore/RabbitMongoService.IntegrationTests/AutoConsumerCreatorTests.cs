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
    public class AutoConsumerCreatorTests : RabbitMongoTestBase
    {
        MessagePublisher Publisher;
        AutoConsumerCreator ConsumerCreator;    

        [OneTimeSetUp]
        public new void OneTimeSetUp()
        {
            base.OneTimeSetUp();

            Publisher = new MessagePublisher(HostName, ExchangeName, CreatorQueueName);
            ConsumerCreator = new AutoConsumerCreator(
                new AutoMessageConsumer(HostName, CreatorQueueName), 
                Creator);

            // Subscribe
            ConsumerCreator.EntryCreated += ConsumerCreator_EntryCreated;
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
