using Newtonsoft.Json.Linq;

namespace ServiceInterfaces
{
    public interface IObjectCreator
    {
        void CreateObject(JObject jObject);
    }
}
