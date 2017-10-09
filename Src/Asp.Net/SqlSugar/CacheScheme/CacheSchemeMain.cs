using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    internal class CacheSchemeMain
    {
        public static T GetOrCreate<T>(ICacheService cacheService, QueryBuilder queryBuilder, Func<T> getData, int cacheDurationInSeconds, SqlSugarClient context)
        {
            CacheKey key = CacheKeyBuider.GetKey(context, queryBuilder);
            string keyString = key.ToString();
            foreach (var tableName in key.Tables)
            {
                //if (!mappingInfo.Any(it=>it.Key.Equals(tableName,StringComparison.CurrentCultureIgnoreCase))) {
                //    cacheService.Add<>(new KeyValuePair<string,string>(tableName, keyString));
                //}
            }
    
            T result = default(T);
            //if (mappingKey.IsNullOrEmpty())
            //    result = getData();
            //else
            //{
            //    result = cacheService.GetOrCreate("", () => getData(), cacheDurationInSeconds);
            //}
            return result;
        }

        public static void RemoveCache(string tableName)
        {
 
        }
    }
}
