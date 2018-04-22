using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using ServiceInterfaces;
using System.Linq;

namespace ServiceCore
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
            var newId = objects.Max(o => o._id) + 1;
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