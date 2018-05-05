using DatabaseInterfaces;
using Newtonsoft.Json.Linq;
using ServiceInterfaces;

namespace RabbitCore
{
    public class RabbitCreator : IObjectCreator
    {
        IMessagePublisher _messagePublisher;

        public RabbitCreator(IMessagePublisher messagePublisher)
        {
            _messagePublisher = messagePublisher;
        }
        //TODO: If this is TRULY a Rabbit Creator, then it should be dependent on RabbitPublisher!
        //I.e. may as well construct its own RabbitPublisher!
        //If it is a generic ObjectCreator, then ok.
        //But then, IT should specify the QueueName. Ah, wait, it does.
        //In constructor though. Ok better.
        /* BUT Most importantly,
         * only want to specify one of these (creator or publisher), in the container.
         * Could make a JObjectPublisher OR a IPublisher and Publisher<T>!
         * In container, just want to specify RabbitPublisher or JObjectPublisher really.
         * 
         * I think, in constructor, should only have services!
         * i.e. things that are same, in one application, for all of those interfaces.
         * so strings, should not be in constructors.
         * they should be in methods!
         * Maybe, should raise state, like do in React?
         * So, only ObjectPublisher, has properties? has strings passed in as constructor?
         * BUT then it would need to set properties of messagePublisher. 
         * FINE! can do this in MANY ways via interfaces!
         * OR could do it via methods of Publisher.
         * The wrapper of a publisher, will always have these properties to hand,
         * SO it is NO BIG DEAL to put this info, into the method call!
         * THE WRAPPER is there, to make the method signature, SIMPLER!
         * if the base component, The MessagePublisher, already has a very simple method sig,
         * then can just use it out of the box!
         * If its method sig does not take a JObject however, then may need a wrapper.
         * And to keep the container SIMPLE, so that can swap a MessagePublisher, for a JObjectPublisher easily,
         * DONT want to have to register MessagePublisher, THEN JObject Publisher! As JObject just a wrapper!
         * SO could either make MessagePublisher have BLANK constructor.
         * OR could construct MessagePublisher within JObjectPublisher. (but essentially same as registering it once).
         * 
         * 
         * Ah!
         * Maybe just need RabbitPublisher,
         * And this creator is just a RabbitPublisher that is setup in the container!
         * Yes basically! TRY IT!
         * EXCEPT, DO need this CreateObject method to convert jObject to String!
         */

        public void CreateObject(JObject jObject)
        {
            var message = jObject.ToString();
            _messagePublisher.PublishMessage("CreateNote", message);
        }

    }
}