using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System.Linq;

namespace IntegrationTests
{
    [TestFixture]
    public class MongoBrokerTests : TestBase
    {

        [OneTimeSetUp]
        public new void OneTimeSetUp()
        {
            base.OneTimeSetUp();
        }

        [SetUp]
        public new void SetUp()
        {
            base.SetUp();
        }

        [Test]
        public void CreateObject_WithNothingInDb_AddsObjectWithId1()
        {
            CreateEntry("new object");

            var jObjects = Broker.ReadObjects();
            int actualId = (int)jObjects.Max(o => o["_id"]);
            Assert.AreEqual(1, actualId);      
        }

        [Test]
        public void CreateObject_WithObjectsInDb_AddsObjectWithNewId()
        {
            CreateEntry("new object");
            CreateEntry("new object");

            var jObjects = Broker.ReadObjects();
            int actualId = (int)jObjects.Max(o => o["_id"]);
            Assert.AreEqual(2, actualId);
        }

        [Test]
        public void CreateObject_WithJObject_AddsObjectWithIdAndContent()
        {
            CreateEntry("new object");

            var jObjects = Broker.ReadObjects();
            int actualId = (int)jObjects.Max(o => o["_id"]);
            Assert.AreEqual(1, actualId);
            var actualObject = Broker.ReadObjects().First(o => (int)o["_id"] == actualId);
            Assert.AreEqual("new object", (string)actualObject["Content"]);
        }

        [Test]
        public void UpdateObject_WithJObject_UpdatesObject()
        {
            CreateEntry("new object");
            dynamic @object = new
            {
                _id = 1,
                Content = "updated object"
            };
            JObject jObject = JObject.FromObject(@object);

            Broker.UpdateObject(jObject);

            var actualObject = Broker.ReadObjects().First(o => (int)o["_id"] == 1);
            Assert.AreEqual(1, (int)actualObject["_id"]);
            Assert.AreEqual("updated object", (string)actualObject["Content"]);
        }

        [Test]
        public void DeleteObject_WithId_DeletesObject()
        {
            CreateEntry("new object");

            Broker.DeleteObject(1);

            var actualObjects = Broker.ReadObjects();
            Assert.AreEqual(0, actualObjects.Count());
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
