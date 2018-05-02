using DatabaseInterfaces;
using Newtonsoft.Json;
using ServiceInterfaces;
using System;

namespace RabbitMongoService
{
    public class AutoConsumerReader : IAutoConsumerReader
    {
        IAutoMessageConsumer _autoConsumer;
        IObjectReader _objectReader;
        IMessagePublisher _messagePublisher;
        public event EventHandler ResultsPublished;

        public AutoConsumerReader(
            IAutoMessageConsumer autoConsumer,
            IObjectReader objectReader, 
            IMessagePublisher messagePublisher)
        {
            autoConsumer.MessageReceived += OnMessageReceived;
            _autoConsumer = autoConsumer;
            _objectReader = objectReader;
            _messagePublisher = messagePublisher;          
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
            ReadObjectsAndPublishMessage();
        }

        private void ReadObjectsAndPublishMessage()
        {
            var objects = _objectReader.ReadObjects();
            var message = JsonConvert.SerializeObject(objects, Formatting.Indented);
            _messagePublisher.PublishMessage(message);
            ResultsPublished?.Invoke(this, new EventArgs());
        }

    }
}