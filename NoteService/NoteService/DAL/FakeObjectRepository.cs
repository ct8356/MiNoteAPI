using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json.Linq;
using ServiceInterfaces;

namespace NoteService
{

    public class FakeObjectRepository : IObjectBroker
    {
        public static ICollection<dynamic> Objects { get; set; }

        public FakeObjectRepository()
        {
            Initialize("test");
        }

        public void Initialize(string dbName)
        {
            Objects = new List<dynamic>();
            //Seed
            SeedWithNotes();
        }

        public void CreateObject(JObject @object)
        {
            var maxId = Objects.Max(o => o.Id);
            var newId = maxId + 1;
            @object["_id"] = newId;
            Objects.Add(@object);
            //POINTLESS,
            //because this repo gets instantiated with every refresh,
            //and is no way around it!
            //got to use persistence i.e. database!
        }

        public ICollection<dynamic> ReadObjects()
        {
            return Objects;
        }

        public void UpdateObject(JObject updatedObject)
        {
            Objects.Remove(Objects.Where(o => o.Id == updatedObject["_id"]));
            Objects.Add(updatedObject);
        }

        public void DeleteObject(int id)
        {
            Objects.Remove(Objects.Where(o => o.Id == id));
        }
 
        public void DeleteEverything()
        {
            //do nothing
        }

        private void SeedWithNotes()
        {
            var notes = new List<Note>() {
                new Note() {
                    Id = 1,
                    Content = "Hello",
                },
                new Note() {
                    Id = 2,
                    Content = "World",
                },
            };
            foreach (var note in notes)
            {
                Objects.Add(note);
            }
        }

    }
}