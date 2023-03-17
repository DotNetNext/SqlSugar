using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class SugarCacheProvider
    {
        public ICacheService Servie { get;  set; }

        public void RemoveDataCache(string likeString) 
        {
            if (Servie == null) return;
            CacheSchemeMain.RemoveCacheByLike(Servie, likeString);
        }

        public List<string> GetAllKey()
        {
            if (Servie == null) return new List<string>();
            return Servie.GetAllKey<string>()?.ToList();
        }

        public void Add(string key,object value)
        {
            if (Servie == null)  return;
            Servie.Add(key,value,60*60*24*100);
        }

        public void Add(string key, object value,int seconds)
        {
            if (Servie == null) return;
            Servie.Add(key, value,seconds);
        }

        public T Get<T>(string key)
        {
            if (Servie == null) return default(T);
            return Servie.Get<T>(key);
        }
    }
}
