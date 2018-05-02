using System;

namespace ServiceInterfaces
{
    public interface IAutoConsumerUpdater
    {
        event EventHandler EntryUpdated;
        void Start();
        void Stop();
    }
}
