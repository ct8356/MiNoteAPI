using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceInterfaces
{
    public interface IAutoMessageConsumer
    {
        //AutoConsumers:
        //have Start and Stop,
        //And ConsumeMessage should be private.
        //Should autoCall consume Message every second,
        //And if nothing found after a second, then give up and try again,
        //So as not to hang up???
        //OR just call ConsumeMessage at the start.
        //THEN as soon as ConsumeMessage Complete, (i.e. message received) 
        //then call it again.
        string HostName { get; set; }
        string QueueName { get; set; }
        event MessageEventHandler MessageReceived;
        void Start();
        void Stop();
    }
}
