using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceInterfaces
{
    public interface IMessageConsumer
    {
        string HostName { get; set; }
        string QueueName { get; set; }
        event MessageEventHandler MessageReceived;
        string ConsumeMessage();
    }
}
