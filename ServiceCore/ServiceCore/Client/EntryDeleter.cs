using DatabaseInterfaces;
using ServiceInterfaces;

namespace RabbitCore
{
    public class EntryDeleter : IEntryDeleter
    {
        IMessagePublisher _messagePublisher;

        public EntryDeleter(IMessagePublisher messagePublisher)
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