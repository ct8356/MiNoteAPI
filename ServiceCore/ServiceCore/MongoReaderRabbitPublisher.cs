using Newtonsoft.Json.Linq;
using ServiceInterfaces;

namespace ServiceCore
{
    public class MongoReaderRabbitPublisher : IObjectReaderMessagePublisher
    {
        IObjectReader _objectReader;
        IMessagePublisher _messagePublisher;

        public MongoReaderRabbitPublisher
            (IObjectReader objectReader, IMessagePublisher messagePublisher)
        {
            _objectReader = objectReader;
            messagePublisher.HostName = "localhost";
            messagePublisher.ExchangeName = "";
            _messagePublisher = messagePublisher;          
        }

        public void ReadObjectsAndPublishMessage()
        {
            var objects = _objectReader.ReadObjects();
            var message = objects.ToString();
            _messagePublisher.PublishMessage("NoteAPI", message);
        }

    }
}