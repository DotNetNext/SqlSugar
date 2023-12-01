using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
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
        internal static bool? _IsErrorDecimalString { get; set; }
        internal static bool? IsErrorDecimalString() 
        {
            if (_IsErrorDecimalString == null)
            {
                decimal dec = Convert.ToDecimal(1.1);
                _IsErrorDecimalString = dec.ToString().Contains(",");
            }
            return _IsErrorDecimalString;
        }
        internal static bool IsParameterConverter(EntityColumnInfo columnInfo)
        {
            return columnInfo != null && columnInfo.SqlParameterDbType != null && columnInfo.SqlParameterDbType is Type
                && typeof(ISugarDataConverter).IsAssignableFrom(columnInfo.SqlParameterDbType as Type);
        }
        internal static SugarParameter GetParameterConverter(int index,ISqlSugarClient db,object value, Expression oppoSiteExpression, EntityColumnInfo columnInfo)
        {
            var entity = db.EntityMaintenance.GetEntityInfo(oppoSiteExpression.Type);
            var type = columnInfo.SqlParameterDbType as Type;
            var ParameterConverter = type.GetMethod("ParameterConverter").MakeGenericMethod(columnInfo.PropertyInfo.PropertyType);
            var obj = Activator.CreateInstance(type);
            var p = ParameterConverter.Invoke(obj, new object[] { value, 100+index }) as SugarParameter;
            return p;
        }
        internal static bool IsErrorParameterName(ConnectionConfig connectionConfig,DbColumnInfo columnInfo)
        {
            return connectionConfig.MoreSettings?.IsCorrectErrorSqlParameterName == true &&
                            columnInfo.DbColumnName.IsContainsIn("-"," ", ".", "(", ")", "（", "）");
        }

        public static bool StringCheckFirstAndLast(string withString, string first, string last)
        {
            return withString.StartsWith(first) && withString.EndsWith(last);
        }
        public static bool HasInterface(Type targetType, Type interfaceType)
        {
            if (targetType == null || interfaceType == null || !interfaceType.IsInterface)
            {
                return false;
            }

            return interfaceType.IsAssignableFrom(targetType);
        }
        public static void ClearPublicProperties<T>(T obj,EntityInfo entity)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            Type type = typeof(T);
    
            foreach (var column in entity.Columns)
            {
                if (column.PropertyInfo.CanWrite && column.PropertyInfo.GetSetMethod() != null)
                {
                    Type propertyType = column.PropertyInfo.PropertyType;
                    object defaultValue = propertyType.IsValueType ? Activator.CreateInstance(propertyType) : null;
                    column.PropertyInfo.SetValue(obj, defaultValue);
                }
            }
        }

        internal static Expression GetIncludeExpression(string navMemberName, EntityInfo entityInfo, out Type properyItemType,out bool isList)
        {
            var navInfo = entityInfo.Columns.Where(it => it.Navigat != null && it.PropertyName.EqualCase(navMemberName)).FirstOrDefault();
            var properyType = navInfo.PropertyInfo.PropertyType;
            properyItemType = properyType;
            if (properyType.FullName.IsCollectionsList())
            {
                properyItemType = properyType.GetGenericArguments()[0];
                isList = true;
            }
            else 
            {
                isList = false;
            }
           return ExpressionBuilderHelper.CreateExpressionSelectField(entityInfo.Type, navInfo.PropertyName, properyType);
        }
        public static string RemoveEqualOne(string value)
        {
            value = value.TrimEnd(' ').TrimEnd('1').TrimEnd('=');
            return value;
        }
        /// <summary>
        /// Available only in Select,Handles logic that cannot be completed by an expression
        /// </summary>
        /// <param name="addValue"></param>
        /// <param name="valueFomatInfo"></param>
        /// <returns></returns>
        internal static object GetFormatValue(object addValue, QueryableFormat valueFomatInfo)
        {
            if (valueFomatInfo.MethodName == "ToString")
            {
                if (valueFomatInfo.Type == UtilConstants.GuidType)
                {
                    addValue = Guid.Parse(addValue + "").ToString(valueFomatInfo.Format);
                }
                else if (valueFomatInfo.Type == UtilConstants.ByteType)
                {
                    addValue = Convert.ToByte(addValue + "").ToString(valueFomatInfo.Format);
                }
                else if (valueFomatInfo.Type == UtilConstants.IntType)
                {
                    addValue = Convert.ToInt32(addValue + "").ToString(valueFomatInfo.Format);
                }
                else if (valueFomatInfo.Type == UtilConstants.LongType)
                {
                    addValue = Convert.ToInt64(addValue + "").ToString(valueFomatInfo.Format);
                }
                else if (valueFomatInfo.Type == UtilConstants.UIntType)
                {
                    addValue = Convert.ToUInt32(addValue + "").ToString(valueFomatInfo.Format);
                }
                else if (valueFomatInfo.Type == UtilConstants.ULongType)
                {
                    addValue = Convert.ToUInt64(addValue + "").ToString(valueFomatInfo.Format);
                }
                else if (valueFomatInfo.Type == UtilConstants.DecType)
                {
                    addValue = Convert.ToDecimal(addValue + "").ToString(valueFomatInfo.Format);
                }
                else if (valueFomatInfo.Type == UtilConstants.DobType)
                {
                    addValue = Convert.ToDouble(addValue + "").ToString(valueFomatInfo.Format);
                }
                else if (valueFomatInfo.TypeString == "Enum")
                {
                    addValue =ChangeType2(addValue, valueFomatInfo.Type)?.ToString();
                }
            }
            else if (valueFomatInfo.MethodName== "OnlyInSelectConvertToString") 
            {

                var methodInfo = valueFomatInfo.MethodInfo;
                if (methodInfo != null)
                {
                    // 如果方法是静态的，传递null作为第一个参数，否则传递类的实例
                    object instance = methodInfo.IsStatic ? null : Activator.CreateInstance(methodInfo.ReflectedType);

                    // 创建一个包含参数值的object数组
                    object[] parameters = new object[] { addValue };

                    // 调用方法
                    addValue=methodInfo.Invoke(instance, parameters);  
                }
            }
            return addValue;
        }
        public  static int CountSubstringOccurrences(string mainString, string searchString)
        {
            string[] substrings = mainString.Split(new string[] { searchString }, StringSplitOptions.None);
            return substrings.Length - 1;
        }
        public static string RemoveBeforeFirstWhere(string query)
        {
            int whereIndex = query.IndexOf("WHERE", StringComparison.OrdinalIgnoreCase);
            if (whereIndex >= 0)
            {
                return query.Substring(whereIndex + "WHERE".Length);
            }
            else
            {
                return query;
            }
        }
        public static List<object> ConvertToListOfObjects(object inValues)
        {
            // 创建一个新的List<object>并逐个将元素转换并添加到其中
            List<object> resultList = new List<object>();
            foreach (var item in (IEnumerable)inValues )
            {
                resultList.Add(item);
            }

            return resultList;
        }
        public static bool IsValueTypeArray(object memberValue)
        {
            return memberValue is List<string> ||
                   memberValue is string[] ||
                   memberValue is List<int> ||
                   memberValue is int[] ||
                   memberValue is List<Guid> ||
                   memberValue is Guid[] ||
                   memberValue is List<long> ||
                   memberValue is long[] ||
                   memberValue is List<int?> ||
                   memberValue is int?[] ||
                   memberValue is List<Guid?> ||
                   memberValue is Guid?[] ||
                   memberValue is List<long?> ||
                   memberValue is long?[] ||
                   memberValue is List<float> ||
                   memberValue is float[] ||
                   memberValue is List<double> ||
                   memberValue is double[] ||
                   memberValue is List<decimal> ||
                   memberValue is decimal[] ||
                   memberValue is List<DateTime> ||
                   memberValue is DateTime[] ||
                   memberValue is List<TimeSpan> ||
                   memberValue is TimeSpan[] ||
                   memberValue is List<bool> ||
                   memberValue is bool[] ||
                   memberValue is List<byte> ||
                   memberValue is byte[] ||
                   memberValue is List<char> ||
                   memberValue is char[] ||
                   memberValue is List<short> ||
                   memberValue is short[] ||
                   memberValue is List<ushort> ||
                   memberValue is ushort[] ||
                   memberValue is List<uint> ||
                   memberValue is uint[] ||
                   memberValue is List<ulong> ||
                   memberValue is ulong[] ||
                   memberValue is List<sbyte> ||
                   memberValue is sbyte[] ||
                   memberValue is List<object> ||
                   memberValue is object[] ||
                   memberValue is List<int?> ||
                   memberValue is int?[] ||
                   memberValue is List<Guid?> ||
                   memberValue is Guid?[] ||
                   memberValue is List<long?> ||
                   memberValue is long?[];
        }
        internal static void EndCustomSplitTable(ISqlSugarClient context,Type entityType)
        {
            if (context == null || entityType == null) 
            {
                return;
            }
            var splitTableAttribute = entityType.GetCustomAttribute<SplitTableAttribute>();
            if (splitTableAttribute == null) 
            {
                return;
            }
            if (splitTableAttribute.CustomSplitTableService != null)
            {
                context.CurrentConnectionConfig.ConfigureExternalServices.SplitTableService = null;
            }
        }

        internal static void StartCustomSplitTable(ISqlSugarClient context, Type entityType)
        {
            if (context == null || entityType == null)
            {
                return;
            }
            var splitTableAttribute = entityType.GetCustomAttribute<SplitTableAttribute>();
            if (splitTableAttribute == null)
            {
                return;
            }
            if (splitTableAttribute.CustomSplitTableService != null)
            {
                context.CurrentConnectionConfig.ConfigureExternalServices.SplitTableService
                    = (ISplitTableService)Activator.CreateInstance(splitTableAttribute.CustomSplitTableService);
            }
        }
        public static void ConvertParameter(SugarParameter p, ISqlBuilder builder)
        {
            if (!p.ParameterName.StartsWith(builder.SqlParameterKeyWord))
            {
                p.ParameterName = (builder.SqlParameterKeyWord + p.ParameterName.TrimStart('@'));
            }
        }
        public static object SetAnonymousObjectPropertyValue(object obj, string propertyName, object propertyValue)
        {
            if (obj.GetType().IsAnonymousType()) // 判断是否为匿名对象
            {
                var objType = obj.GetType();
                var objFields = objType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
                foreach (var field in objFields) // 遍历字段列表，查找需要修改的属性
                {
                    if (field.Name == $"<{propertyName}>i__Field")
                    {
                        field.SetValue(obj, propertyValue); // 使用反射修改属性值
                        break;
                    }
                }
            }
            else
            {
                obj.GetType().GetProperty(propertyName).SetValue(obj, propertyValue);
            }
            return obj;
        }

        internal static bool IsNumberArray(Type type)
        {
         
            return type.IsIn(typeof(int[]),
                               typeof(long[]),
                               typeof(short[]),
                               typeof(uint[]),
                               typeof(ulong[]),
                               typeof(ushort[]),
                               typeof(int?[]),
                               typeof(long?[]),
                               typeof(short?[]),
                               typeof(uint?[]),
                               typeof(ulong?[]),
                               typeof(ushort?[]),
                               typeof(List<int>),
                               typeof(List<long>),
                               typeof(List<short>),
                               typeof(List<uint>),
                               typeof(List<ulong>),
                               typeof(List<ushort>),
                               typeof(List<int?>),
                               typeof(List<long?>),
                               typeof(List<short?>),
                               typeof(List<uint?>),
                               typeof(List<ulong?>),
                               typeof(List<ushort?>));
        }
        public static string GetNativeSql(string sql,SugarParameter[] pars)
        {
            if (pars == null||pars.Length==0)
                return "\r\n[Sql]:"+sql+"\r\n";
            return $"\r\n[Sql]:{sql} \r\n[Pars]:{string.Join(" ",pars.Select(it=>$"\r\n[Name]:{it.ParameterName} [Value]:{it.Value} [Type]:{it.DbType} {(it.IsNvarchar2?"nvarchar2":"")}  "))} \r\n";
        }
        public static string ToUnderLine(string str, bool isToUpper = false)
        {
            if (str == null || str.Contains("_"))
            {
                return str;
            }
            else if (isToUpper)
            {
                return string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToUpper();
            }
            else
            {
                return string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
            }
        }
        internal static bool IsArrayMember(Expression expression, SqlSugarProvider context)
        {
            if (expression == null)
                return false;
            if (!(expression is LambdaExpression))
                return false;
            var lambda = (LambdaExpression)expression;
            if (!(lambda.Body is MemberExpression))
                return false;
            var member = lambda.Body as MemberExpression;
            if (!(member.Type.IsClass()))
                return false;
            if (member.Expression == null)
                return false;
            var entity = context.EntityMaintenance.GetEntityInfo(member.Expression.Type);
            var json = entity.Columns.FirstOrDefault(z => z.IsArray && z.PropertyName == member.Member.Name);
            return json != null;
        }
        internal static bool IsJsonMember(Expression expression, SqlSugarProvider context)
        {
            if (expression == null)
                return false;
            if (!(expression is LambdaExpression))
                return false;
            var lambda = (LambdaExpression)expression;
            if (!(lambda.Body is MemberExpression))
                return false;
            var member = lambda.Body as MemberExpression;
            if (!(member.Type.IsClass()))
                return false;
            if (member.Expression == null)
                return false;
            var entity = context.EntityMaintenance.GetEntityInfo(member.Expression.Type);
            var json = entity.Columns.FirstOrDefault(z => z.IsJson && z.PropertyName == member.Member.Name);
            return json != null;
        }
        public static string GetSeparatorChar()
        {
            return Path.Combine("a", "a").Replace("a", "");
        }
        public static bool IsParentheses(object name)
        {
            return name.ObjToString().Trim().Last() == ')' && name.ObjToString().Trim().First() == '(';
        }

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
                if (destinationType.Name == "DateOnly"&&sourceType==typeof(string)) 
                {
                    var type = Type.GetType("System.DateOnly", true, true);
                    var method = type.GetMethods().FirstOrDefault(it => it.GetParameters().Length == 1 && it.Name == "FromDateTime");
                    return method.Invoke(null, new object[] {Convert.ToDateTime(value)});
                }
                var destinationConverter = TypeDescriptor.GetConverter(destinationType);
                if (destinationConverter != null && destinationConverter.CanConvertFrom(value.GetType()))
                    return destinationConverter.ConvertFrom(null, culture, value);

                var sourceConverter = TypeDescriptor.GetConverter(sourceType);
                if (sourceConverter != null && sourceConverter.CanConvertTo(destinationType))
                    return sourceConverter.ConvertTo(null, culture, value, destinationType);

                if (destinationType.IsEnum && value is int)
                    return Enum.ToObject(destinationType, (int)value);

                if (destinationType.Name == "TimeOnly"&& sourceType.Name!= "TimeOnly") 
                {
                    var type = Type.GetType("System.TimeOnly", true, true);
                    var method=type.GetMethods().FirstOrDefault(it => it.GetParameters().Length == 1 && it.Name == "FromTimeSpan");
                    return method.Invoke(null, new object[] { value });
                }
                if (destinationType.Name == "DateOnly" && sourceType.Name != "DateOnly")
                {
                    var type = Type.GetType("System.DateOnly", true, true);
                    var method = type.GetMethods().FirstOrDefault(it => it.GetParameters().Length == 1 && it.Name == "FromDateTime");
                    return method.Invoke(null, new object[] { value });
                }

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
                   DataExecuted = it.AopEvents?.DataExecuted,
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
                DbLinkName= it.DbLinkName,
                IndexSuffix = it.IndexSuffix,
                InitKeyType = it.InitKeyType,
                IsAutoCloseConnection = it.IsAutoCloseConnection,
                LanguageType = it.LanguageType,
                MoreSettings = it.MoreSettings == null ? null : new ConnMoreSettings()
                {
                    DefaultCacheDurationInSeconds = it.MoreSettings.DefaultCacheDurationInSeconds,
                    DisableNvarchar = it.MoreSettings.DisableNvarchar,
                    PgSqlIsAutoToLower = it.MoreSettings.PgSqlIsAutoToLower,
                    PgSqlIsAutoToLowerCodeFirst= it.MoreSettings.PgSqlIsAutoToLowerCodeFirst,
                    IsAutoRemoveDataCache = it.MoreSettings.IsAutoRemoveDataCache,
                    IsWithNoLockQuery = it.MoreSettings.IsWithNoLockQuery,
                    TableEnumIsString = it.MoreSettings.TableEnumIsString,
                    DisableMillisecond = it.MoreSettings.DisableMillisecond,
                    DbMinDate=it.MoreSettings.DbMinDate,
                    IsNoReadXmlDescription=it.MoreSettings.IsNoReadXmlDescription,
                    SqlServerCodeFirstNvarchar=it.MoreSettings.SqlServerCodeFirstNvarchar,
                    IsAutoToUpper=it.MoreSettings.IsAutoToUpper,
                    IsAutoDeleteQueryFilter=it.MoreSettings.IsAutoDeleteQueryFilter,
                    IsAutoUpdateQueryFilter = it.MoreSettings.IsAutoUpdateQueryFilter,
                    EnableModelFuncMappingColumn=it.MoreSettings.EnableModelFuncMappingColumn,
                    EnableOracleIdentity = it.MoreSettings.EnableOracleIdentity,
                    IsWithNoLockSubquery=it.MoreSettings.IsWithNoLockSubquery,
                    EnableCodeFirstUpdatePrecision=it.MoreSettings.EnableCodeFirstUpdatePrecision,
                    SqliteCodeFirstEnableDefaultValue=it.MoreSettings.SqliteCodeFirstEnableDefaultValue,
                    SqliteCodeFirstEnableDescription=it.MoreSettings.SqliteCodeFirstEnableDescription,
                    IsCorrectErrorSqlParameterName = it.MoreSettings.IsCorrectErrorSqlParameterName,
                    SqliteCodeFirstEnableDropColumn=it.MoreSettings.SqliteCodeFirstEnableDropColumn

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

        internal static object GetRandomByType(Type underType)
        {
            if (underType == UtilConstants.GuidType)
            {
                return Guid.NewGuid();
            }
            else if (underType == UtilConstants.LongType)
            {
                return SnowFlakeSingle.Instance.NextId();
            }
            else if (underType == UtilConstants.StringType)
            {
                return Guid.NewGuid() + "";
            }
            else if (underType == UtilConstants.DateType)
            {
                return System.DateTime.Now;
            }
            else 
            {
                return Guid.NewGuid() + "";
            }
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
            //if (method?.DeclaringType?.FullName?.Contains("Furion.InternalApp")==true)
            //{
            //    return false;
            //}
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

        internal static object GetConvertValue(object entityValue)
        {
            if (entityValue != null && entityValue is DateTime)
            {
                entityValue = entityValue.ObjToDate().ToString("yyyy-MM-dd HH:mm:ss.fff");
            }
            return entityValue;
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

        public static Type GetUnderType(Type oldType)
        {
            Type type = Nullable.GetUnderlyingType(oldType);
            return type == null ? oldType : type;
        }
        public static object GetDefaultValue(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
        public static string ReplaceFirstMatch(string input, string pattern, string replacement)
        {
            Regex regex = new Regex(pattern);
            return regex.Replace(input, replacement, 1);
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
            if (value is string && type == typeof(Guid?)) return value.IsNullOrEmpty() ? null : (Guid?)new Guid(value as string);
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
                if (Regex.IsMatch(dbTypeName, @"SimpleAggregateFunction"))
                    dbTypeName = Regex.Match(dbTypeName, @"((?<=,\s)[^Nullable]\w+)|((?<=Nullable\()\w+)").Value;
                else
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

        public static string GetSqlValue(object value)
        {
            if (value == null)
            {
                return "null";
            }
            else if (UtilMethods.IsNumber(value.GetType().Name))
            {
                return value.ObjToString();
            }
            else if (value is DateTime)
            {
                return UtilMethods.GetConvertValue(value) + "";
            }
            else
            {
                return value.ToSqlValue();
            }
        }

        public static void DataInoveByExpresson<Type>(Type[] datas, MethodCallExpression callExpresion)
        {
            var methodInfo = callExpresion.Method;
            foreach (var item in datas)
            {
                if (item != null)
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

        public static Type GetTypeByTypeName(string ctypename)
        {
 
            if (ctypename.EqualCase(UtilConstants.DecType.Name))
            {
                return UtilConstants.DecType;
            }
            else if (ctypename.EqualCase(UtilConstants.DobType.Name))
            {
                return UtilConstants.DobType;
            }
            else if (ctypename.EqualCase(UtilConstants.DateType.Name))
            {
                return UtilConstants.DateType;
            }
            else if (ctypename.EqualCase(UtilConstants.IntType.Name))
            {
                return UtilConstants.IntType;
            }
            else if (ctypename.EqualCase(UtilConstants.BoolType.Name))
            {
                return UtilConstants.BoolType;
            }
            else if (ctypename.EqualCase(UtilConstants.LongType.Name))
            {
                return UtilConstants.LongType;
            }
            else if (ctypename.EqualCase(UtilConstants.ShortType.Name))
            {
                return UtilConstants.ShortType;
            }
            else if (ctypename.EqualCase(UtilConstants.DateTimeOffsetType.Name))
            {
                return UtilConstants.DateTimeOffsetType;
            }
            else if (ctypename.EqualCase(UtilConstants.GuidType.Name))
            {
                return UtilConstants.GuidType;
            }
            else if (ctypename.EqualCase("int"))
            {
                return UtilConstants.IntType;
            }
            else if (ctypename.EqualCase("long"))
            {
                return UtilConstants.LongType;
            }
            else if (ctypename.EqualCase("short"))
            {
                return UtilConstants.ShortType;
            }
            else if (ctypename.EqualCase("byte"))
            {
                return UtilConstants.ByteType;
            }
            else if (ctypename.EqualCase("uint"))
            {
                return UtilConstants.UIntType;
            }
            else if (ctypename.EqualCase("ulong"))
            {
                return UtilConstants.ULongType;
            }
            else if (ctypename.EqualCase("ushort"))
            {
                return UtilConstants.UShortType;
            }
            else if (ctypename.EqualCase("uint32"))
            {
                return UtilConstants.UIntType;
            }
            else if (ctypename.EqualCase("uint64"))
            {
                return UtilConstants.ULongType;
            }
            else if (ctypename.EqualCase("bool"))
            {
                return UtilConstants.BoolType;
            }
            else if (ctypename.EqualCase("ToBoolean"))
            {
                return UtilConstants.BoolType;
            }
            else if (ctypename.EqualCase("uint16"))
            {
                return UtilConstants.UShortType;
            }
            else if (ctypename.EqualCase(UtilConstants.ByteArrayType.Name))
            {
                return UtilConstants.ByteArrayType;
            }
            else
            {
                return UtilConstants.StringType;
            }
        }
        public static object ConvertDataByTypeName(string ctypename,string value)
        {
            var item = new ConditionalModel() {
                CSharpTypeName = ctypename,
                FieldValue = value
            };
            if (item.FieldValue == string.Empty && item.CSharpTypeName.HasValue() && !item.CSharpTypeName.EqualCase("string")) 
            {
                return null;
            }
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
            else if (item.FieldValue!=null&&item.CSharpTypeName.EqualCase(UtilConstants.BoolType.Name))
            {
                return Convert.ToBoolean(item.FieldValue.ToLower());
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
                    if (item.ParameterName.StartsWith(":")&&!result.Contains(item.ParameterName)) 
                    {
                        item.ParameterName = "@"+item.ParameterName.TrimStart(':');
                    }
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
                    else if (item.Value is DateTime)
                    {
                        result = result.Replace(item.ParameterName, "'"+item.Value.ObjToDate().ToString("yyyy-MM-dd HH:mm:ss.fff")+"'");
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
                        result = result.Replace(item.ParameterName, $"'{item.Value.ObjToString().ToSqlFilter()}'");
                    }
                    else
                    {
                        result = result.Replace(item.ParameterName, $"N'{item.Value.ObjToString().ToSqlFilter()}'");
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

        public static string FiledNameSql()
        {
            return $"[value=sql{UtilConstants.ReplaceKey}]";
        }

        internal static object TimeOnlyToTimeSpan(object value)
        {
            if (value == null) return null;
            var method = value.GetType().GetMethods().First(it => it.GetParameters().Length == 0 && it.Name == "ToTimeSpan");
            return method.Invoke(value, new object[] { });
        }

        internal static object DateOnlyToDateTime(object value)
        {
            if (value == null) return null;
            var method = value.GetType().GetMethods().First(it => it.GetParameters().Length == 0 && it.Name == "ToShortDateString");
            return method.Invoke(value, new object[] { });
        }


        internal static void AddDiscrimator<T>(Type type, ISugarQueryable<T> queryable,string shortName=null)
        {
            var entityInfo = queryable.Context?.EntityMaintenance?.GetEntityInfoWithAttr(type);
            if (entityInfo!=null&&entityInfo.Discrimator.HasValue())
            {
                Check.ExceptionEasy(!Regex.IsMatch(entityInfo.Discrimator, @"^(?:\w+:\w+)(?:,\w+:\w+)*$"), "The format should be type:cat for this type, and if there are multiple, it can be FieldName:cat,FieldName2:dog ", "格式错误应该是type:cat这种格式，如果是多个可以FieldName:cat,FieldName2:dog，不要有空格");
                var array = entityInfo.Discrimator.Split(',');
                foreach (var disItem in array)
                {
                    var name = disItem.Split(':').First();
                    var value = disItem.Split(':').Last();
                    queryable.Where(shortName+name, "=", value);
                }
            }
        }
        internal static string GetDiscrimator(EntityInfo entityInfo,ISqlBuilder  builer)
        { 
            List<string> wheres = new List<string>();
            if (entityInfo != null && entityInfo.Discrimator.HasValue())
            {
                Check.ExceptionEasy(!Regex.IsMatch(entityInfo.Discrimator, @"^(?:\w+:\w+)(?:,\w+:\w+)*$"), "The format should be type:cat for this type, and if there are multiple, it can be FieldName:cat,FieldName2:dog ", "格式错误应该是type:cat这种格式，如果是多个可以FieldName:cat,FieldName2:dog，不要有空格");
                var array = entityInfo.Discrimator.Split(',');
                foreach (var disItem in array)
                {
                    var name = disItem.Split(':').First();
                    var value = disItem.Split(':').Last();
                    wheres.Add($"{builer.GetTranslationColumnName(name)}={value.ToSqlValue()} ");
                }
            }
            return string.Join(" AND ", wheres);
        }

    }
}
