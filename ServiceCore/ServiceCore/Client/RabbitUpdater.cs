using DatabaseInterfaces;
using Newtonsoft.Json.Linq;
using ServiceInterfaces;

namespace RabbitCore
{
    public class RabbitUpdater : IObjectUpdater
    {
        IMessagePublisher _messagePublisher;

        public RabbitUpdater(IMessagePublisher messagePublisher)
        {
            _messagePublisher = messagePublisher;
        }

        public void UpdateObject(JObject jObject)
        {
            var message = jObject.ToString();
            _messagePublisher.PublishMessage("UpdateNote", message);
        }

    }
}