using ServiceInterfaces;

namespace RabbitMongoService
{
    public class Service : IService
    { 

        IAutoConsumerCreator ConsumerCreator;
        IAutoConsumerReader ConsumerReader;

        public Service(
            IAutoConsumerCreator consumerCreator, 
            IAutoConsumerReader readerPublisher)
        {
            ConsumerCreator = consumerCreator;
            ConsumerReader = readerPublisher;
        }

        public void Start()
        {
            ConsumerCreator.Start();
            ConsumerReader.Start();
        }

        public void Stop()
        {
            ConsumerCreator.Stop();
            ConsumerReader.Stop();
        }

    }
}
