using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlSugar
{
    public class UtilMethods
    {

        internal static bool IsDefaultValue(object value)
        {
            if (value == null) return true;
            return value.Equals(UtilMethods.GetDefaultValue(value.GetType()));
        }

        internal static DateTime ConvertFromDateTimeOffset(DateTimeOffset dateTime)
        {
            if (dateTime.Offset.Equals(TimeSpan.Zero))
                return dateTime.UtcDateTime;
            else if (dateTime.Offset.Equals(TimeZoneInfo.Local.GetUtcOffset(dateTime.DateTime)))
                return DateTime.SpecifyKind(dateTime.DateTime, DateTimeKind.Local);
            else
                return dateTime.DateTime;
        }

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
                    break;
                }
            }
            return isAsync;
        }


        public static ConnectionConfig CopyConfig(ConnectionConfig it)
        {
            return new ConnectionConfig()
            {
                AopEvents =it.AopEvents==null?null:new AopEvents() { 
                   DataExecuting=it.AopEvents?.DataExecuting,
                   OnDiffLogEvent=it.AopEvents?.OnDiffLogEvent,
                   OnError=it.AopEvents?.OnError,
                   OnExecutingChangeSql=it.AopEvents?.OnExecutingChangeSql,
                   OnLogExecuted=it.AopEvents?.OnLogExecuted,
                   OnLogExecuting= it.AopEvents?.OnLogExecuting,
                },
                ConfigId = it.ConfigId,
                ConfigureExternalServices =it.ConfigureExternalServices==null?null:new ConfigureExternalServices() { 
                      AppendDataReaderTypeMappings=it.ConfigureExternalServices.AppendDataReaderTypeMappings,
                      DataInfoCacheService=it.ConfigureExternalServices.DataInfoCacheService,
                      EntityNameService=it.ConfigureExternalServices.EntityNameService,
                      EntityService=it.ConfigureExternalServices.EntityService,
                      RazorService=it.ConfigureExternalServices.RazorService,
                      ReflectionInoCacheService=it.ConfigureExternalServices.ReflectionInoCacheService,
                      SerializeService=it.ConfigureExternalServices.SerializeService,
                      SplitTableService=it.ConfigureExternalServices.SplitTableService,
                      SqlFuncServices=it.ConfigureExternalServices.SqlFuncServices
                },
                ConnectionString = it.ConnectionString,
                DbType = it.DbType,
                IndexSuffix = it.IndexSuffix,
                InitKeyType = it.InitKeyType,
                IsAutoCloseConnection = it.IsAutoCloseConnection,
                LanguageType = it.LanguageType,
                MoreSettings = it.MoreSettings == null ? null : new ConnMoreSettings()
                {
                    DefaultCacheDurationInSeconds = it.MoreSettings.DefaultCacheDurationInSeconds,
                    DisableNvarchar = it.MoreSettings.DisableNvarchar,
                    PgSqlIsAutoToLower = it.MoreSettings.PgSqlIsAutoToLower,
                    IsAutoRemoveDataCache = it.MoreSettings.IsAutoRemoveDataCache,
                    IsWithNoLockQuery = it.MoreSettings.IsWithNoLockQuery,
                    TableEnumIsString = it.MoreSettings.TableEnumIsString,
                    DisableMillisecond = it.MoreSettings.DisableMillisecond,
                    DbMinDate=it.MoreSettings.DbMinDate
                },
                SqlMiddle = it.SqlMiddle == null ? null : new SqlMiddle
                {
                    IsSqlMiddle = it.SqlMiddle.IsSqlMiddle,
                    ExecuteCommand = it.SqlMiddle.ExecuteCommand,
                    ExecuteCommandAsync = it.SqlMiddle.ExecuteCommandAsync,
                    GetDataReader = it.SqlMiddle.GetDataReader,
                    GetDataReaderAsync = it.SqlMiddle.GetDataReaderAsync,
                    GetDataSetAll = it.SqlMiddle.GetDataSetAll,
                    GetDataSetAllAsync = it.SqlMiddle.GetDataSetAllAsync,
                    GetScalar = it.SqlMiddle.GetScalar,
                    GetScalarAsync = it.SqlMiddle.GetScalarAsync
                },
                SlaveConnectionConfigs = it.SlaveConnectionConfigs
            };
        }

        public static bool IsAsyncMethod(MethodBase method)
        {
            if (method == null)
            {
                return false;
            }
            if (method.DeclaringType != null)
            {
                if (method.DeclaringType.GetInterfaces().Contains(typeof(IAsyncStateMachine)))
                {
                    return true;
                }
            }
            var name = method.Name;
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
                if (frame.GetMethod().Module.Name.ToLower() != "sqlsugar.dll" && frame.GetMethod().Name.First() != '<')
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

        internal static DateTime GetMinDate(ConnectionConfig currentConnectionConfig)
        {
            if (currentConnectionConfig.MoreSettings == null)
            {
                return Convert.ToDateTime("1900-01-01");
            }
            else if (currentConnectionConfig.MoreSettings.DbMinDate == null)
            {
                return Convert.ToDateTime("1900-01-01");
            }
            else 
            {
                return currentConnectionConfig.MoreSettings.DbMinDate.Value;
            }
        }

        internal static Type GetUnderType(Type oldType)
        {
            Type type = Nullable.GetUnderlyingType(oldType);
            return type == null ? oldType : type;
        }
        public static object GetDefaultValue(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
        public static string ReplaceSqlParameter(string itemSql, SugarParameter itemParameter, string newName)
        {
            itemSql = Regex.Replace(itemSql, string.Format(@"{0} ", "\\" + itemParameter.ParameterName), newName + " ", RegexOptions.IgnoreCase);
            itemSql = Regex.Replace(itemSql, string.Format(@"{0}\)", "\\" + itemParameter.ParameterName), newName + ")", RegexOptions.IgnoreCase);
            itemSql = Regex.Replace(itemSql, string.Format(@"{0}\,", "\\" + itemParameter.ParameterName), newName + ",", RegexOptions.IgnoreCase);
            itemSql = Regex.Replace(itemSql, string.Format(@"{0}$", "\\" + itemParameter.ParameterName), newName, RegexOptions.IgnoreCase);
            itemSql = Regex.Replace(itemSql, string.Format(@"\+{0}\+", "\\" + itemParameter.ParameterName), "+" + newName + "+", RegexOptions.IgnoreCase);
            itemSql = Regex.Replace(itemSql, string.Format(@"\+{0} ", "\\" + itemParameter.ParameterName), "+" + newName + " ", RegexOptions.IgnoreCase);
            itemSql = Regex.Replace(itemSql, string.Format(@" {0}\+", "\\" + itemParameter.ParameterName), " " + newName + "+", RegexOptions.IgnoreCase);
            itemSql = Regex.Replace(itemSql, string.Format(@"\|\|{0}\|\|", "\\" + itemParameter.ParameterName), "||" + newName + "||", RegexOptions.IgnoreCase);
            itemSql = Regex.Replace(itemSql, string.Format(@"\={0}\+", "\\" + itemParameter.ParameterName), "=" + newName + "+", RegexOptions.IgnoreCase);
            itemSql = Regex.Replace(itemSql, string.Format(@"{0}\|\|", "\\" + itemParameter.ParameterName), newName + "||", RegexOptions.IgnoreCase);
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
        public static object ChangeType2(object value, Type type)
        {
            if (value == null && type.IsGenericType) return Activator.CreateInstance(type);
            if (value == null) return null;
            if (type == value.GetType()) return value;
            if (type.IsEnum)
            {
                if (value is string)
                    return Enum.Parse(type, value as string);
                else
                    return Enum.ToObject(type, value);
            }
            if (!type.IsInterface && type.IsGenericType)
            {
                Type innerType = type.GetGenericArguments()[0];
                object innerValue = ChangeType(value, innerType);
                return Activator.CreateInstance(type, new object[] { innerValue });
            }
            if (value is string && type == typeof(Guid)) return new Guid(value as string);
            if (value is string && type == typeof(Version)) return new Version(value as string);
            if (!(value is IConvertible)) return value;
            return Convert.ChangeType(value, type);
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

        public static Func<string, object> GetTypeConvert(object value)
        {
            if (value is int || value is uint || value is int? || value is uint?)
            {
                return x => Convert.ToInt32(x);
            }
            else if (value is short || value is ushort || value is short? || value is ushort?)
            {
                return x => Convert.ToInt16(x);
            }
            else if (value is long || value is long? || value is ulong? || value is long?)
            {
                return x => Convert.ToInt64(x);
            }
            else if (value is DateTime|| value is DateTime?)
            {
                return x => Convert.ToDateTime(x);
            }
            else if (value is bool||value is bool?)
            {
                return x => Convert.ToBoolean(x);
            }
            return null;
        }

        internal static string GetTypeName(object value)
        {
            if (value == null)
            {
                return null;
            }
            else 
            {
                return value.GetType().Name;
            }
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
        public static object GetPropertyValue<T>(T t, string PropertyName)
        {
            return t.GetType().GetProperty(PropertyName).GetValue(t, null);
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
            if (StaticConfig.Encode != null) 
            {
                return StaticConfig.Encode(code);
            }
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
                if (StaticConfig.Decode != null)
                {
                    return StaticConfig.Decode(code);
                }
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

        public static void DataInoveByExpresson<Type>(Type[] datas, MethodCallExpression callExpresion)
        {
            var methodInfo = callExpresion.Method;
            foreach (var item in datas)
            {
                if (callExpresion.Arguments.Count == 0)
                {
                    methodInfo.Invoke(item, null);
                }
                else
                {
                    List<object> methodParameters = new List<object>();
                    foreach (var callItem in callExpresion.Arguments)
                    {
                        var parameter = callItem.GetType().GetProperties().FirstOrDefault(it => it.Name == "Value");
                        if (parameter == null)
                        {
                            var value = LambdaExpression.Lambda(callItem).Compile().DynamicInvoke();
                            methodParameters.Add(value);
                        }
                        else
                        {
                            var value = parameter.GetValue(callItem, null);
                            methodParameters.Add(value);
                        }
                    }
                    methodInfo.Invoke(item, methodParameters.ToArray());
                }
            }
        }

        public static Dictionary<string, T> EnumToDictionary<T>()
        {
            Dictionary<string, T> dic = new Dictionary<string, T>();
            if (!typeof(T).IsEnum)
            {
                return dic;
            }
            string desc = string.Empty;
            foreach (var item in Enum.GetValues(typeof(T)))
            {
                var key = item.ToString().ToLower();
                if (!dic.ContainsKey(key))
                    dic.Add(key, (T)item);
            }
            return dic;
        }
        public static object ConvertDataByTypeName(string ctypename,string value)
        {
            var item = new ConditionalModel() {
                CSharpTypeName = ctypename,
                FieldValue = value
            };
            if (item.CSharpTypeName.EqualCase(UtilConstants.DecType.Name))
            {
                return Convert.ToDecimal(item.FieldValue);
            }
            else if (item.CSharpTypeName.EqualCase(UtilConstants.DobType.Name))
            {
                return Convert.ToDouble(item.FieldValue);
            }
            else if (item.CSharpTypeName.EqualCase(UtilConstants.DateType.Name))
            {
                return Convert.ToDateTime(item.FieldValue);
            }
            else if (item.CSharpTypeName.EqualCase(UtilConstants.IntType.Name))
            {
                return Convert.ToInt32(item.FieldValue);
            }
            else if (item.CSharpTypeName.EqualCase(UtilConstants.LongType.Name))
            {
                return Convert.ToInt64(item.FieldValue);
            }
            else if (item.CSharpTypeName.EqualCase(UtilConstants.ShortType.Name))
            {
                return Convert.ToInt16(item.FieldValue);
            }
            else if (item.CSharpTypeName.EqualCase(UtilConstants.DateTimeOffsetType.Name))
            {
                return UtilMethods.GetDateTimeOffsetByDateTime(Convert.ToDateTime(item.FieldValue));
            }
            else if (item.CSharpTypeName.EqualCase(UtilConstants.GuidType.Name))
            {
                return Guid.Parse(item.FieldValue);
            }
            else if (item.CSharpTypeName.EqualCase("int"))
            {
                return Convert.ToInt32(item.FieldValue);
            }
            else if (item.CSharpTypeName.EqualCase("long"))
            {
                return Convert.ToInt64(item.FieldValue);
            }
            else if (item.CSharpTypeName.EqualCase("short"))
            {
                return Convert.ToInt16(item.FieldValue);
            }
            else if (item.CSharpTypeName.EqualCase("byte"))
            {
                return Convert.ToByte(item.FieldValue);
            }
            else if (item.CSharpTypeName.EqualCase("uint"))
            {
                return Convert.ToUInt32(item.FieldValue);
            }
            else if (item.CSharpTypeName.EqualCase("ulong"))
            {
                return Convert.ToUInt64(item.FieldValue);
            }
            else if (item.CSharpTypeName.EqualCase("ushort"))
            {
                return Convert.ToUInt16(item.FieldValue);
            }
            else if (item.CSharpTypeName.EqualCase("uint32"))
            {
                return Convert.ToUInt32(item.FieldValue);
            }
            else if (item.CSharpTypeName.EqualCase("uint64"))
            {
                return Convert.ToUInt64(item.FieldValue);
            }
            else if (item.CSharpTypeName.EqualCase("bool"))
            {
                return Convert.ToBoolean(item.FieldValue);
            }
            else if (item.CSharpTypeName.EqualCase("ToBoolean"))
            {
                return Convert.ToBoolean(item.FieldValue);
            }
            else if (item.CSharpTypeName.EqualCase("uint16"))
            {
                return Convert.ToUInt16(item.FieldValue);
            }
            else if (item.CSharpTypeName.EqualCase("byte[]")&&item.FieldValue!=null&&item.FieldValue.Contains("|")) 
            {
                return item.FieldValue.Split('|').Select(it=>Convert.ToByte(it)).ToArray();
            }
            else
            {
                return item.FieldValue;
            }
        }

        public static bool IsNumber(string ctypename)
        {
            if (ctypename.IsNullOrEmpty()) 
            {
                return false;
            }
            var item = new ConditionalModel()
            {
                CSharpTypeName = ctypename,
            };
            if (item.CSharpTypeName.EqualCase(UtilConstants.DecType.Name))
            {
                return true;
            }
            else if (item.CSharpTypeName.EqualCase(UtilConstants.DobType.Name))
            {
                return true;
            }
            else if (item.CSharpTypeName.EqualCase(UtilConstants.IntType.Name))
            {
                return true;
            }
            else if (item.CSharpTypeName.EqualCase(UtilConstants.LongType.Name))
            {
                return true;
            }
            else if (item.CSharpTypeName.EqualCase(UtilConstants.ShortType.Name))
            {
                return true;
            }
            else if (item.CSharpTypeName.EqualCase("int"))
            {
                return true;
            }
            else if (item.CSharpTypeName.EqualCase("long"))
            {
                return true;
            }
            else if (item.CSharpTypeName.EqualCase("short"))
            {
                return true;
            }
            else if (item.CSharpTypeName.EqualCase("byte"))
            {
                return true;
            }
            else if (item.CSharpTypeName.EqualCase("uint"))
            {
                return true;
            }
            else if (item.CSharpTypeName.EqualCase("ulong"))
            {
                return true;
            }
            else if (item.CSharpTypeName.EqualCase("ushort"))
            {
                return true;
            }
            else if (item.CSharpTypeName.EqualCase("uint32"))
            {
                return true;
            }
            else if (item.CSharpTypeName.EqualCase("uint64"))
            {
                return true;
            }
            else if (item.CSharpTypeName.EqualCase("uint16"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Get Week Last Day Sun
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static DateTime GetWeekLastDaySun(DateTime datetime)
        {
            //星期天为最后一天
            int weeknow = Convert.ToInt32(datetime.DayOfWeek);
            weeknow = (weeknow == 0 ? 7 : weeknow);
            int daydiff = (7 - weeknow);

            //本周最后一天
            string LastDay = datetime.AddDays(daydiff).ToString("yyyy-MM-dd");
            return Convert.ToDateTime(LastDay);
        }
        /// <summary>
        /// Get Week First Day Mon
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static DateTime GetWeekFirstDayMon(DateTime datetime)
        {
            //星期一为第一天
            int weeknow = Convert.ToInt32(datetime.DayOfWeek);

            //因为是以星期一为第一天，所以要判断weeknow等于0时，要向前推6天。
            weeknow = (weeknow == 0 ? (7 - 1) : (weeknow - 1));
            int daydiff = (-1) * weeknow;

            //本周第一天
            string FirstDay = datetime.AddDays(daydiff).ToString("yyyy-MM-dd");
            return Convert.ToDateTime(FirstDay);
        }
        public static string GetSqlString(DbType dbType, string sql, SugarParameter []  parametres,bool DisableNvarchar=false) 
        {
            if (parametres == null)
                parametres = new SugarParameter[] { };
            return GetSqlString(new ConnectionConfig()
            {
                DbType = dbType,
                MoreSettings=new ConnMoreSettings() 
                { 
                     DisableNvarchar=DisableNvarchar
                }
            },new  KeyValuePair<string, List<SugarParameter>>(sql,parametres.ToList()));
        }
        public static string GetSqlString(ConnectionConfig connectionConfig,KeyValuePair<string, List<SugarParameter>> sqlObj)
        {
            var result = sqlObj.Key;
            if (sqlObj.Value != null)
            {
                foreach (var item in sqlObj.Value.OrderByDescending(it => it.ParameterName.Length))
                {
                    if (connectionConfig.MoreSettings == null) 
                    {
                        connectionConfig.MoreSettings = new ConnMoreSettings();
                    }
                    if (item.Value != null && item.Value is DateTime &&((DateTime)item.Value==DateTime.MinValue)) 
                    {
                        item.Value = connectionConfig.MoreSettings.DbMinDate;
                    }
                    if (item.Value == null || item.Value == DBNull.Value)
                    {
                        result = result.Replace(item.ParameterName, "null");
                    }
                    else if (UtilMethods.IsNumber(item.Value.GetType().Name))
                    {
                        result = result.Replace(item.ParameterName, item.Value.ObjToString());
                    }
                    else if (item.Value is byte[])
                    {
                        result = result.Replace(item.ParameterName, "0x" + BitConverter.ToString((byte[])item.Value));
                    }
                    else if (item.Value is bool)
                    {
                        if (connectionConfig.DbType == DbType.PostgreSQL)
                        {
                            result = result.Replace(item.ParameterName, (Convert.ToBoolean(item.Value) ? "true": "false")  );
                        }
                        else
                        {
                            result = result.Replace(item.ParameterName, (Convert.ToBoolean(item.Value) ? 1 : 0) + "");
                        }
                    }
                    else if (item.Value.GetType() != UtilConstants.StringType && connectionConfig.DbType == DbType.PostgreSQL && PostgreSQLDbBind.MappingTypesConst.Any(x => x.Value.ToString().EqualCase(item.Value.GetType().Name)))
                    {
                        var type = PostgreSQLDbBind.MappingTypesConst.First(x => x.Value.ToString().EqualCase(item.Value.GetType().Name)).Key;
                        var replaceValue = string.Format("CAST('{0}' AS {1})", item.Value, type);
                        result = result.Replace(item.ParameterName, replaceValue);
                    }
                    else if (connectionConfig.MoreSettings?.DisableNvarchar == true || item.DbType == System.Data.DbType.AnsiString || connectionConfig.DbType == DbType.Sqlite)
                    {
                        result = result.Replace(item.ParameterName, $"'{item.Value.ObjToString()}'");
                    }
                    else
                    {
                        result = result.Replace(item.ParameterName, $"N'{item.Value.ObjToString()}'");
                    }
                }
            }

            return result;
        }

        public static void CheckArray<T>(T[] insertObjs) where T : class, new()
        {

            if (insertObjs != null 
                && insertObjs.Length == 1
                && insertObjs.FirstOrDefault()!=null
                && insertObjs.FirstOrDefault().GetType().FullName.Contains("System.Collections.Generic.List`"))
            {
                Check.ExceptionEasy("Insertable(T []) is an array and your argument is a List", "二次封装引起的进错重载,当前方法是 Insertable(T []) 参数是一个数组，而你的参数是一个List");
            }
        }

    }
}
