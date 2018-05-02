using System;

namespace ServiceInterfaces
{
    public interface IAutoConsumerReader
    {
        event EventHandler ResultsPublished;
        void Start();
        void Stop();
    }
}
