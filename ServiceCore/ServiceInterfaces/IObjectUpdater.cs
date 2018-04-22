using Newtonsoft.Json.Linq;

namespace ServiceInterfaces
{
    public interface IObjectUpdater
    {
        void UpdateObject(JObject jObject);
    }
}
