﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using ServiceCore;
using System.Linq;

namespace IntegrationTests
{
    [TestFixture]
    public class MongoDbObjectRepositoryTests
    {

        protected MongoDbObjectRepository _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new MongoDbObjectRepository();
            var repo = _sut;
            repo.Initialize("test");
            repo.DeleteEverything();
            repo.ConditionalSeed();
        }

        [Test]
        public void CreateObject_WithNewtonsoftJObject_AddsObjectWithNewId()
        {
            dynamic @object = new
            {
                _id = 1,
                Content = "new object"
            };
            JObject jObject = JObject.FromObject(@object);

            var repository = _sut;
            repository.CreateObject(jObject);

            var jObjects = repository.ReadObjects();
            int actualId = (int)jObjects.Max(o => o["_id"]);
            Assert.AreEqual(3, actualId);
            var actualObject = repository.ReadObjects().First(o => (int)o["_id"] == actualId);
            Assert.AreEqual("new object", (string)actualObject["Content"]);
        }

        [Test]
        public void UpdateObject_WithNewtonsoftJObject_UpdatesObject()
        {
            dynamic @object = new
            {
                _id = 1,
                Content = "new object"
            };
            JObject jObject = JObject.FromObject(@object);

            var repository = _sut;
            repository.UpdateObject(jObject);

            var actualObject = repository.ReadObjects().First(o => (int)o["_id"] == 1);
            Assert.AreEqual(1, (int)actualObject["_id"]);
            Assert.AreEqual("new object", (string)actualObject["Content"]);
        }

        [Test]
        public void DeleteObject_WithId_DeletesObject()
        {
            var repository = _sut;
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
