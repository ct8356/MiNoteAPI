using DatabaseInterfaces;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace MongoClient
{
    public class MongoBroker : IMongoEntryBroker
    {

        IMongoCollection<BsonDocument> Documents { get; set; }
        IMongoCollection<dynamic> Objects { get; set; }
        IObjectCreator _creator;
        IObjectReader _reader;
        IObjectUpdater _updater;
        IObjectDeleter _deleter;

        public MongoBroker(
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
            var client = new MongoDB.Driver.MongoClient();
            var Database = client.GetDatabase(databaseName);
            Documents = Database.GetCollection<BsonDocument>("Objects");
            Objects = Database.GetCollection<dynamic>("Objects");
            //Documents is needed for adding to db, and reading from db (deserializing).
        }

        public void ConditionalSeed()
        {
            var documents = Documents.Find(new BsonDocument()).ToList();
            bool collectionIsEmpty = documents.Count() < 1;
            if (collectionIsEmpty)
            {
                SeedWithDynamicObjects();
            }
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

        public void DeleteEverything()
        {
            Documents.DeleteMany("{}");
        }

        #region Seed methods

        private void SeedWithDynamicObjects()
        {
            var documents = new List<dynamic>() {
                new {
                    _id = 1,
                    Content = "Hello",
                },
                new {
                    _id = 2,
                    Content = "World",
                },
            };
            Objects.InsertMany(documents);
        }

        /*private void SeedWithNotes()
        {
            var notes = new List<Note>() {
                new Note() {
                    Id = 1,
                    Content = "Hello",
                },
                new Note() {
                    Id = 2,
                    Content = "World",
                },
            };
            Objects.InsertMany(notes);
        }

        private void SeedWithNotesViaDocuments()
        {
            var notes = new List<Note>() {
                new Note() {
                    Id = 1,
                    Content = "Hello",
                },
                new Note() {
                    Id = 2,
                    Content = "World",
                },
            };
            foreach (var note in notes)
            {
                Documents.InsertOne(note.ToBsonDocument());
            }
        }*/

        private void SeedWithBsonDocuments()
        {
            var documents = new List<BsonDocument>() {
                new BsonDocument() {
                    { "Content", "Hello" },
                },
                new BsonDocument() {
                    { "Content", "World" },
                },
            };
            Documents.InsertMany(documents);
        }

        #endregion

    }
}