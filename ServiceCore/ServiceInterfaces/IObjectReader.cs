using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace ServiceInterfaces
{
    public interface IObjectReader
    {
        ICollection<dynamic> ReadObjects();
    }
}
