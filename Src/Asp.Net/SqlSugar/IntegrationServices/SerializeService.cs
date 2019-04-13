using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class SerializeService : ISerializeService
    {
        public string SerializeObject(object value)
        {
            return JsonConvert.SerializeObject(value, new JsonSerializerSettings()
            {
                ContractResolver = new MyContractResolver()
            });
        }

        public T DeserializeObject<T>(string value)
        {
            var jSetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ContractResolver = new MyContractResolver() };
            return JsonConvert.DeserializeObject<T>(value, jSetting);
        }
    }
    public class MyContractResolver : Newtonsoft.Json.Serialization.DefaultContractResolver
    {
 

        public MyContractResolver()
        {

        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var list = type.GetProperties()
                        .Where(x => !x.GetCustomAttributes(true).Any(a => (a is SugarColumn) && ((SugarColumn)a).NoSerialize == true))
                        .Select(p => new JsonProperty()
                        {
                            PropertyName = p.Name,
                            PropertyType = p.PropertyType,
                            Readable = true,
                            Writable = true,
                            ValueProvider = base.CreateMemberValueProvider(p)
                        }).ToList();

            return list;
        }
    }
}
