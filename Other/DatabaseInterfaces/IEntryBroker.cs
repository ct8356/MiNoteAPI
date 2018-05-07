namespace DatabaseInterfaces
{
    public interface IEntryBroker 
        : IEntryCreator
        , IEntryReader
        , IEntryUpdater
        , IEntryDeleter
    {
        //wrap MessagePublisher and MessageConsumer in a broker,
        //that way, could just use straight connection to Mongo, 
        //without Rabbit, if you want to.
        void Initialize(string databaseName);
    }
}
