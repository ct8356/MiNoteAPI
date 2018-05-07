using DatabaseInterfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoClient
{
    public class EntryDeleter : IEntryDeleter
    {
        IMongoCollection<BsonDocument> Documents { get; set; }

        public EntryDeleter(string databaseName, string collectionName)
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