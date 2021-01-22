using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlSugar
{
    public class UtilMethods
    {

        internal static object To(object value, Type destinationType)
        {
            return To(value, destinationType, CultureInfo.InvariantCulture);
        }

        internal static object To(object value, Type destinationType, CultureInfo culture)
        {
            if (value != null)
            {
                destinationType = UtilMethods.GetUnderType(destinationType);
                var sourceType = value.GetType();

                var destinationConverter = TypeDescriptor.GetConverter(destinationType);
                if (destinationConverter != null && destinationConverter.CanConvertFrom(value.GetType()))
                    return destinationConverter.ConvertFrom(null, culture, value);

                var sourceConverter = TypeDescriptor.GetConverter(sourceType);
                if (sourceConverter != null && sourceConverter.CanConvertTo(destinationType))
                    return sourceConverter.ConvertTo(null, culture, value, destinationType);

                if (destinationType.IsEnum && value is int)
                    return Enum.ToObject(destinationType, (int)value);

                if (!destinationType.IsInstanceOfType(value))
                    return Convert.ChangeType(value, destinationType, culture);
            }
            return value;
        }
        public static bool IsAnyAsyncMethod(StackFrame[] methods)
        {
            bool isAsync = false;
            foreach (var item in methods)
            {
                if (UtilMethods.IsAsyncMethod(item.GetMethod()))
                {
                    isAsync = true;
                }
            }
            return isAsync;
        }

        public static bool IsAsyncMethod(MethodBase method)
        {
            if (method == null)
            {
                return false;
            }
            var name= method.Name;
            if (name.Contains("OutputAsyncCausalityEvents"))
            {
                return true;
            }
            if (name.Contains("OutputWaitEtwEvents"))
            {
                return true;
            }
            if (name.Contains("ExecuteAsync"))
            {
                return true;
            }
            Type attType = typeof(AsyncStateMachineAttribute); 
            var attrib = (AsyncStateMachineAttribute)method.GetCustomAttribute(attType);
            return (attrib != null);
        }

        public static StackTraceInfo GetStackTrace()
        {

            StackTrace st = new StackTrace(true);
            StackTraceInfo info = new StackTraceInfo();
            info.MyStackTraceList = new List<StackTraceInfoItem>();
            info.SugarStackTraceList = new List<StackTraceInfoItem>();
            for (int i = 0; i < st.FrameCount; i++)
            {
                var frame = st.GetFrame(i);
                if (frame.GetMethod().Module.Name.ToLower() != "sqlsugar.dll"&& frame.GetMethod().Name.First()!='<')
                {
                    info.MyStackTraceList.Add(new StackTraceInfoItem()
                    {
                        FileName = frame.GetFileName(),
                        MethodName = frame.GetMethod().Name,
                        Line = frame.GetFileLineNumber()
                    });
                }
                else
                {
                    info.SugarStackTraceList.Add(new StackTraceInfoItem()
                    {
                        FileName = frame.GetFileName(),
                        MethodName = frame.GetMethod().Name,
                        Line = frame.GetFileLineNumber()
                    });
                }
            }
            return info;
        }

        internal static T To<T>(object value)
        {
            return (T)To(value, typeof(T));
        }
        internal static Type GetUnderType(Type oldType)
        {
            Type type = Nullable.GetUnderlyingType(oldType);
            return type == null ? oldType : type;
        }
        public static string ReplaceSqlParameter(string itemSql, SugarParameter itemParameter, string newName)
        {
            itemSql = Regex.Replace(itemSql, string.Format(@"{0} ", "\\" + itemParameter.ParameterName), newName + " ", RegexOptions.IgnoreCase);
            itemSql = Regex.Replace(itemSql, string.Format(@"{0}\)", "\\" + itemParameter.ParameterName), newName + ")", RegexOptions.IgnoreCase);
            itemSql = Regex.Replace(itemSql, string.Format(@"{0}\,", "\\" + itemParameter.ParameterName), newName + ",", RegexOptions.IgnoreCase);
            itemSql = Regex.Replace(itemSql, string.Format(@"{0}$", "\\" + itemParameter.ParameterName), newName, RegexOptions.IgnoreCase);
            itemSql = Regex.Replace(itemSql, string.Format(@"\+{0}\+", "\\" + itemParameter.ParameterName), "+" + newName + "+", RegexOptions.IgnoreCase);
            itemSql = Regex.Replace(itemSql, string.Format(@"\+{0} ", "\\" + itemParameter.ParameterName), "+" + newName +" ", RegexOptions.IgnoreCase);
            itemSql = Regex.Replace(itemSql, string.Format(@" {0}\+", "\\" + itemParameter.ParameterName)," "+ newName + "+", RegexOptions.IgnoreCase);
            itemSql = Regex.Replace(itemSql, string.Format(@"\|\|{0}\|\|", "\\" + itemParameter.ParameterName), "+" + newName + "+", RegexOptions.IgnoreCase);
            return itemSql;
        }
        internal static Type GetRootBaseType(Type entityType)
        {
            var baseType = entityType.BaseType;
            while (baseType != null && baseType.BaseType != UtilConstants.ObjType)
            {
                baseType = baseType.BaseType;
            }
            return baseType;
        }


        internal static Type GetUnderType(PropertyInfo propertyInfo, ref bool isNullable)
        {
            Type unType = Nullable.GetUnderlyingType(propertyInfo.PropertyType);
            isNullable = unType != null;
            unType = unType ?? propertyInfo.PropertyType;
            return unType;
        }

        internal static Type GetUnderType(PropertyInfo propertyInfo)
        {
            Type unType = Nullable.GetUnderlyingType(propertyInfo.PropertyType);
            unType = unType ?? propertyInfo.PropertyType;
            return unType;
        }

        internal static bool IsNullable(PropertyInfo propertyInfo)
        {
            Type unType = Nullable.GetUnderlyingType(propertyInfo.PropertyType);
            return unType != null;
        }

        internal static bool IsNullable(Type type)
        {
            Type unType = Nullable.GetUnderlyingType(type);
            return unType != null;
        }
        internal static T IsNullReturnNew<T>(T returnObj) where T : new()
        {
            if (returnObj.IsNullOrEmpty())
            {
                returnObj = new T();
            }
            return returnObj;
        }

        internal static T ChangeType<T>(T obj, Type type)
        {
            return (T)Convert.ChangeType(obj, type);
        }

        internal static T ChangeType<T>(T obj)
        {
            return (T)Convert.ChangeType(obj, typeof(T));
        }

        internal static DateTimeOffset GetDateTimeOffsetByDateTime(DateTime date)
        {
            date = DateTime.SpecifyKind(date, DateTimeKind.Utc);
            DateTimeOffset utcTime2 = date;
            return utcTime2;
        }

        internal static void RepairReplicationParameters(ref string appendSql, SugarParameter[] parameters, int addIndex, string append = null)
        {
            if (appendSql.HasValue() && parameters.HasValue())
            {
                foreach (var parameter in parameters.OrderByDescending(it => it.ParameterName.Length))
                {
                    //Compatible with.NET CORE parameters case
                    var name = parameter.ParameterName;
                    string newName = name + append + addIndex;
                    appendSql = ReplaceSqlParameter(appendSql, parameter, newName);
                    parameter.ParameterName = newName;
                }
            }
        }

        internal static string GetPackTable(string sql, string shortName)
        {
            return string.Format(" ({0}) {1} ", sql, shortName);
        }

        internal static string GetParenthesesValue(string dbTypeName)
        {
            if (Regex.IsMatch(dbTypeName, @"\(.+\)"))
            {
                dbTypeName = Regex.Replace(dbTypeName, @"\(.+\)", "");
            }
            dbTypeName = dbTypeName.Trim();
            return dbTypeName;
        }

        internal static T GetOldValue<T>(T value, Action action)
        {
            action();
            return value;
        }

        internal static object DefaultForType(Type targetType)
        {
            return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
        }

        internal static Int64 GetLong(byte[] bytes)
        {
            return Convert.ToInt64(string.Join("", bytes).PadRight(20, '0'));
        }

        internal static string GetMD5(string myString)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = System.Text.Encoding.Unicode.GetBytes(myString);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x");
            }

            return byte2String;
        }

        public static string EncodeBase64(string code)
        {
            if (code.IsNullOrEmpty()) return code;
            string encode = "";
            byte[] bytes = Encoding.GetEncoding("utf-8").GetBytes(code);
            try
            {
                encode = Convert.ToBase64String(bytes);
            }
            catch
            {
                encode = code;
            }
            return encode;
        }
        public static string ConvertNumbersToString(string value)
        {
            string[] splitInt = value.Split(new char[] { '9' }, StringSplitOptions.RemoveEmptyEntries);

            var splitChars = splitInt.Select(s => Convert.ToChar(
                                              Convert.ToInt32(s, 8)
                                            ).ToString());

            return string.Join("", splitChars);
        }
        public static string ConvertStringToNumbers(string value)
        {
            StringBuilder sb = new StringBuilder();

            foreach (char c in value)
            {
                int cAscil = (int)c;
                sb.Append(Convert.ToString(c, 8) + "9");
            }

            return sb.ToString();
        }

        public static string DecodeBase64(string code)
        {
            try
            {
                if (code.IsNullOrEmpty()) return code;
                string decode = "";
                byte[] bytes = Convert.FromBase64String(code);
                try
                {
                    decode = Encoding.GetEncoding("utf-8").GetString(bytes);
                }
                catch
                {
                    decode = code;
                }
                return decode;
            }
            catch
            {
                return code;
            }
        }

    }
}
