using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using ServiceCore;
using System.Linq;

namespace IntegrationTests
{
    [TestFixture]
    public class RabbitConsumerMongoCreatorReaderTests
    {
        protected RabbitConsumerMongoCreator _consumerCreator;
        protected MongoReaderRabbitPublisher _readerPublisher;

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
            var client = new MongoClient();
            var database = client.GetDatabase("test");
            var documents = database.GetCollection<BsonDocument>("Objects");
            var objectReader = new MongoReader()
            {
                Documents = documents,
            };
            var objectCreator = new MongoCreator()
            {
                ObjectReader = objectReader,
                Documents = documents,
            };
            var messagePublisher = new RabbitPublisher()
            {
                HostName = hostName,
                ExchangeName = exchangeName,
            };
            _consumerCreator = new RabbitConsumerMongoCreator(messageConsumer, objectCreator);
            _readerPublisher = new MongoReaderRabbitPublisher(objectReader, messagePublisher);
        }

        [Test]
        public void PublishRequest_ConsumeResult_Works()
        {
            dynamic @object = new
            {
                _id = 1,
                Content = "new object"
            };
            JObject jObject = JObject.FromObject(@object);


        }

    }
}
