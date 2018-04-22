using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceInterfaces
{
    public interface IObjectBroker
    {
        //wrap MessagePublisher and MessageConsumer in a broker,
        //that way, could just use straight connection to Mongo, 
        //without Rabbit, if you want to.
        void CreateObject(JObject jObject);
        ICollection<dynamic> ReadObjects();
        void UpdateObject(JObject jObject);
        void DeleteObject(int id);
        void Initialize(string databaseName);
    }
}
