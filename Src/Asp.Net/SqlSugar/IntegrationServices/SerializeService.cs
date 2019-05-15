using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
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
            return JsonConvert.SerializeObject(value);
        }

        public string SugarSerializeObject(object value)
        {
            return JsonConvert.SerializeObject(value, new JsonSerializerSettings()
            {
                ContractResolver = new MyContractResolver()
            });
        }
 
        public T DeserializeObject<T>(string value)
        {
            var jSetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
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
            if (type.IsAnonymousType()||type==UtilConstants.ObjType|| type.Namespace=="SqlSugar"|| type.IsClass()==false)
            {
                return base.CreateProperties(type, memberSerialization);
            }
            else
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
                foreach (var item in list)
                {
                    if (UtilMethods.GetUnderType(item.PropertyType) == UtilConstants.DateType)
                    {
                        CreateDateProperty(type, item);
                    }
                }
                return list;
            }
        }

        private static void CreateDateProperty(Type type, JsonProperty item)
        {
            var property = type.GetProperties().Where(it => it.Name == item.PropertyName).First();
            var itemType = UtilMethods.GetUnderType(property);
            if (property.GetCustomAttributes(true).Any(it => it is SugarColumn))
            {
                var sugarAttribute = (SugarColumn)property.GetCustomAttributes(true).First(it => it is SugarColumn);
                item.Converter = new IsoDateTimeConverter() { DateTimeFormat = sugarAttribute.SerializeDateTimeFormat };
            }
        }
    }

}
