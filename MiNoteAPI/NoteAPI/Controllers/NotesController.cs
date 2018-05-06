using DatabaseInterfaces;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using ServiceInterfaces;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace NoteAPI.Controllers
{
    public class NotesController : ApiController
    {

        IEntryBroker Broker;
        IService Service;

        public NotesController(IControllerEntryBroker broker, IService service)
        {
            //Should not have to pass service here,
            //Just a hack so I can do it in one solution.
            //Also bad, because recreates service with every call to controller.
            //SO I think might end up with lots.
            //(no, surely service only lives as long as Controller does?)
            //But see if it works...

            //NOTE, this controller gets called everytime you access the page.
            //BUT on first run, it takes its sweet time!
            //It even loads the page, before calling this constructor!
            Broker = broker;
            Broker.Initialize("test");
            Service = service;
            Service.Start();
        }

        [HttpPost]
        public IHttpActionResult PostNote(JObject note)
        {
            Broker.CreateObject(note);
            return Ok();
        }

        public IEnumerable<dynamic> GetAllNotes()
        {
            //This method is recognised by view by naming convention.
            //It is part of the API.
            return Broker.ReadObjects();
        }

        public IHttpActionResult GetNote(int id)
        {
            var note = Broker.ReadObjects().FirstOrDefault(p => (int)p["_id"] == id);
            if (note == null)
            {
                NotFound();
            }
            return Ok(note);
        }

        [HttpPut]
        public IHttpActionResult PutNote(JObject note)
        {
            Broker.UpdateObject(note);
            return Ok();
        }

        [HttpDelete]
        public IHttpActionResult DeleteNote(int id)
        {
            Broker.DeleteObject(id);
            return Ok();
        }

    }
}
