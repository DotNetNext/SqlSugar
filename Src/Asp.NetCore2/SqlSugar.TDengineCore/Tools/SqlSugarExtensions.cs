using SqlSugar.TDengine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SqlSugar 
{
     public static class SqlSugarExtensions
    {
        public static void MappingSTableName<T>(this ISqlSugarClient db,string newSTableName)
        {
            STableAttribute sTableAttribute = typeof(T).GetCustomAttribute<STableAttribute>();
            if (db.TempItems == null) 
            {
                db.TempItems = new Dictionary<string, object>();
            }
            if (sTableAttribute != null)
            {
                var key = "GetCommonSTableAttribute_" + sTableAttribute.STableName;
                if (db.TempItems.ContainsKey(key)) 
                {
                    db.TempItems.Remove(key);
                }
                db.TempItems.Add(key, newSTableName);
            } 
        }
    }
}
