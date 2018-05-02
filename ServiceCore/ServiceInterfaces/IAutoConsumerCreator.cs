using System;

namespace ServiceInterfaces
{
    public interface IAutoConsumerCreator
    {
        event EventHandler EntryCreated;
        void Start();
        void Stop();
    }
}
