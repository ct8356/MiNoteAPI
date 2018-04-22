using Newtonsoft.Json.Linq;
using NoteService;
using NUnit.Framework;
using ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NUnitTests
{
    [TestFixture]
    public class FakeObjectRepositoryTests
    {

        protected IObjectBroker _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new FakeObjectRepository();
        }

        [Test]
        public void CreateObject_WithObject_AddsObjectWithNewId()
        {
            dynamic @object = new
            {
                _id = 1,
                Content = "new object"
            };
            JObject jObject = JObject.FromObject(@object);

            var repository = _sut;
            repository.CreateObject(jObject);

            var actualId = repository.ReadObjects().Max(o => o.Id);
            Assert.That(actualId, Is.EqualTo(3));
            var actualObject = repository.ReadObjects().First(o => o.Id == actualId);
            Assert.That(actualObject.Content, Is.EqualTo("new object"));
        }

    }
}
