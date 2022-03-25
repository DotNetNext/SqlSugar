using System;
using System.Collections.Generic;

namespace SqlSugar
{
    public interface ICacheService
    {
        void Add<V>(string key, V value);
        void Add<V>(string key, V value, int cacheDurationInSeconds);
        bool ContainsKey<V>(string key);
        V Get<V>(string key);
        IEnumerable<string> GetAllKey<V>();
        void Remove<V>(string key);
        V GetOrCreate<V>(string cacheKey, Func<V> create,int cacheDurationInSeconds=int.MaxValue);
    }
}