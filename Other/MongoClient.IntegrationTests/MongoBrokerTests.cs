using MongoClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System.Linq;

namespace IntegrationTests
{
    [TestFixture]
    public class MongoBrokerTests
    {
        MongoCreator Creator { get; set; }
        MongoReader Reader { get; set; }
        MongoBroker Broker;

        [SetUp]
        public void SetUp()
        {
            var databaseName = "test";
            var collectionName = "Objects";
            Reader = new MongoReader(databaseName, collectionName);
            Creator = new MongoCreator(Reader, databaseName, collectionName);
            Broker = new MongoBroker(Creator, Reader, null, null);
            var repo = Broker;
            repo.Initialize("test");
            repo.DeleteEverything();
        }

        [Test]
        public void CreateObject_WithNothingInDb_AddsObjectWithId1()
        {
            dynamic @object = new
            {
                _id = 1,
                Content = "new object"
            };
            JObject jObject = JObject.FromObject(@object);

            var repository = Broker;
            repository.CreateObject(jObject);

            var jObjects = repository.ReadObjects();
            int actualId = (int)jObjects.Max(o => o["_id"]);
            Assert.AreEqual(1, actualId);      
        }

        [Test]
        public void CreateObject_WithObjectsInDb_AddsObjectWithNewId()
        {
            dynamic @object = new
            {
                _id = 1,
                Content = "new object"
            };
            JObject jObject = JObject.FromObject(@object);

            var repository = Broker;
            repository.ConditionalSeed();
            repository.CreateObject(jObject);

            var jObjects = repository.ReadObjects();
            int actualId = (int)jObjects.Max(o => o["_id"]);
            Assert.AreEqual(3, actualId);
        }

        [Test]
        public void CreateObject_WithJObject_AddsObjectWithIdAndContent()
        {
            dynamic @object = new
            {
                _id = 1,
                Content = "new object"
            };
            JObject jObject = JObject.FromObject(@object);

            Broker.CreateObject(jObject);

            var jObjects = Broker.ReadObjects();
            int actualId = (int)jObjects.Max(o => o["_id"]);
            Assert.AreEqual(1, actualId);
            var actualObject = Broker.ReadObjects().First(o => (int)o["_id"] == actualId);
            Assert.AreEqual("new object", (string)actualObject["Content"]);
        }

        [Test]
        public void UpdateObject_WithJObject_UpdatesObject()
        {
            dynamic @object = new
            {
                _id = 1,
                Content = "new object"
            };
            JObject jObject = JObject.FromObject(@object);

            var repository = Broker;
            repository.ConditionalSeed();
            repository.UpdateObject(jObject);

            var actualObject = repository.ReadObjects().First(o => (int)o["_id"] == 1);
            Assert.AreEqual(1, (int)actualObject["_id"]);
            Assert.AreEqual("new object", (string)actualObject["Content"]);
        }

        [Test]
        public void DeleteObject_WithId_DeletesObject()
        {
            var repository = Broker;
            repository.ConditionalSeed();
            repository.DeleteObject(1);

            var actualObjects = repository.ReadObjects();
            Assert.AreEqual(1, actualObjects.Count());
        }

        #region Related experiments

        [Test]
        public void ConvertJObjectToDynamicObject()
        {
            dynamic @object = new
            {
                _id = 1,
                Content = "new object"
            };
            JObject jObject = JObject.FromObject(@object);

            string expectedString = "{\"_id\":1,\"Content\":\"new object\"}";
            Assert.AreEqual(expectedString, jObject.ToString(Formatting.None));
        }

        [Test]
        public void CastJObjectToDynamicObject()
        {
            dynamic @object = new
            {
                _id = 1,
                Content = "new object"
            };
            JObject jObject = JObject.FromObject(@object);
            dynamic actualObject = jObject;
            //This does not create a dynamic object like you wanted,
            //It just creates a JObject.
            //This won't get inserted into Objects<dynamic> nicely.

            string expectedString = "{\"_id\":1,\"Content\":\"new object\"}";
            Assert.AreEqual(expectedString, jObject.ToString(Formatting.None));
        }

        #endregion Related experiments

    }
}
