﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RabbitCore;
using RabbitMongoService;
using System;
using System.Collections.Generic;
using System.Threading;

namespace IntegrationTests
{
    [TestFixture]
    public class AutoConsumerReaderTests : RabbitMongoTestBase
    {

        MessagePublisher Publisher;
        AutoConsumerReader ConsumerReader;
        MessageConsumer Consumer;

        AutoResetEvent AutoResetEvent;

        [OneTimeSetUp]
        public new void OneTimeSetUp()
        {
            var databaseName = "Test";
            var collectionName = "Objects";
            var hostName = "localHost";
            var exchangeName = "";
            var requestQueueName = "ReadNotes";
            var responseQueueName = "NoteAPI";
            Reader = new MongoClient.EntryReader(databaseName, collectionName);
            Creator = new MongoClient.EntryCreator(Reader, databaseName, collectionName);
            Broker = new MongoClient.EntryBroker(Creator, Reader, null, null);
            Broker.Initialize(databaseName);
            Broker.DeleteEverything();

            Publisher = new MessagePublisher(hostName, exchangeName, requestQueueName);

            var autoConsumer = new AutoMessageConsumer(hostName, requestQueueName);
            var objectReader = new MongoClient.EntryReader(databaseName, collectionName);      
            var messagePublisher = new MessagePublisher(hostName, exchangeName, responseQueueName);
            ConsumerReader = new AutoConsumerReader(autoConsumer, objectReader, messagePublisher);

            Consumer = new MessageConsumer(hostName, responseQueueName);

            // Subscribe
            ConsumerReader.ResultsPublished += ConsumerReader_ResultsPublished;
            AutoResetEvent = new AutoResetEvent(false);
        }

        [Test]
        public void OnMessageReceived_WithEntriesInDb_ReadsEntriesAndPublishesMessage()
        {
            dynamic @object = new
            {
                _id = 1,
                Content = "new object"
            };
            JObject jObject = JObject.FromObject(@object);
            Broker.CreateObject(jObject);
            ConsumerReader.Start();

            Publisher.PublishMessage("Not important");
            AutoResetEvent.WaitOne(); //Wait until Results published.

            var receivedMessage = Consumer.ConsumeMessage();
            var jObjects = JsonConvert.DeserializeObject<List<JObject>>(receivedMessage);
            var actualJObject = jObjects[0];
            Assert.AreEqual("new object", (string)actualJObject["Content"]);
            ConsumerReader.Stop();
        }

        private void ConsumerReader_ResultsPublished(object sender, EventArgs e)
        {
            AutoResetEvent.Set();
        }

    }
}
