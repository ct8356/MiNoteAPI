using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json.Linq;
using ServiceInterfaces;

namespace NoteAPI.Controllers
{
    public class NotesController : ApiController
    {

        private IObjectBroker _broker;

        public NotesController(IObjectBroker broker)
        {
            //NOTE, this controller gets called everytime you access the page.
            //BUT on first run, it takes its sweet time!
            //It even loads the page, before calling this constructor!
            _broker = broker;
            _broker.Initialize("test");
        }

        [HttpPost]
        public IHttpActionResult PostNote(JObject note)
        {
            _broker.CreateObject(note);
            return Ok();
        }

        public IEnumerable<dynamic> GetAllNotes()
        {
            //This method is recognised by view by naming convention.
            //It is part of the API.
            return _broker.ReadObjects();
        }

        public IHttpActionResult GetNote(int id)
        {
            var note = _broker.ReadObjects().FirstOrDefault(p => (int)p["_id"] == id);
            if (note == null)
            {
                NotFound();
            }
            return Ok(note);
        }

        [HttpPut]
        public IHttpActionResult PutNote(JObject note)
        {
            _broker.UpdateObject(note);
            return Ok();
        }

        [HttpDelete]
        public IHttpActionResult DeleteNote(int id)
        {
            _broker.DeleteObject(id);
            return Ok();
        }

    }
}
