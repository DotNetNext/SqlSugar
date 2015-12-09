using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data;

namespace SqlSugar
{
    /// <summary>
    /// IEnumerable扩展函数
    /// </summary>
    public static class IEnumerableExtensions
    {
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
            PropertyInfo prop = typeof(T).GetProperty(sortField);

            Check.Exception(prop == null, "No property '" + sortField + "' in + " + typeof(T).Name + "'");

            if (orderByType == OrderByType.desc)
                return list.OrderByDescending(it => prop.GetValue(it, null));
            else
                return list.OrderBy(it => prop.GetValue(it, null));

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
            PropertyInfo prop = typeof(T).GetProperty(sortField);

            Check.Exception(prop == null, "No property '" + sortField + "' in + " + typeof(T).Name + "'");

            if (orderByType == OrderByType.desc)
                return list.ThenByDescending(it => prop.GetValue(it, null));
            else
                return list.ThenBy(it => prop.GetValue(it, null));

        }


        /// <summary>
        /// 排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="sortField"></param>
        /// <param name="orderByType"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<T> OrderByDataRow<T>(this IEnumerable<T> list, string sortField, OrderByType orderByType) where T:DataRow
        {
            PropertyInfo prop = typeof(T).GetProperty(sortField);

            if (orderByType == OrderByType.desc)
                return list.OrderByDescending(it=>it[sortField]);
            else
                return list.OrderBy(it => it[sortField]);

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
            PropertyInfo prop = typeof(T).GetProperty(sortField);

            if (orderByType == OrderByType.desc)
                return list.ThenByDescending(it => it[sortField]);
            else
                return list.ThenBy(it => it[sortField]);

        }
    }
}
