using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;

namespace NoteService
{
    public class Note
    {    
        public int Id { get; set; }
        public string Content { get; set; }
    }
}