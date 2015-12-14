using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data;
using System.Data.SqlTypes;

namespace SqlSugar
{
    /// <summary>
    /// IEnumerable扩展函数
    /// </summary>
    public static class IEnumerableExtensions
    {

        static Type _guidType = typeof(Guid);
        /// <summary>
        /// 排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="sortField"></param>
        /// <param name="orderByType"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<T> OrderBy<T>(this IEnumerable<T> list, string sortField, OrderByType orderByType)
        {
            var type = typeof(T);
            PropertyInfo prop = type.GetProperty(sortField);
            Check.Exception(prop == null, "No property '" + sortField + "' in + " + typeof(T).Name + "'");
            if (orderByType == OrderByType.desc)
                return list.OrderByDescending(it =>ConvertField(prop.GetValue(it, null)));
            else
                return list.OrderBy(it => ConvertField(prop.GetValue(it, null)));


        }
        /// <summary>
        /// 排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="sortField"></param>
        /// <param name="orderByType"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<T> ThenBy<T>(this IOrderedEnumerable<T> list, string sortField, OrderByType orderByType)
        {
            var type = typeof(T);
            PropertyInfo prop = type.GetProperty(sortField);
            Check.Exception(prop == null, "No property '" + sortField + "' in + " + typeof(T).Name + "'");
            if (orderByType == OrderByType.desc)
                return list.ThenByDescending(it => ConvertField(prop.GetValue(it, null)));
            else
                return list.ThenBy(it => ConvertField(prop.GetValue(it, null)));

        }


        /// <summary>
        /// 排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="sortField"></param>
        /// <param name="orderByType"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<T> OrderByDataRow<T>(this IEnumerable<T> list, string sortField, OrderByType orderByType) where T : DataRow
        {
            var type = typeof(T);
            PropertyInfo prop = type.GetProperty(sortField);
            if (orderByType == OrderByType.desc)
                return list.OrderByDescending(it => ConvertField(it[sortField]));
            else
                return list.OrderBy(it => ConvertField(it[sortField]));

        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="sortField"></param>
        /// <param name="orderByType"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<T> ThenByDataRow<T>(this IOrderedEnumerable<T> list, string sortField, OrderByType orderByType) where T : DataRow
        {
            var type = typeof(T);
            PropertyInfo prop = type.GetProperty(sortField);
            if (orderByType == OrderByType.desc)
                return list.ThenByDescending(it => ConvertField(it[sortField]));
            else
                return list.ThenBy(it => ConvertField(it[sortField]));
        }

        /// <summary>
        /// 解决GUID在SQL和C#中，排序方式不一致
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        private static object ConvertField(object thisValue)
        {
            if (thisValue == null) return null;
            var isGuid = thisValue.GetType() == _guidType;
            if (isGuid)
            {
                return SqlGuid.Parse(thisValue.ToString());
            }
            else
            {
                return thisValue;
            }
        }
    }
}
