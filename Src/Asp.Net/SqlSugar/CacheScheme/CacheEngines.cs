using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    internal class CacheEngines
    {
        static string CacheMappingKey = "SqlSugarDataCacheMapping";
        public static string GetCacheMapping(string tableName)
        {
            return CacheMappingKey+ UtilConstants.Dot+tableName+ UtilConstants.Dot + Guid.NewGuid();
        }
    }
}
