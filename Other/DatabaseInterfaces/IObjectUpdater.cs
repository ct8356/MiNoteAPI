using Newtonsoft.Json.Linq;

namespace DatabaseInterfaces
{
    public interface IObjectUpdater
    {
        void UpdateObject(JObject jObject);
    }
}
