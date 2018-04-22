using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ServiceCore
{

    public class MongoDbObjectRepository : IObjectBroker
    {
        public IMongoDatabase Database { get; set; }
        public IMongoCollection<BsonDocument> Documents { get; set; }
        public IMongoCollection<dynamic> Objects { get; set; }

        public MongoDbObjectRepository()
        {
            //Parameterless constructor gets used by controller.
        }

        public void Initialize(string databaseName)
        {
            // Connect to MongoDB instance running on localhost
            var client = new MongoClient();
            Database = client.GetDatabase(databaseName);
            Documents = Database.GetCollection<BsonDocument>("Objects"); 
            //Documents is needed for adding to db, and reading from db (deserializing).
            Objects = Database.GetCollection<dynamic>("Objects");
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
            var objects = ReadObjects();
            var newId = (int)objects.Max(o => o["_id"]) + 1;
            jObject["_id"] = newId;
            var doc = BsonDocument.Parse(jObject.ToString());
            //sadly,  jObject.ToBsonDocument() does not work. (get lots of unwanted props).
            Documents.InsertOne(doc);
            //AH! I think if object gets casted to a dynamic object,
            //THEN it gets funny properties like _t!!
            //AND puts all props in a values object.
        }

        public ICollection<JObject> ReadObjects()
        {
            var objects = new List<JObject>();
            //For some reason, I can only get below to work for Documents.
            //When I use Objects, Deserialize breaks. Cannot handle it.
            //Think its because Deserialize, has to accept documents!
            //"Find" might work with different type from Document. Can experiment later.
            var documents = Documents.Find(new BsonDocument()).ToList();
            for (int i = 0; i < documents.Count; i++)
            {
                var @object = BsonSerializer.Deserialize<dynamic>(documents[i]);
                //pretty sure the deserialize method converts _id to Id, if put Note in <type>.
                objects.Add(JObject.FromObject(@object));
            }
            return objects;
        }

        public void UpdateObject(JObject jObject)
        {
            var id = Convert.ToInt32(jObject["_id"]);
            var filter = new BsonDocument("_id", id);
            var doc = BsonDocument.Parse(jObject.ToString());
            Documents.ReplaceOne(filter, doc);
        }

        public void DeleteObject(int id)
        {
            var filter = new BsonDocument("_id", id);
            Objects.DeleteOne(filter);
        }

        public void DeleteEverything()
        {
            Documents.DeleteMany("{}");
            Objects.DeleteMany("{}");
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

        #endregion Seed methods

    }
}