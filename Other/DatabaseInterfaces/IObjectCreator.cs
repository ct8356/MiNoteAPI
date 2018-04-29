using Newtonsoft.Json.Linq;

namespace DatabaseInterfaces
{
    public interface IObjectCreator
    {
        void CreateObject(JObject jObject);
    }
}
