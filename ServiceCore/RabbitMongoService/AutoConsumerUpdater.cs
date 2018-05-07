using DatabaseInterfaces;
using Newtonsoft.Json.Linq;
using ServiceInterfaces;
using System;

namespace RabbitMongoService
{
    public class AutoConsumerUpdater : IAutoConsumerUpdater
    {
        IAutoMessageConsumer AutoConsumer;
        IEntryUpdater EntryUpdater;
        public event EventHandler EntryUpdated;

        public AutoConsumerUpdater(
            IAutoMessageConsumer autoConsumer, 
            IEntryUpdater entryUpdater)
        {  
            autoConsumer.MessageReceived += OnMessageReceived;
            AutoConsumer = autoConsumer;
            EntryUpdater = entryUpdater;
        }

        public void Start()
        {
            AutoConsumer.Start();
        }

        public void Stop()
        {
            AutoConsumer.Stop();
        }

        private void OnMessageReceived(object sender, IMessageEventArgs e)
        {
            var message = e.Message;
            var jObject = JObject.Parse(message);
            EntryUpdater.UpdateObject(jObject);
            EntryUpdated?.Invoke(this, new EventArgs());
        }

    }
}