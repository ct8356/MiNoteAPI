using MongoClient;
using Newtonsoft.Json.Linq;

namespace IntegrationTests
{
    public class TestBase
    {
        protected EntryCreator Creator;
        protected EntryReader Reader;
        protected EntryUpdater Updater;
        protected EntryDeleter Deleter;
        protected EntryBroker Broker;

        string DatabaseName = "Test";
        string CollectionName = "Objects";

        protected virtual void OneTimeSetUp()
        {
            Reader = new EntryReader(DatabaseName, CollectionName);
            Creator = new EntryCreator(Reader, DatabaseName, CollectionName);          
            Updater = new EntryUpdater(DatabaseName, CollectionName);
            Deleter = new EntryDeleter(DatabaseName, CollectionName);
            Broker = new EntryBroker(Creator, Reader, Updater, Deleter);
            Broker.Initialize(DatabaseName);  
        }

        protected virtual void SetUp()
        {
            Broker.DeleteEverything();
        }

        protected void CreateEntry(string contents)
        {
            dynamic @object = new
            {
                _id = 1,
                Content = contents
            };
            var jObject = JObject.FromObject(@object);
            Creator.CreateObject(jObject);
        }

    }
}
