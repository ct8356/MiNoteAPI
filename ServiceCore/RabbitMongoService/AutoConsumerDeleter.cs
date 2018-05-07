using DatabaseInterfaces;
using Newtonsoft.Json.Linq;
using ServiceInterfaces;
using System;

namespace RabbitMongoService
{
    public class AutoConsumerDeleter : IAutoConsumerDeleter
    {
        IAutoMessageConsumer AutoConsumer;
        IEntryDeleter EntryDeleter;
        public event EventHandler EntryDeleted;

        public AutoConsumerDeleter(
            IAutoMessageConsumer autoConsumer, 
            IEntryDeleter entryDeleter)
        {  
            autoConsumer.MessageReceived += OnMessageReceived;
            AutoConsumer = autoConsumer;
            EntryDeleter = entryDeleter;
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
            int id = Int32.Parse(message);
            EntryDeleter.DeleteObject(id);
            EntryDeleted?.Invoke(this, new EventArgs());
        }

    }
}