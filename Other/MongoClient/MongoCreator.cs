using DatabaseInterfaces;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace MongoClient
{
    public class MongoCreator : IObjectCreator
    {
        public IObjectReader ObjectReader { get; set; }
        public IMongoCollection<BsonDocument> Documents { get; set; }

        public MongoCreator()
        {
        }

        public void CreateObject(JObject jObject)
        {
            var objects = ObjectReader.ReadObjects();
            var maxId = (int?)objects.Max(o => o["_id"]);
            var newId = (maxId ?? 0) + 1;
            jObject["_id"] = newId;
            var doc = BsonDocument.Parse(jObject.ToString());
            //sadly,  jObject.ToBsonDocument() does not work. (get lots of unwanted props).
            Documents.InsertOne(doc);
            //AH! I think if object gets casted to a dynamic object,
            //THEN it gets funny properties like _t!!
            //AND puts all props in a values object.
        }

    }
}