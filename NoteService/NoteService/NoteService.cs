using RabbitMongoService;
using ServiceInterfaces;

namespace NoteService
{
    public class NoteService : Service
    { 
        /* Don't really even need this class. Or project.
         * All this is here for, is to specify name of database and/or collection.
         * Can do that from Unity container.
         * So try that! Register<IService>().With<RabbitMongoService>().WithParams("dbName");
         */

        public NoteService(
            IAutoConsumerCreator consumerCreator,
            IAutoConsumerReader readerPublisher
        ) : base(consumerCreator, readerPublisher, "Notes")
        {
        }

    }
}
