using DatabaseInterfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServiceInterfaces;
using System.Collections.Generic;

namespace RabbitCore
{
    public class EntryReader : IEntryReader
    {
        IMessageConsumer _messageConsumer;
        IMessagePublisher _messagePublisher;

        public EntryReader(IMessageConsumer messageConsumer, IMessagePublisher messagePublisher)
        {
            _messageConsumer = messageConsumer;
            _messagePublisher = messagePublisher;
        }

        public ICollection<JObject> ReadObjects()
        {
            //SEND
            var message = "Not important";
            _messagePublisher.PublishMessage("ReadNotes", message);
            //MAYBE only send the ACK once got Notes from db AND sent to queue, so its not too chatty?
            //It just says, OK API, you can consume now?)
            //OR just maybe if I am lucky, just consume straight away,
            //and it is intelligent enough to wait? yes.
            //BUT what if lots of requests? How does it know THAT is its message?
            //Perhaps it needs a guid?
            //OR perhaps, just each user, gets their own queue?
            //Yes that is possible... Makes sense in fact. Yes I think so...
            var receivedMessage = _messageConsumer.ConsumeMessage();
            //var jObjects = JObject.Parse(receivedMessage); //Does not work, because it reads it as one object.
            var jObjects = JsonConvert.DeserializeObject<List<JObject>>(receivedMessage);
            var objects = new List<JObject>();
            for (int i = 0; i < jObjects.Count; i++)
            {
                objects.Add(jObjects[i] as JObject);
            }
            return objects;
        }

    }
}