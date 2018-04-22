using Newtonsoft.Json.Linq;
using ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServiceCore
{
    public class RabbitMqObjectBroker : IObjectBroker
    {
        //NOTE! It would actually be better, to have a separate microservice,
        //for each crud operation!
        //I.e. a NoteCreationService, a NoteReadingService, a NoteUpdatingService,
        //a NoteDeletionService.
        //OR just NoteCreator, NoteReader, NoteUpdater, NoteDeleter
        IMessageConsumer _messageConsumer;
        IMessagePublisher _messagePublisher;

        public RabbitMqObjectBroker(IMessageConsumer messageConsumer, IMessagePublisher messagePublisher)
        {
            _messageConsumer = messageConsumer;
            _messagePublisher = messagePublisher;
        }

        public void Initialize(string databaseName)
        {
            // Do nothing
        }

        public void CreateObject(JObject jObject)
        {
            var message = jObject.ToString();
            _messagePublisher.PublishMessage("CreateNote", message);
        }

        public ICollection<dynamic> ReadObjects()
        {
            //SEND
            var message = "Some message";
            _messagePublisher.PublishMessage("ReadNotes", message);
            //RECEIVE (need to wait somehow, something like fetch from React?
            //or just keep consuming always (yes), then respond to event?
            //fetch is happy to wait.
            //BUT, would still need to pass it back, FROM this method!
            //That is what fetch is expecting! I think that requires Tasks?
            //ACTUALLY, this method is HAPPY to wait!
            //I think, I don't know. Only happy to wait if NOTEAPI exists everywhere,
            //BUT if one gateway, DOES it exist everywhere? I guess it could, and maybe should?
            //OK lets assume this method is happy to wait.
            //THEN, I just consume Message, AFTER I have got an ACK message from NoteService?
            //BUT at that point, its only received message, has not GOT notes from db yet.
            //MAYBE only send the ACK once got Notes from db AND sent to queue, so its not too chatty?
            //It just says, OK API, you can consume now?)
            //OR just maybe if I am lucky, just consume straight away,
            //and it is intelligent enough to wait?
            //BUT what if lots of requests? How does it know THAT is its message?
            //Perhaps it needs a guid?
            //OR perhaps, just each user, gets their own queue?
            //Yes that is possible... Makes sense in fact. Yes I think so...
            var receivedMessage = _messageConsumer.ConsumeMessage();
            var jObjects = JObject.Parse(receivedMessage);
            var objects = new List<dynamic>();
            for (int i = 0; i < jObjects.Count; i++)
            {
                objects.Add(jObjects[i]);
            }
            return objects;
        }

        public void UpdateObject(JObject jObject)
        {
            var message = jObject.ToString();
            _messagePublisher.PublishMessage("UpdateNote", message);
        }

        public void DeleteObject(int id)
        {
            var message = id.ToString();
            _messagePublisher.PublishMessage("DeleteNote", message);
        }

    }
}