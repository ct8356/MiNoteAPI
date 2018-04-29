using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace DatabaseInterfaces
{
    public interface IObjectReader
    {
        ICollection<JObject> ReadObjects();
    }
}
