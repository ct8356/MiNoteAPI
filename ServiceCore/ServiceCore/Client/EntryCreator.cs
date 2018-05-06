using DatabaseInterfaces;
using Newtonsoft.Json.Linq;
using ServiceInterfaces;

namespace RabbitCore
{
    public class EntryCreator : IObjectCreator
    {
        IMessagePublisher _messagePublisher;

        public EntryCreator(IMessagePublisher messagePublisher)
        {
            _messagePublisher = messagePublisher;
        }

        public void CreateObject(JObject jObject)
        {
            var message = jObject.ToString();
            _messagePublisher.PublishMessage(message);
        }

    }
}