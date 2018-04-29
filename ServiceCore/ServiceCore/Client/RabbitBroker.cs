using DatabaseInterfaces;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace RabbitCore
{
    public class RabbitBroker : IObjectBroker
    {
        IObjectCreator _creator;
        IObjectReader _reader;
        IObjectUpdater _updater;
        IObjectDeleter _deleter;

        public RabbitBroker(
            IObjectCreator creator,
            IObjectReader reader,
            IObjectUpdater updater,
            IObjectDeleter deleter)
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