using ServiceInterfaces;

namespace RabbitMongoService
{
    public class CrudService : IService
    { 

        IAutoConsumerCreator ConsumerCreator;
        IAutoConsumerReader ConsumerReader;
        IAutoConsumerUpdater ConsumerUpdater;
        IAutoConsumerDeleter ConsumerDeleter;

        public CrudService(
            IAutoConsumerCreator consumerCreator, 
            IAutoConsumerReader consumerReader,
            IAutoConsumerUpdater consumerUpdater,
            IAutoConsumerDeleter consumerDeleter)
        {
            ConsumerCreator = consumerCreator;
            ConsumerReader = consumerReader;
            ConsumerUpdater = consumerUpdater;
            ConsumerDeleter = consumerDeleter; 
        }

        public void Start()
        {
            ConsumerCreator.Start();
            ConsumerReader.Start();
            ConsumerUpdater.Start();
            ConsumerDeleter.Start();
        }

        public void Stop()
        {
            ConsumerCreator.Stop();
            ConsumerReader.Stop();
            ConsumerUpdater.Stop();
            ConsumerDeleter.Stop();
        }

    }
}
