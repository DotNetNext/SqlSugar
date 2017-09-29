using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    internal class CacheSchemeMain
    {

        public static T GetOrCreate<T>(ICacheService cacheService, SqlSugarClient context, QueryBuilder queryBuilder,Func<T> getData)
        {
            string key = CacheKeyBuider.GetKey(context,queryBuilder).ToString();
            var result= cacheService.GetOrCreate(key, () => getData());
            return result;
        }
    }
}
