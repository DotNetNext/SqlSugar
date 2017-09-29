using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    internal class CacheSchemeMain
    {

        public static T GetOrCreate<T>(ICacheService cacheService,QueryBuilder queryBuilder,Func<T> getData,int cacheDurationInSeconds, SqlSugarClient context)
        {
            string key = CacheKeyBuider.GetKey(context,queryBuilder).ToString();
            var result= cacheService.GetOrCreate(key, () => getData(), cacheDurationInSeconds);
            return result;
        }
    }
}
