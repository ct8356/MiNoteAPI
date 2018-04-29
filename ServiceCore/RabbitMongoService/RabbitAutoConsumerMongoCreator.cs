﻿using DatabaseInterfaces;
using Newtonsoft.Json.Linq;
using ServiceInterfaces;
using System;

namespace RabbitMongoService
{
    public class RabbitAutoConsumerMongoCreator : IAutoMessageConsumerObjectCreator
    {
        IAutoMessageConsumer _autoConsumer;
        IObjectCreator _objectCreator;
        public event EventHandler EntryCreated;

        public RabbitAutoConsumerMongoCreator
            (IAutoMessageConsumer autoConsumer, IObjectCreator objectCreator)
        {  
            autoConsumer.HostName = "localhost";
            autoConsumer.QueueName = "CreateNote";
            autoConsumer.MessageReceived += OnMessageReceived;    
            _autoConsumer = autoConsumer;
            _objectCreator = objectCreator;
        }

        public void Start()
        {
            _autoConsumer.Start();
        }

        public void Stop()
        {
            _autoConsumer.Stop();
        }

        private void OnMessageReceived(object sender, IMessageEventArgs e)
        {
            var message = e.Message;
            var jObject = JObject.Parse(message);
            _objectCreator.CreateObject(jObject);
            EntryCreated?.Invoke(this, new EventArgs());
        }

    }
}