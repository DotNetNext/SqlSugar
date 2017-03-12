using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Linq.Expressions;

namespace SqlSugar
{
    ///<summary>
    /// ** 描述：SqlSugar扩展工具类
    /// ** 创始时间：2015-7-19
    /// ** 修改时间：-
    /// ** 作者：sunkaixuan
    /// ** 使用说明：
    /// </summary>
    public static class SqlSugarToolExtensions
    {

        /// <summary>
        /// 将数组转换成Where In 需要的格式(例如:参数 new int{1,2,3} 反回 "'1','2','3'")
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static string ToJoinSqlInVal<T>(this T[] array)
        {
            if (array == null || array.Length == 0)
            {
                return ToSqlValue(string.Empty);
            }
            else
            {
                return string.Join(",", array.Where(c => c != null).Select(it => (it + "").ToSqlValue()));
            }
        }
        /// <summary>
        /// 将字符串转换成SQL参数所需要的格式(例如: 参数value返回'value')
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToSqlValue(this string value)
        {
            return string.Format("'{0}'", value.ToSqlFilter());
        }
        /// <summary>
        ///SQL注入过滤
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToSqlFilter(this string value)
        {
            if (!value.IsNullOrEmpty())
            {
                value = value.Replace("'", "''");//转释单引号
            }
            return value;
        }
        /// <summary>
        ///  指定数据类型，如果不在指定类当中则引发异常(只允许输入指定字母、数字、下划线、时间、GUID)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToSuperSqlFilter(this string value)
        {
            if (value.IsNullOrEmpty()) return value;
            if (Regex.IsMatch(value, @"^(\w|\.|\:|\-| |\,)+$"))
            {
                return value;
            }
            throw new SqlSugarException("指定类型(只允许输入指定字母、数字、下划线、时间、guid)。");
        }

        /// <summary>
        /// 获取锁字符串
        /// </summary>
        /// <param name="isNoLock"></param>
        /// <returns></returns>
        internal static string GetLockString(this bool isNoLock)
        {
            return isNoLock ? "WITH(NOLOCK)" : null; ;
        }
        /// <summary>
        /// 获取Select需要的字段
        /// </summary>
        /// <param name="selectFileds"></param>
        /// <returns></returns>
        internal static string GetSelectFiles(this string selectFileds)
        {
            return selectFileds.IsNullOrEmpty() ? "*" : selectFileds;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupByFileds"></param>
        /// <returns></returns>
        internal static string GetGroupBy(this string groupByFileds)
        {
            return groupByFileds.IsNullOrEmpty() ? "" : " GROUP BY " + groupByFileds;
        }
        /// <summary>
        /// 将Request里的参数转成SqlParameter[]
        /// </summary>
        /// <returns></returns>
        internal static void RequestParasToSqlParameters(SqlParameterCollection oldParas)
        {
            var oldParaList = oldParas.Cast<SqlParameter>().ToList();
            var paraDictionarAll = SqlSugarTool.GetParameterDictionary();
            if (paraDictionarAll != null && paraDictionarAll.Count() > 0)
            {

                foreach (KeyValuePair<string, string> it in paraDictionarAll)
                {

                    var par = new SqlParameter("@" + it.Key, it.Value);
                    if (!oldParaList.Any(oldPara => oldPara.ParameterName == ("@" + it.Key)))
                    {
                        oldParas.Add(par);
                    }
                }
            }
        }
        /// <summary>
        /// 获取转释后的表名和列名
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static string GetTranslationSqlName(this string tableName)
        {
            return SqlSugarTool.GetTranslationSqlName(tableName);
        }
        /// <summary>
        /// 获取参数名
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal static string GetSqlParameterName(this string name)
        {
            return SqlSugarTool.GetSqlParameterName(name);
        }

        /// <summary>
        ///获取没有符号的参数名称
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal static string GetSqlParameterNameNoParSymbol(this string name)
        {
            return SqlSugarTool.GetSqlParameterNameNoParSymbol(name);
        }

        /// <summary>
        /// 数组条件筛选
        /// </summary>
        /// <param name="thisValue"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal static string[] ArrayWhere(this string[] thisValue, Func<string, bool> expression)
        {
            if (thisValue == null) return null;
            thisValue= thisValue.Where(expression).ToArray();
            return thisValue;
        }

        /// <summary>
        /// 数组添加元素
        /// </summary>
        /// <param name="thisValue"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        internal static string[] ArrayAdd(this string[] thisValue, params string[] items)
        {
            if (thisValue == null) thisValue = new string[] { };
            var reval= thisValue.ToList();
            reval.AddRange(items);
            thisValue = reval.ToArray();
            return thisValue;
        }

        /// <summary>
        /// 数组移除
        /// </summary>
        /// <param name="thisValue"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        internal static string[] ArrayRemove(this string[] thisValue, string item)
        {
            if (thisValue == null) thisValue = new string[] { };
            var reval = thisValue.ToList();
            reval.Remove(item);
            thisValue= reval.ToArray();
            return thisValue;
        }
    }
}
