using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Linq.Expressions;

namespace SqlSugar
{
    /// <summary>
    /// ** 描述：缓存操作类
    /// ** 创始时间：2015-6-9
    /// ** 修改时间：-
    /// ** 作者：sunkaixuan
    /// ** 使用说明：http://www.cnblogs.com/sunkaixuan/p/4563462.html
    /// </summary>
    /// <typeparam name="V">值类型</typeparam>
    internal class CacheManager<V> : IStorageObject<V>
    {

        readonly System.Collections.Concurrent.ConcurrentDictionary<string, V> InstanceCache = new System.Collections.Concurrent.ConcurrentDictionary<string, V>();

        #region 全局变量
        private static CacheManager<V> _instance = null;
        private static readonly object _instanceLock = new object();
        #endregion

        #region 构造函数

        private CacheManager() { }
        #endregion

        #region  属性
        /// <summary>         
        ///根据key获取value     
        /// </summary>         
        /// <value></value>      
        public override V this[string key]
        {
            get
            {
                return this.Get(key);
            }
        }
        #endregion

        #region 公共函数

        /// <summary>         
        /// 验证key是否存在       
        /// </summary>         
        /// <param name="key">key</param>         
        /// <returns> /// 	存在<c>true</c> 不存在<c>false</c>.        /// /// </returns>         
        public override bool ContainsKey(string key)
        {
            return this.InstanceCache.ContainsKey(key);

            //throw new NotImplementedException();
            //return HttpRuntime.Cache[CreateKey(key)] != null;
        }

        /// <summary>         
        /// 根据key获取value  
        /// </summary>         
        /// <param name="key">key</param>         
        /// <returns></returns>         
        public override V Get(string key)
        {
            if (this.ContainsKey(key))
                return this.InstanceCache[key];
            else
                return default(V);
        }

        /// <summary>         
        /// 获取实例 （单例模式）       
        /// </summary>         
        /// <returns></returns>         
        public static CacheManager<V> GetInstance()
        {
            if (_instance == null)
                lock (_instanceLock)
                    if (_instance == null)
                        _instance = new CacheManager<V>();
            return _instance;
        }

        /// <summary>         
        /// 插入缓存(默认20分钟)        
        /// </summary>         
        /// <param name="key"> key</param>         
        /// <param name="value">value</param>          
        public override void Add(string key, V value)
        {
            this.InstanceCache.GetOrAdd(key, value);
        }

        /// <summary>         
        /// 插入缓存        
        /// </summary>         
        /// <param name="key"> key</param>         
        /// <param name="value">value</param>         
        /// <param name="cacheDurationInSeconds">过期时间单位秒</param>         
        public void Add(string key, V value, int cacheDurationInSeconds)
        {
            Add(key, value);
        }

        ///// <summary>         
        ///// 插入缓存.         
        ///// </summary>         
        ///// <param name="key">key</param>         
        ///// <param name="value">value</param>         
        ///// <param name="cacheDurationInSeconds">过期时间单位秒</param>         
        ///// <param name="priority">缓存项属性</param>         
        //public void Add(string key, V value, int cacheDurationInSeconds, CacheItemPriority priority)
        //{
        //    string keyString = CreateKey(key);
        //    HttpRuntime.Cache.Insert(keyString, value, null, DateTime.Now.AddSeconds(cacheDurationInSeconds), Cache.NoSlidingExpiration, priority, null);
        //}

        ///// <summary>         
        ///// 插入缓存.         
        ///// </summary>         
        ///// <param name="key">key</param>         
        ///// <param name="value">value</param>         
        ///// <param name="cacheDurationInSeconds">过期时间单位秒</param>         
        ///// <param name="priority">缓存项属性</param>         
        //public void Add(string key, V value, int cacheDurationInSeconds, CacheDependency dependency, CacheItemPriority priority)
        //{
        //    //string keyString = CreateKey(key);
        //    //HttpRuntime.Cache.Insert(keyString, value, dependency, DateTime.Now.AddSeconds(cacheDurationInSeconds), Cache.NoSlidingExpiration, priority, null);
        //}

        /// <summary>         
        /// 删除缓存         
        /// </summary>         
        /// <param name="key">key</param>         
        public override void Remove(string key)
        {
            V val;
            this.InstanceCache.TryRemove(key, out val);

            //throw new NotImplementedException();
            //HttpRuntime.Cache.Remove(CreateKey(key));
        }

        /// <summary>
        /// 清除所有缓存
        /// </summary>
        public override void RemoveAll()
        {
            this.InstanceCache.Clear();

        }

        /// <summary>
        /// 清除所有缓存
        /// </summary>
        /// <param name="removeExpression">表达式条件</param>
        public override void RemoveAll(Func<string, bool> removeExpression)
        {
            //throw new NotImplementedException();
            //System.Web.Caching.Cache _cache = HttpRuntime.Cache;
            var allKeyList = GetAllKey();
            var delKeyList = allKeyList.Where(removeExpression).ToList();
            foreach (var key in delKeyList)
            {
                Remove(key);
            }
        }

        /// <summary>
        /// 获取所有缓存key
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<string> GetAllKey()
        {
            return this.InstanceCache.Keys;

            //throw new NotImplementedException();
            //IDictionaryEnumerator CacheEnum = HttpRuntime.Cache.GetEnumerator();
            //while (CacheEnum.MoveNext())
            //{
            //    yield return CacheEnum.Key.ToString();
            //}
        }
        #endregion


    }
}
