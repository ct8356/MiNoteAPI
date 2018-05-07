using DatabaseInterfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace MongoClient
{
    public class EntryReader : IEntryReader
    {
        IMongoCollection<BsonDocument> Documents { get; set; }

        public EntryReader(string databaseName, string collectionName)
        {
            var client = new MongoDB.Driver.MongoClient();
            var database = client.GetDatabase(databaseName);
            Documents = database.GetCollection<BsonDocument>(collectionName);
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

    }
}