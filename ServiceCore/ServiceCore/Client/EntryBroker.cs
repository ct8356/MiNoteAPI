using DatabaseInterfaces;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace RabbitCore
{
    public class EntryBroker : IRabbitEntryBroker
    {
        IEntryCreator _creator;
        IEntryReader _reader;
        IEntryUpdater _updater;
        IEntryDeleter _deleter;

        public EntryBroker(
            IEntryCreator creator,
            IEntryReader reader,
            IEntryUpdater updater,
            IEntryDeleter deleter)
        {
            _creator = creator;
            _reader = reader;
            _updater = updater;
            _deleter = deleter;
        }

        public void Initialize(string databaseName)
        {
            // Do nothing.
            // Assume db is already initialised.
        }

        public void CreateObject(JObject jObject)
        {
            _creator.CreateObject(jObject);
        }

        public ICollection<JObject> ReadObjects()
        {
            return _reader.ReadObjects();
        }

        public void UpdateObject(JObject jObject)
        {
            _updater.UpdateObject(jObject);
        }

        public void DeleteObject(int id)
        {
            _deleter.DeleteObject(id);
        }

    }
}