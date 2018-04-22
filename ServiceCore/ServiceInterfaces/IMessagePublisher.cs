using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceInterfaces
{
    public interface IMessagePublisher
    {
        void PublishMessage(string routingKey, string message);
    }
}
