using DatabaseInterfaces;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using System;

namespace MongoClient
{
    public class MongoUpdater : IObjectUpdater
    {
        IMongoCollection<BsonDocument> Documents { get; set; }

        public MongoUpdater(string databaseName, string collectionName)
        {
            var client = new MongoDB.Driver.MongoClient();
            var database = client.GetDatabase(databaseName);
            Documents = database.GetCollection<BsonDocument>(collectionName);
        }

        public void UpdateObject(JObject jObject)
        {
            var id = Convert.ToInt32(jObject["_id"]);
            var filter = new BsonDocument("_id", id);
            var doc = BsonDocument.Parse(jObject.ToString());
            Documents.ReplaceOne(filter, doc);
        }

    }
}