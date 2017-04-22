using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace SqlSugar
{
    public class RewritableMethods : IRewritableMethods
    {
        
        /// <summary>
        ///DataReader to Dynamic
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public ExpandoObject DataReaderToExpandoObject(IDataReader reader)
        {
            ExpandoObject result = new ExpandoObject();
            var dic = ((IDictionary<string, object>)result);
            for (int i = 0; i < reader.FieldCount; i++)
            {
                try
                {
                    dic.Add(reader.GetName(i), reader.GetValue(i));
                }
                catch
                {
                    dic.Add(reader.GetName(i), null);
                }
            }
            return result;
        }

        public List<T> DataReaderToDynamicList<T>(IDataReader reader)
        {
            var tType = typeof(T);
            var classProperties = tType.GetProperties().Where(it => it.PropertyType.IsClass()).ToList();
            var reval = new List<T>();
            if (reader != null && !reader.IsClosed)
            {
                while (reader.Read())
                {
                    var expandoObject = DataReaderToExpandoObject(reader);
                    var dic = (IDictionary<string,object>)expandoObject;
                    foreach (var item in classProperties)
                    {
                        var startsWithName = item.Name + "_";
                        List<string> removeKeys = new List<string>();
                        foreach (var d in dic)
                        {
                            if (d.Key.StartsWith(startsWithName)) {
                                removeKeys.Add(d.Key);
                            }
                        }
                        if (removeKeys.Any()) {
                            var keyValues = removeKeys.Select(it => new KeyValuePair<string, object>(it.Replace(startsWithName, null), dic[it])).ToList();
                            foreach (var key in removeKeys)
                            {
                                dic.Remove(key);
                            }
                            if (!item.PropertyType.IsAnonymousType())
                            {
                                var obj = Activator.CreateInstance(item.PropertyType, true);
                                var ps = obj.GetType().GetProperties();
                                foreach (var keyValue in keyValues)
                                {
                                    ps.Single(it => it.Name == keyValue.Key).SetValue(obj, keyValue.Value);
                                }
                                dic.Add(item.Name, obj);
                            }
                        }
                    }
                    var stringValue = SerializeObject(expandoObject);
                    reval.Add((T)DeserializeObject<T>(stringValue));
                }
                reader.Close();
            }
            return reval;
        }

        /// <summary>
        /// Serialize Object
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string SerializeObject(object value)
        {
            return JsonConvert.SerializeObject(value);
        }

        /// <summary>
        /// Serialize Object
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public T DeserializeObject<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }
    }
}
