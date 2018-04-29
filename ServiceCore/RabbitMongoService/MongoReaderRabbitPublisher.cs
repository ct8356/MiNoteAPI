using DatabaseInterfaces;
using Newtonsoft.Json;
using ServiceInterfaces;

namespace RabbitMongoService
{
    public class MongoReaderRabbitPublisher : IObjectReaderMessagePublisher
    {
        IObjectReader _objectReader;
        IMessagePublisher _messagePublisher;

        public MongoReaderRabbitPublisher
            (IObjectReader objectReader, IMessagePublisher messagePublisher)
        {
            _objectReader = objectReader;
            _messagePublisher = messagePublisher;          
        }

        public void ReadObjectsAndPublishMessage()
        {
            var objects = _objectReader.ReadObjects();
            var message = JsonConvert.SerializeObject(objects, Formatting.Indented);
            _messagePublisher.PublishMessage(message);
        }

    }
}