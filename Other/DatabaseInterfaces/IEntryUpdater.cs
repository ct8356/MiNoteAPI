using Newtonsoft.Json.Linq;

namespace DatabaseInterfaces
{
    public interface IEntryUpdater
    {
        void UpdateObject(JObject jObject);
    }
}
