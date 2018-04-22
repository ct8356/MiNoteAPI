using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using ServiceInterfaces;
using System.Collections.Generic;
using System.Linq;

namespace ServiceCore
{
    public class MongoReader : IObjectReader
    {
        public IMongoCollection<BsonDocument> Documents { get; set; }

        public MongoReader()
        {
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
                objects.Add(BsonSerializer.Deserialize<JObject>(documents[i]));
                //pretty sure the deserialize method converts _id to Id, if put Note in <type>.
            }
            return objects;
        }

    }
}