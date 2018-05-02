using System;

namespace ServiceInterfaces
{
    public interface IAutoConsumerDeleter
    {
        event EventHandler EntryDeleted;
        void Start();
        void Stop();
    }
}
