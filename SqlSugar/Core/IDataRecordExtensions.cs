using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace SqlSugar
{
    /// <summary>
    /// ** 描述：IDataRecord扩展类
    /// ** 创始时间：2016-8-7
    /// ** 修改时间：-
    /// ** 作者：孙凯旋
    /// ** 使用说明：
    /// </summary>
    public static class IDataRecordExtensions
    {
        /// <summary>
        /// 获取bool
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public static bool? GetConvertBoolean(this IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return null;
            }
            var reval = dr.GetBoolean(i);
            return reval;
        }

        /// <summary>
        /// 获取byte
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public static byte? GetConvertByte(this IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return null;
            }
            var reval = dr.GetByte(i);
            return reval;
        }

        /// <summary>
        /// 获取char
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public static char? GetConvertChar(this IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return null;
            }
            var reval = dr.GetChar(i);
            return reval;
        }

        /// <summary>
        /// 获取时间
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public static DateTime? GetConvertDateTime(this IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return null;
            }
            var reval = dr.GetDateTime(i);
            return reval;
        }

        /// <summary>
        /// 获取转换Decimal
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public static decimal? GetConvertDecimal(this IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return null;
            }
            var reval = dr.GetDecimal(i);
            return reval;
        }

        /// <summary>
        /// 获取Double
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public static double? GetConvertDouble(this IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return null;
            }
            var reval = dr.GetDouble(i);
            return reval;
        }

        /// <summary>
        /// 获取GUID
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public static Guid? GetConvertGuid(this IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return null;
            }
            var reval = dr.GetGuid(i);
            return reval;
        }

        /// <summary>
        /// 获取int16
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public static short? GetConvertInt16(this IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return null;
            }
            var reval = dr.GetInt16(i);
            return reval;
        }

        /// <summary>
        /// 获取int32
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public static Int32? GetConvertInt32(this IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return null;
            }
            var reval = dr.GetInt32(i);
            return reval;
        }

        /// <summary>
        /// 获取int64
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public static long? GetConvetInt64(this IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return null;
            }
            var reval = dr.GetInt64(i);
            return reval;
        }

        /// <summary>
        /// 获取float
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public static float? GetConvertFloat(this IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return null;
            }
            var reval = dr.GetFloat(i);
            return reval;
        }

        /// <summary>
        /// 获取其它类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public static Nullable<T> GetOtherNull<T>(this IDataReader dr, int i) where T : struct
        {
            if (dr.IsDBNull(i))
            {
                return null;
            }
            return (T)Convert.ChangeType(dr.GetValue(i), typeof(T));

        }

        /// <summary>
        /// 获取其它类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public static T GetOther<T>(this IDataReader dr, int i) 
        {
            return (T)Convert.ChangeType(dr.GetValue(i), typeof(T));
        }

        /// <summary>
        /// 获取枚举
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public static Nullable<T> GetConvertEnum_Nullable<T>(this IDataReader dr, int i) where T : struct
        {
            if (dr.IsDBNull(i))
            {
                return null;
            }
            object value = dr.GetValue(i);
            T t = (T)Enum.ToObject(typeof(T), value);
            return t;
        }

    }
}
