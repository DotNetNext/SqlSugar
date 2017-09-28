using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class SerializeManager<T>:ISerializeManager<T>  
    {
        private static SerializeManager<T> _instance = null;
        private static readonly object _instanceLock = new object();
        public static SerializeManager<T> GetInstance() {
            if (_instance == null)
                lock (_instanceLock)
                    if (_instance == null)
                    {
                        _instance = new SerializeManager<T>();
                    }
            return _instance;
        }
        public string SerializeObject(object value)
        {
            return JsonConvert.SerializeObject(value);
        }

        public T DeserializeObject(string value)
        {
            var jSetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            return JsonConvert.DeserializeObject<T>(value, jSetting);
        }
    }
}
