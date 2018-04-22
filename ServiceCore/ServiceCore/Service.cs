using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceInterfaces;

namespace ServiceCore
{
    public class Service : IService
    {

        RabbitConsumer _messageConsumer;
        RabbitPublisher _messagePublisher;

        public void Start()
        {

        }

        public void Stop()
        {

        }

    }
}
