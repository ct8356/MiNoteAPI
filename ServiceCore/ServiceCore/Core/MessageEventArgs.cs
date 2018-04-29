using ServiceInterfaces;
using System;

namespace RabbitCore
{
    public class MessageEventArgs : EventArgs, IMessageEventArgs
    {

        public string Message { get; private set; }

        public MessageEventArgs(string message)
        {
            Message = message;
        }

    }
}
