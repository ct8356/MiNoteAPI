using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RabbitCore;
using System.Linq;

namespace IntegrationTests
{
    [TestFixture]
    public class EntryCreatorReaderTests
    {
        EntryCreator Creator;
        EntryReader Reader;

        [OneTimeSetUp]
        public void SetUp()
        {
            var hostName = "localhost";
            var requestQueueName = "CreateNote";
            var exchangeName = "";
            var responseQueueName = "NoteApi";
            var messageConsumer = new MessageConsumer(hostName, responseQueueName);
            var messagePublisher = new MessagePublisher(hostName, exchangeName, requestQueueName);
            Creator = new EntryCreator(messagePublisher);
            Reader = new EntryReader(messageConsumer, messagePublisher);           
        }

        [Test]
        public void CreateObject_ReadObjects_ReturnsCreatedObject()
        {
            dynamic @object = new
            {
                _id = 1,
                Content = "new object"
            };
            JObject jObject = JObject.FromObject(@object);

            Creator.CreateObject(jObject);

            //AH duh! this test is just creating message,
            //then reading of same queue!
            //You need to use different queueNames,
            //AND construct all the Mongo bits too!
            var actualObjects = Reader.ReadObjects();
            var actualId = actualObjects.Max(o => o["_id"]);
            Assert.AreEqual(3, actualId);
            var actualObject = actualObjects.First(o => o["_id"] == actualId);
            Assert.AreEqual("new object", actualObject["Content"]);
        }

    }
}
