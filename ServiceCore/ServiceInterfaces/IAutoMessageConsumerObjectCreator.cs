using System;

namespace ServiceInterfaces
{
    public interface IAutoMessageConsumerObjectCreator
    {
        event EventHandler EntryCreated;
        void Start();
        void Stop();
    }
}
