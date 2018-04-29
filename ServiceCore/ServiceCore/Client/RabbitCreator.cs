using DatabaseInterfaces;
using Newtonsoft.Json.Linq;
using ServiceInterfaces;

namespace RabbitCore
{
    public class RabbitCreator : IObjectCreator
    {
        IMessagePublisher _messagePublisher;

        public RabbitCreator(IMessagePublisher messagePublisher)
        {
            _messagePublisher = messagePublisher;
        }

        public void CreateObject(JObject jObject)
        {
            var message = jObject.ToString();
            _messagePublisher.PublishMessage("CreateNote", message);
        }

    }
}