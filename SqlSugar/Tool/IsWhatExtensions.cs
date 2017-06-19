using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlSugar
{
    /// <summary>
    /// ** 描述：逻辑判段是什么？
    /// ** 创始时间：2015-5-29
    /// ** 修改时间：-
    /// ** 作者：sunkaixuan
    /// ** 使用说明：http://www.cnblogs.com/sunkaixuan/p/4539654.html
    /// </summary>
    internal static class IsWhatExtensions
    {
        /// <summary>
        /// 值在的范围？
        /// </summary>
        /// <param name="thisValue"></param>
        /// <param name="begin">大于等于begin</param>
        /// <param name="end">小于等于end</param>
        /// <returns></returns>
        public static bool IsInRange(this int thisValue, int begin, int end)
        {
            return thisValue >= begin && thisValue <= end;
        }
        /// <summary>
        /// 值在的范围？
        /// </summary>
        /// <param name="thisValue"></param>
        /// <param name="begin">大于等于begin</param>
        /// <param name="end">小于等于end</param>
        /// <returns></returns>
        public static bool IsInRange(this DateTime thisValue, DateTime begin, DateTime end)
        {
            return thisValue >= begin && thisValue <= end;
        }

        /// <summary>
        /// 在里面吗?
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="thisValue"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool IsIn<T>(this T thisValue, params T[] values)
        {
            return values.Contains(thisValue);
        }

       /// <summary>
        /// 在里面吗?
       /// </summary>
       /// <param name="thisValue"></param>
       /// <param name="inValues"></param>
       /// <returns></returns>
        public static bool IsContainsIn(this string thisValue, params string[] inValues)
        {
            return inValues.Any(it => thisValue.Contains(it));
        }

        /// <summary>
        /// 是null或""?
        /// </summary>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this object thisValue)
        {
            if (thisValue == null || thisValue == DBNull.Value) return true;
            return thisValue.ToString() == "";
        }
        /// <summary>
        /// 是null或""?
        /// </summary>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this Guid? thisValue)
        {
            if (thisValue == null) return true;
            return thisValue == Guid.Empty;
        }
        /// <summary>
        /// 是null或""?
        /// </summary>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this Guid thisValue)
        {
            if (thisValue == null) return true;
            return thisValue == Guid.Empty;
        }

        /// <summary>
        /// 有值?(与IsNullOrEmpty相反)
        /// </summary>
        /// <returns></returns>
        public static bool IsValuable(this object thisValue)
        {
            if (thisValue == null || thisValue == DBNull.Value) return false;
            return thisValue.ToString() != "";
        }
        /// <summary>
        /// 有值?(与IsNullOrEmpty相反)
        /// </summary>
        /// <returns></returns>
        public static bool IsValuable(this IEnumerable<object> thisValue)
        {
            if (thisValue == null || thisValue.Count() == 0) return false;
            return true;
        }

        /// <summary>
        /// 是零?
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsZero(this object thisValue)
        {
            return (thisValue == null || thisValue.ToString() == "0");
        }

        /// <summary>
        /// 是INT?
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsInt(this object thisValue)
        {
            if (thisValue == null) return false;
            return Regex.IsMatch(thisValue.ToString(), @"^\d+$");
        }
        /// <summary>
        /// 不是INT?
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsNoInt(this object thisValue)
        {
            if (thisValue == null) return true;
            return !Regex.IsMatch(thisValue.ToString(), @"^\d+$");
        }

        /// <summary>
        /// 是金钱?
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsMoney(this object thisValue)
        {
            if (thisValue == null) return false;
            double outValue = 0;
            return double.TryParse(thisValue.ToString(), out outValue);
        }

        /// <summary>
        /// 是GUID?
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsGuid(this object thisValue)
        {
            if (thisValue == null) return false;
            Guid outValue = Guid.Empty;
            return Guid.TryParse(thisValue.ToString(), out outValue);
        }

        /// <summary>
        /// 是时间?
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsDate(this object thisValue)
        {
            if (thisValue == null) return false;
            DateTime outValue = DateTime.MinValue;
            return DateTime.TryParse(thisValue.ToString(), out outValue);
        }

        /// <summary>
        /// 是邮箱?
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsEamil(this object thisValue)
        {
            if (thisValue == null) return false;
            return Regex.IsMatch(thisValue.ToString(), @"^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$");
        }

        /// <summary>
        /// 是手机?
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsMobile(this object thisValue)
        {
            if (thisValue == null) return false;
            return Regex.IsMatch(thisValue.ToString(), @"^\d{11}$");
        }

        /// <summary>
        /// 是座机?
        /// </summary>
        public static bool IsTelephone(this object thisValue)
        {
            if (thisValue == null) return false;
            return System.Text.RegularExpressions.Regex.IsMatch(thisValue.ToString(), @"^(\(\d{3,4}\)|\d{3,4}-|\s)?\d{8}$");

        }

        /// <summary>
        /// 是身份证?
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsIDcard(this object thisValue)
        {
            if (thisValue == null) return false;
            return System.Text.RegularExpressions.Regex.IsMatch(thisValue.ToString(), @"^(\d{15}$|^\d{18}$|^\d{17}(\d|X|x))$");
        }

        /// <summary>
        /// 是传真?
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsFax(this object thisValue)
        {
            if (thisValue == null) return false;
            return System.Text.RegularExpressions.Regex.IsMatch(thisValue.ToString(), @"^[+]{0,1}(\d){1,3}[ ]?([-]?((\d)|[ ]){1,12})+$");
        }

        /// <summary>
        /// 是适合正则匹配?
        /// </summary>
        /// <param name="thisValue"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static bool IsMatch(this object thisValue, string pattern)
        {
            if (thisValue == null) return false;
            Regex reg = new Regex(pattern);
            return reg.IsMatch(thisValue.ToString());
        }

        /// <summary>
        /// 是否是动态类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsAnonymousType(this Type type)
        {
            string typeName = type.Name;
            return typeName.Contains("<>") && typeName.Contains("__") && typeName.Contains("AnonymousType");
        }
        /// <summary>
        /// 是List类型
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsCollectionsList(this string thisValue)
        {
            return (thisValue + "").StartsWith("System.Collections.Generic.List");
        }
        /// <summary>
        /// 是string[]类型
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsStringArray(this string thisValue)
        {
            return (thisValue + "").IsMatch(@"System\.[a-z,A-Z,0-9]+?\[\]");
        }
        /// <summary>
        /// 是Enumerable
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsEnumerable(this string thisValue)
        {
            return (thisValue + "").StartsWith("System.Linq.Enumerable");
        }
    }
}
