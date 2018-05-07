using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RabbitCore;
using RabbitMongoService;

namespace IntegrationTests
{
    [TestFixture]
    public class AutoConsumerDeleterTests : RabbitMongoTestBase
    {

        MessagePublisher Publisher;
        AutoConsumerDeleter ConsumerDeleter;

        [OneTimeSetUp]
        public new void OneTimeSetUp()
        {
            base.OneTimeSetUp();

            Publisher = new MessagePublisher(HostName, ExchangeName, DeleterQueueName);
            ConsumerDeleter = new AutoConsumerDeleter(
                new AutoMessageConsumer(HostName, DeleterQueueName),
                Deleter);

            // Subscribe
            ConsumerDeleter.EntryDeleted += delegate { AutoResetEvent.Set(); };
        }

        [SetUp]
        public new void SetUp()
        {
            base.SetUp();
        }

        [Test]
        public void OnMessageReceived_WithIdInMessage_DeletesEntry()
        {
            CreateEntry("new object");
            ConsumerDeleter.Start();

            Publisher.PublishMessage("1");
            AutoResetEvent.WaitOne(); //Wait until entry deleted

            var jObjects = Reader.ReadObjects();
            Assert.AreEqual(0, jObjects.Count);
            ConsumerDeleter.Stop();
        }

    }
}
