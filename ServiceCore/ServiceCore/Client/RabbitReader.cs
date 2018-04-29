using DatabaseInterfaces;
using Newtonsoft.Json.Linq;
using ServiceInterfaces;
using System.Collections.Generic;

namespace RabbitCore
{
    public class RabbitReader : IObjectReader
    {
        IMessageConsumer _messageConsumer;
        IMessagePublisher _messagePublisher;

        public RabbitReader(IMessageConsumer messageConsumer, IMessagePublisher messagePublisher)
        {
            _messageConsumer = messageConsumer;
            _messageConsumer.HostName = "localHost";
            _messageConsumer.QueueName = ""; //TODO need to pass this. Specific to client.
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
            var jObjects = JObject.Parse(receivedMessage);
            var objects = new List<JObject>();
            for (int i = 0; i < jObjects.Count; i++)
            {
                objects.Add(jObjects[i] as JObject);
            }
            return objects;
        }

    }
}