using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Linq.Expressions;

namespace SqlSugar
{
    internal class CacheManager<V>
    {
        readonly System.Collections.Concurrent.ConcurrentDictionary<string, V> InstanceCache = new System.Collections.Concurrent.ConcurrentDictionary<string, V>();
        private static CacheManager<V> _instance = null;
        private static readonly object _instanceLock = new object();
        private CacheManager() { }

        public V this[string key]
        {
            get
            {
                return this.Get(key);
            }
        }

        public bool ContainsKey(string key)
        {
            return this.InstanceCache.ContainsKey(key);
        }
   
        public V Get(string key)
        {
            if (this.ContainsKey(key))
                return this.InstanceCache[key];
            else
                return default(V);
        }
    
        public static CacheManager<V> GetInstance()
        {
            if (_instance == null)
                lock (_instanceLock)
                    if (_instance == null)
                        _instance = new CacheManager<V>();
            return _instance;
        }
     
        public void Add(string key, V value)
        {
            this.InstanceCache.GetOrAdd(key, value);
        }
 
        public void Add(string key, V value, int cacheDurationInSeconds)
        {
            Add(key, value);
        }

        public void Remove(string key)
        {
            V val;
            this.InstanceCache.TryRemove(key, out val);
        }

        public IEnumerable<string> GetAllKey()
        {
            return this.InstanceCache.Keys;
        }

    }

    internal class CacheFactory {
        public static void Action<T>(string cacheKey, Action<CacheManager<T>, string> successAction, Func<CacheManager<T>, string, T> errorAction)
        {
            var cm = CacheManager<T>.GetInstance();
            if (cm.ContainsKey(cacheKey)) successAction(cm, cacheKey);
            else
            {
                cm.Add(cacheKey, errorAction(cm, cacheKey));
            }
        }
        public static T Func<T>(string cacheKey, Func<CacheManager<T>, string, T> successAction, Func<CacheManager<T>, string, T> errorAction)
        {
            var cm = CacheManager<T>.GetInstance();
            if (cm.ContainsKey(cacheKey)) return successAction(cm, cacheKey);
            else
            {
                var reval = errorAction(cm, cacheKey);
                cm.Add(cacheKey, reval);
                return reval;
            }
        }
    }
}
