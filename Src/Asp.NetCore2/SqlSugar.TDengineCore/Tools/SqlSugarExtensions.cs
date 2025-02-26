using SqlSugar.TDengine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace SqlSugar 
{
     public static class SqlSugarExtensions
     {
        public static ISugarQueryable<T> AsTDengineSTable<T>(this ISugarQueryable<T> queryable)   where T:class,new()
        {
            var attr=SqlSugar.TDengine.UtilMethods.GetCommonSTableAttribute(queryable.Context,typeof(T).GetCustomAttribute<STableAttribute>());
            queryable.AS(attr.STableName);
            return queryable;
        }
        public static IDeleteable<T> AsTDengineSTable<T>(this IDeleteable<T> queryable) where T : class, new()
        {
            var attr = SqlSugar.TDengine.UtilMethods.GetCommonSTableAttribute(((DeleteableProvider<T>)queryable).Context, typeof(T).GetCustomAttribute<STableAttribute>());
            queryable.AS(attr.STableName);
            return queryable;
        }
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
