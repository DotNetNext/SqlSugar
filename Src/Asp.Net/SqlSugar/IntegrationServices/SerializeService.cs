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
            if (value != null && value.GetType().FullName.StartsWith("System.Text.Json.")) 
            {
                // 动态创建一个 JsonSerializer 实例
                Type serializerType = value.GetType().Assembly.GetType("System.Text.Json.JsonSerializer");

                var methods =  serializerType
                    .GetMyMethod("Serialize", 2);

                // 调用 SerializeObject 方法序列化对象
                string json = (string)methods.MakeGenericMethod(value.GetType())
                    .Invoke(null, new object[] { value,null });
                return json;
            }
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
            if (typeof(T).FullName.StartsWith("System.Text.Json."))
            {
                // 动态创建一个 JsonSerializer 实例
                Type serializerType =typeof(T).Assembly.GetType("System.Text.Json.JsonSerializer");

                var methods = serializerType
                    .GetMethods().Where(it=>it.Name== "Deserialize")
                    .Where(it=>it.GetParameters().Any(z=>z.ParameterType==typeof(string))).First();

                // 调用 SerializeObject 方法序列化对象
                T json = (T)methods.MakeGenericMethod(typeof(T))
                    .Invoke(null, new object[] { value, null });
                return json;
            }
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
