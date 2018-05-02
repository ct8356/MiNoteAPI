using DatabaseInterfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoClient
{
    public class MongoDeleter : IObjectDeleter
    {
        IMongoCollection<BsonDocument> Documents { get; set; }

        public MongoDeleter(string databaseName, string collectionName)
        {
            var client = new MongoDB.Driver.MongoClient();
            var database = client.GetDatabase(databaseName);
            Documents = database.GetCollection<BsonDocument>(collectionName);
        }

        public void DeleteObject(int id)
        {
            var filter = new BsonDocument("_id", id);
            //Objects.DeleteOne(filter);
            Documents.DeleteOne(filter);
        }

    }
}