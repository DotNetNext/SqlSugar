using System;
using System.Collections.Generic;

namespace SqlSugar
{
    public interface ICacheService<V>
    {
        V this[string key] { get; }
        void Add(string key, V value);
        void Add(string key, V value, int cacheDurationInSeconds);
        bool ContainsKey(string key);
        V Get(string key);
        IEnumerable<string> GetAllKey();
        void Remove(string key);
        V GetOrCreate(string cacheKey, Func<ICacheService<V>, string, V> successAction, Func<ICacheService<V>, string, V> errorAction);
    }
}