using RabbitCore;
using System.Threading;

namespace IntegrationTests
{
    public class RabbitMongoTestBase : TestBase
    {
        protected EntryCreator RabbitCreator;
        protected EntryReader RabbitReader;
        protected EntryUpdater RabbitUpdater;
        protected EntryDeleter RabbitDeleter;
        protected EntryBroker RabbitBroker;

        protected AutoResetEvent AutoResetEvent;

        protected string HostName = "localHost";
        protected string ExchangeName = "";
        protected string ApiQueueName = "NoteApi";
        protected string CreatorQueueName = "CreateNote";
        protected string ReaderQueueName = "ReadNotes";
        protected string UpdaterQueueName = "UpdateNote";
        protected string DeleterQueueName = "DeleteNote";

        protected override void OneTimeSetUp()
        {
            base.OneTimeSetUp();
            RabbitCreator = new EntryCreator(
                new MessagePublisher(HostName, ExchangeName, CreatorQueueName));
            RabbitReader = new EntryReader(
                new MessageConsumer(HostName, ApiQueueName), 
                new MessagePublisher(HostName, ExchangeName, ReaderQueueName));
            RabbitUpdater = new EntryUpdater(
                new MessagePublisher(HostName, ExchangeName, UpdaterQueueName));
            RabbitDeleter = new EntryDeleter(
                new MessagePublisher(HostName, ExchangeName, DeleterQueueName));
            RabbitBroker = new EntryBroker(Creator, Reader, Updater, Deleter);

            AutoResetEvent = new AutoResetEvent(false);
        }

    }
}
