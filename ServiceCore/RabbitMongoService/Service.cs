using ServiceInterfaces;

namespace RabbitMongoService
{
    public class Service : IService
    { 

        IAutoMessageConsumerObjectCreator _consumerCreator;
        IObjectReaderMessagePublisher _readerPublisher;

        public Service(
            IAutoMessageConsumerObjectCreator consumerCreator, 
            IObjectReaderMessagePublisher readerPublisher,
            string databaseName)
        {
            _consumerCreator = consumerCreator;
            _readerPublisher = readerPublisher;
        }

        public void Start()
        {
            _consumerCreator.Start();
        }

        public void Stop()
        {
            _consumerCreator.Stop();
        }

    }
}
