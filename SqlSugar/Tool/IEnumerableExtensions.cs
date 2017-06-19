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
    /// ** 描述：IEnumerable扩展类
    /// ** 创始时间：2015-6-9
    /// ** 修改时间：-
    /// ** 作者：sunkaixuan
    /// ** 使用说明：
    /// </summary>
    public static class IEnumerableExtensions
    {

        static Type _guidType = typeof(Guid);

        #region 单组
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
            if (orderByType == OrderByType.Desc)
                return list.OrderByDescending(it => ConvertField(prop.GetValue(it, null)));
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
            if (orderByType == OrderByType.Desc)
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
            if (orderByType == OrderByType.Desc)
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
            if (orderByType == OrderByType.Desc)
                return list.ThenByDescending(it => ConvertField(it[sortField]));
            else
                return list.ThenBy(it => ConvertField(it[sortField]));
        }
        #endregion


        #region 多组

        /// <summary>
        /// 排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="orderByTypes"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<T> OrderBy<T>(this IEnumerable<T> list, List<OrderByDictionary> orderByTypes)
        {
            var type = typeof(T);
            IOrderedEnumerable<T> reval = list.OrderBy(it => true);
            foreach (OrderByDictionary orderByType in orderByTypes)
            {
                PropertyInfo prop = type.GetProperty(orderByType.OrderByField);
                Check.Exception(prop == null, "No property '" + orderByType.OrderByField + "' in + " + typeof(T).Name + "'");
                if (!orderByType.IsAsc)
                    reval = reval.ThenByDescending(it => ConvertField(prop.GetValue(it, null)));
                else
                    reval = reval.ThenBy(it => ConvertField(prop.GetValue(it, null)));
            }
            return reval;
        }
        
        /// <summary>
        /// 排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="orderByTypes"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<T> ThenBy<T>(this IOrderedEnumerable<T> list, List<OrderByDictionary> orderByTypes)
        {
            var type = typeof(T);
            foreach (OrderByDictionary orderByType in orderByTypes)
            {
                PropertyInfo prop = type.GetProperty(orderByType.OrderByField);
                Check.Exception(prop == null, "No property '" + orderByType.OrderByField + "' in + " + typeof(T).Name + "'");
                if (!orderByType.IsAsc)
                    list = list.ThenByDescending(it => ConvertField(prop.GetValue(it, null)));
                else
                    list = list.ThenBy(it => ConvertField(prop.GetValue(it, null)));
            }
            return list;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="orderByTypes"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<T> OrderByDataRow<T>(this IEnumerable<T> list, List<OrderByDictionary> orderByTypes) where T : DataRow
        {
            var type = typeof(T);
            IOrderedEnumerable<T> reval = null;
            foreach (OrderByDictionary orderByType in orderByTypes)
            {
                if (reval == null)
                {
                    PropertyInfo prop = type.GetProperty(orderByType.OrderByField);
                    if (!orderByType.IsAsc)
                        reval = list.OrderByDescending(it => ConvertField(it[orderByType.OrderByField]));
                    else
                        reval = list.OrderBy(it => ConvertField(it[orderByType.OrderByField]));
                }
                else
                {
                    PropertyInfo prop = type.GetProperty(orderByType.OrderByField);
                    if (!orderByType.IsAsc)
                        reval = reval.ThenByDescending(it => ConvertField(it[orderByType.OrderByField]));
                    else
                        reval = reval.ThenBy(it => ConvertField(it[orderByType.OrderByField]));
                }
            }
            return reval;
        }
        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="list"></param>
        /// <param name="orderByTypes"></param>
        /// <param name="orderByUnqueField"></param>
        /// <returns></returns>
        public static List<DataRow> OrderByDataRow(this IEnumerable<DataRow> list, List<OrderByDictionary> orderByTypes, OrderByDictionary orderByUnqueField)
        {
            orderByTypes.Add(orderByUnqueField);
            var dt = list.AsEnumerable().CopyToDataTable();
            var guidType = typeof(Guid);
            var sqlGuidType = typeof(SqlGuid);
            System.Data.DataTable dtByConvertGuidToSqlGuid= new System.Data.DataTable();
            foreach (DataColumn it in dt.Columns)
            {
                var isGuid = it.DataType == guidType;
                if (isGuid)
                    dtByConvertGuidToSqlGuid.Columns.Add(it.ColumnName, sqlGuidType);
                else
                    dtByConvertGuidToSqlGuid.Columns.Add(it.ColumnName, it.DataType);
            }
            //将dataTable中guid换转成sqlguid，这样排序才会和SQL一致
            dtByConvertGuidToSqlGuid.Load(dt.CreateDataReader(), System.Data.LoadOption.OverwriteChanges);
            var view = dtByConvertGuidToSqlGuid.AsDataView();
            view.Sort = string.Join(",", orderByTypes.Select(it => string.Format(" {0} {1} ", it.OrderByField, it.IsAsc ? "ASC" : "DESC")));
            var reval = view.ToTable().AsEnumerable().ToList();
            orderByTypes.Remove(orderByUnqueField);
            return reval;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="orderByTypes"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<T> ThenByDataRow<T>(this IOrderedEnumerable<T> list, List<OrderByDictionary> orderByTypes) where T : DataRow
        {
            var type = typeof(T);
            foreach (OrderByDictionary orderByType in orderByTypes)
            {
                PropertyInfo prop = type.GetProperty(orderByType.OrderByField);
                if (!orderByType.IsAsc)
                    list = list.ThenByDescending(it => ConvertField(it[orderByType.OrderByField]));
                else
                    list = list.ThenBy(it => ConvertField(it[orderByType.OrderByField]));
            }
            return list;
        }
        #endregion


        #region 多组反转

        /// <summary>
        /// 排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="orderByTypes"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<T> OrderByReverse<T>(this IEnumerable<T> list, List<OrderByDictionary> orderByTypes)
        {
            var type = typeof(T);
            IOrderedEnumerable<T> reval = list.OrderBy(it => true);
            foreach (OrderByDictionary orderByType in orderByTypes)
            {
                PropertyInfo prop = type.GetProperty(orderByType.OrderByField);
                Check.Exception(prop == null, "No property '" + orderByType.OrderByField + "' in + " + typeof(T).Name + "'");
                if (orderByType.IsAsc)
                    reval = reval.ThenByDescending(it => ConvertField(prop.GetValue(it, null)));
                else
                    reval = reval.ThenBy(it => ConvertField(prop.GetValue(it, null)));
            }
            return reval;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="orderByTypes"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<T> ThenByReverse<T>(this IOrderedEnumerable<T> list, List<OrderByDictionary> orderByTypes)
        {
            var type = typeof(T);
            foreach (OrderByDictionary orderByType in orderByTypes)
            {
                PropertyInfo prop = type.GetProperty(orderByType.OrderByField);
                Check.Exception(prop == null, "No property '" + orderByType.OrderByField + "' in + " + typeof(T).Name + "'");
                if (orderByType.IsAsc)
                    list = list.ThenByDescending(it => ConvertField(prop.GetValue(it, null)));
                else
                    list = list.ThenBy(it => ConvertField(prop.GetValue(it, null)));
            }
            return list;
        }

       
        /// <summary>
        /// 排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="orderByTypes"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<T> OrderByDataRowReverse<T>(this IEnumerable<T> list, List<OrderByDictionary> orderByTypes) where T : DataRow
        {
            var type = typeof(T);
            IOrderedEnumerable<T> reval = list.OrderBy(it => true);
            foreach (OrderByDictionary orderByType in orderByTypes)
            {
                PropertyInfo prop = type.GetProperty(orderByType.OrderByField);
                if (orderByType.IsAsc)
                    reval = list.OrderByDescending(it => ConvertField(it[orderByType.OrderByField]));
                else
                    reval = list.OrderBy(it => ConvertField(it[orderByType.OrderByField]));
            }
            return reval;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="orderByTypes"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<T> ThenByDataRowReverse<T>(this IOrderedEnumerable<T> list, List<OrderByDictionary> orderByTypes) where T : DataRow
        {
            var type = typeof(T);
            foreach (OrderByDictionary orderByType in orderByTypes)
            {
                PropertyInfo prop = type.GetProperty(orderByType.OrderByField);
                if (orderByType.IsAsc)
                    list = list.ThenByDescending(it => ConvertField(it[orderByType.OrderByField]));
                else
                    list = list.ThenBy(it => ConvertField(it[orderByType.OrderByField]));
            }
            return list;
        }
        #endregion


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
