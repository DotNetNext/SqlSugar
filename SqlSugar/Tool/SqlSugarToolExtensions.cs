﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Data.SqlClient;

namespace SqlSugar
{

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
                return string.Join(",", array.Where(c => c != null).Select(it => (it + "").ToSuperSqlFilter().ToSqlValue()));
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
                if (Regex.IsMatch(value, @"%\d|0x\d|0x[0-9,a-z]{6,300}|(\@.*\=)", RegexOptions.IgnoreCase))
                {
                    throw new SqlSugarException("查询参数不允许存在特殊字符。");
                }
                value = value.Replace("'", "''");
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
        internal static void RequestParasToSqlParameters(SqlParameterCollection  oldParas)
        {
            var oldParaList= oldParas.Cast<SqlParameter>().ToList();
            var paraDictionarAll =SqlSugarTool.GetParameterDictionary();
            if (paraDictionarAll != null && paraDictionarAll.Count() > 0)
            {
            
                foreach (KeyValuePair<string, string> it in paraDictionarAll)
                {
                 
                    var par=new SqlParameter("@" + it.Key, it.Value);
                    if (!oldParaList.Any(oldPara=>oldPara.ParameterName==("@"+it.Key)))
                    {
                        oldParas.Add(par);
                    }
                }
            }
        }

    }
}
