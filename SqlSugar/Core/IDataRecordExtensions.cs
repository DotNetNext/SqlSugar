using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace SqlSugar
{
    /// <summary>
    /// DataRecord扩展
    /// </summary>
    public static class IDataRecordExtensions
    {

        public static bool? GetConvertBoolean(this IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return null;
            }
            var reval = dr.GetBoolean(i);
            return reval;
        }

        public static byte? GetConvertByte(this IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return null;
            }
            var reval = dr.GetByte(i);
            return reval;
        }

        public static char? GetConvertChar(this IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return null;
            }
            var reval = dr.GetChar(i);
            return reval;
        }

        public static DateTime? GetConvertDateTime(this IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return null;
            }
            var reval = dr.GetDateTime(i);
            return reval;
        }

        public static decimal? GetConvertDecimal(this IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return null;
            }
            var reval = dr.GetDecimal(i);
            return reval;
        }

        public static double? GetConvertDouble(this IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return null;
            }
            var reval = dr.GetDouble(i);
            return reval;
        }

        public static Guid? GetConvertGuid(this IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return null;
            }
            var reval = dr.GetGuid(i);
            return reval;
        }

        public static short? GetConvertInt16(this IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return null;
            }
            var reval = dr.GetInt16(i);
            return reval;
        }

        public static Int32? GetConvertInt32(this IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return null;
            }
            var reval = dr.GetInt32(i);
            return reval;
        }

        public static long? GetConvetInt64(this IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return null;
            }
            var reval = dr.GetInt64(i);
            return reval;
        }
        public static Nullable<T> GetOtherNull<T>(this IDataReader dr, int i) where T : struct
        {
            if (dr.IsDBNull(i))
            {
                return null;
            }
            return (T)Convert.ChangeType(dr.GetValue(i), typeof(T));

        }
        public static T GetOther<T>(this IDataReader dr, int i) 
        {
            return (T)Convert.ChangeType(dr.GetValue(i), typeof(T));
        }
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
