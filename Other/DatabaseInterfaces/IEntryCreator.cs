using Newtonsoft.Json.Linq;

namespace DatabaseInterfaces
{
    public interface IEntryCreator
    {
        void CreateObject(JObject jObject);
    }
}
