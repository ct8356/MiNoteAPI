namespace ServiceInterfaces
{
    public interface IAutoMessageConsumer
    {
        //ConsumeMessage should be private.
        string HostName { get; set; }
        string QueueName { get; set; }
        event MessageEventHandler MessageReceived;
        void Start();
        void Stop();
    }
}
