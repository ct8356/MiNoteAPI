using ServiceInterfaces;

namespace RabbitMongoService
{
    public class Service : IService
    { 

        IAutoConsumerCreator _consumerCreator;
        IAutoConsumerReader _readerPublisher;

        public Service(
            IAutoConsumerCreator consumerCreator, 
            IAutoConsumerReader readerPublisher,
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
