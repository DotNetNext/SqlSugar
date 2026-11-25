// 外置包自带的工具类副本（ClickHouse 外置包先例）：主库中这些工具类为 internal，跨程序集
// 不可见；副本与主库同签名、同名、internal，仅在包内可见，不与主库冲突，也不暴露公共面。
// 本文件成员镜像自主库 06789315f（origin/master）版工具类（镜像类清单：Utilities/ValidateExtensions.cs、
// Utilities/UtilConvert.cs、OnlyNet/AsyncHelper.cs、Utilities/UtilConstants.cs、
// ExpressionsToSql/Common/ErrorMessage.cs 的 ConnnectionOpen），主库演进须同步本副本。
// 成员清单即 Vastbase 载荷实际使用的最小集。
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace SqlSugar
{
    internal static class ValidateExtensions
    {
        public static bool IsIn<T>(this T thisValue, params T[] values)
        {
            return values.Contains(thisValue);
        }
        public static bool IsNullOrEmpty(this object thisValue)
        {
            if (thisValue == null || thisValue == DBNull.Value) return true;
            return thisValue.ToString() == "";
        }
        public static bool HasValue(this object thisValue)
        {
            if (thisValue == null || thisValue == DBNull.Value) return false;
            return thisValue.ToString() != "";
        }
        public static bool HasValue(this IEnumerable<object> thisValue)
        {
            if (thisValue == null || thisValue.Count() == 0) return false;
            return true;
        }
        public static bool IsValuable(this IEnumerable<KeyValuePair<string, string>> thisValue)
        {
            if (thisValue == null || thisValue.Count() == 0) return false;
            return true;
        }
    }

    internal static class UtilConvert
    {
        public static bool EqualCase(this string thisValue, string equalValue)
        {
            if (thisValue != null && equalValue != null)
            {
                return thisValue.ToLower() == equalValue.ToLower();
            }
            else
            {
                return thisValue == equalValue;
            }
        }
        public static string ObjToString(this object thisValue)
        {
            if (thisValue != null) return thisValue.ToString().Trim();
            return "";
        }
        public static string ObjToStringNoTrim(this object thisValue)
        {
            if (thisValue != null) return thisValue.ToString();
            return "";
        }
        public static int ObjToInt(this object thisValue)
        {
            int reval = 0;
            if (thisValue == null) return 0;
            if (thisValue is Enum)
            {
                return Convert.ToInt32(thisValue);
            }
            if (thisValue != null && thisValue != DBNull.Value && int.TryParse(thisValue.ToString(), out reval))
            {
                return reval;
            }
            return reval;
        }
        public static bool ObjToBool(this object thisValue)
        {
            bool reval = false;
            if (thisValue != null && thisValue != DBNull.Value && bool.TryParse(thisValue.ToString(), out reval))
            {
                return reval;
            }
            return reval;
        }
        public static DateTime ObjToDate(this object thisValue)
        {
            if (thisValue is DateTime)
            {
                return (DateTime)thisValue;
            }
            DateTime reval = DateTime.MinValue;
            if (thisValue != null && thisValue != DBNull.Value && DateTime.TryParse(thisValue.ToString(), out reval))
            {
                reval = Convert.ToDateTime(thisValue);
            }
            return reval;
        }
    }

    internal static class AsyncHelper
    {
        public static async Task RollbackAsync(this DbTransaction db)
        {
            db.Rollback();
            //.net frame work no RollbackAsync
            await Task.Delay(0);
        }
        public static async Task CommitAsync(this DbTransaction db)
        {
            db.Commit();
            //.net frame work no CommitAsyncAsync
            await Task.Delay(0);
        }
    }

    internal static class UtilConstants
    {
        internal static Type GuidType = typeof(Guid);
        internal static Type StringType = typeof(string);
        internal static Type DateType = typeof(DateTime);
    }

    internal static partial class ErrorMessage
    {
        // 镜像主库 ErrorMessage.ConnnectionOpen（主库现行为 GetThrowMessage 双语形态）。
        // GetThrowMessage 是主库 internal，经反射直调主库实现——语言开关（ConnectionConfig.
        // LanguageType 公共通道写入）与主库异常文案同源，主库演进该格式时随锚点同步。
        public static string ConnnectionOpen
        {
            get
            {
                return _mainGetThrowMessage.Value(
                    "Connection open error . {0} ",
                    " 连接数据库过程中发生错误，检查服务器是否正常连接字符串是否正确，错误信息：{0}.",
                    new string[0]);
            }
        }
        private static readonly System.Lazy<System.Func<string, string, string[], string>> _mainGetThrowMessage =
            new System.Lazy<System.Func<string, string, string[], string>>(() =>
            {
                var main = typeof(SqlSugarClient).Assembly.GetType("SqlSugar.ErrorMessage");
                var method = main.GetMethod("GetThrowMessage",
                    System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                return (System.Func<string, string, string[], string>)method.CreateDelegate(
                    typeof(System.Func<string, string, string[], string>));
            });
    }
}
