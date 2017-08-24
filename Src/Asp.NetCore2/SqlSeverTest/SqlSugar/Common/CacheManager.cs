using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Linq.Expressions;
namespace SqlSugar
{
    public class CacheManager<V> : ICacheManager<V>
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
                    {
                        _instance = new CacheManager<V>();
                        Action addItem =()=> { CacheManager<V>.GetInstance().RemoveAllCache(); };
                        CacheManager.Add(addItem);
                    }
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

        public void RemoveAllCache()
        {
            foreach (var key in GetAllKey())
            {
                this.Remove(key);
            }
        }

        public IEnumerable<string> GetAllKey()
        {
            return this.InstanceCache.Keys;
        }

        public void Action(string cacheKey, Action<ICacheManager<V>, string> successAction, Func<ICacheManager<V>, string, V> errorAction)
        {
            if (this.ContainsKey(cacheKey)) successAction(this, cacheKey);
            else
            {
                this.Add(cacheKey, errorAction(this, cacheKey));
            }
        }
        public V Func(string cacheKey, Func<ICacheManager<V>, string, V> successAction, Func<ICacheManager<V>, string, V> errorAction)
        {
            var cm = CacheManager<V>.GetInstance();
            if (cm.ContainsKey(cacheKey)) return successAction(cm, cacheKey);
            else
            {
                var reval = errorAction(cm, cacheKey);
                cm.Add(cacheKey, reval);
                return reval;
            }
        }
    }
    public static class CacheManager
    {
        private static List<Action> removeActions = new List<Action>();
        internal static void Add(Action removeAction)
        {
            removeActions.Add(removeAction);
        }

        public static void RemoveAllCache()
        {
            lock (removeActions)
            {
                foreach (var item in removeActions)
                {
                    item();
                }
            }
        }
    }
}
