using DatabaseInterfaces;
using ServiceInterfaces;

namespace RabbitCore
{
    public class RabbitDeleter : IObjectDeleter
    {
        IMessagePublisher _messagePublisher;

        public RabbitDeleter(IMessagePublisher messagePublisher)
        {
            _messagePublisher = messagePublisher;
        }

        public void DeleteObject(int id)
        {
            var message = id.ToString();
            _messagePublisher.PublishMessage("DeleteNote", message);
        }

    }
}