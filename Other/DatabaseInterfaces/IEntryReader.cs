using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace DatabaseInterfaces
{
    public interface IEntryReader
    {
        ICollection<JObject> ReadObjects();
    }
}
