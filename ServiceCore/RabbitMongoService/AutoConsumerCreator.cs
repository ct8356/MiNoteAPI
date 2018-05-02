using DatabaseInterfaces;
using Newtonsoft.Json.Linq;
using ServiceInterfaces;
using System;

namespace RabbitMongoService
{
    public class AutoConsumerCreator : IAutoConsumerCreator
    {
        IAutoMessageConsumer _autoConsumer;
        IObjectCreator _objectCreator;
        public event EventHandler EntryCreated;

        public AutoConsumerCreator(
            IAutoMessageConsumer autoConsumer, 
            IObjectCreator objectCreator)
        {  
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