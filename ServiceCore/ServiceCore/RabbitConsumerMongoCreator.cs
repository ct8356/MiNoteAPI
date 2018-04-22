using Newtonsoft.Json.Linq;
using ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServiceCore
{
    public class RabbitConsumerMongoCreator : IMessageConsumerObjectCreator
    {
        IMessageConsumer _messageConsumer;
        IObjectCreator _objectCreator;

        public RabbitConsumerMongoCreator
            (IMessageConsumer messageConsumer, IObjectCreator objectCreator)
        {  
            messageConsumer.HostName = "localhost";
            messageConsumer.QueueName = "CreateNote";
            messageConsumer.MessageReceived += OnMessageReceived;
            _messageConsumer = messageConsumer;
            _objectCreator = objectCreator;
        }

        public void OnMessageReceived(object sender, IMessageEventArgs e)
        {
            var message = e.Message;
            var jObject = JObject.Parse(message);
            _objectCreator.CreateObject(jObject);
        }

    }
}