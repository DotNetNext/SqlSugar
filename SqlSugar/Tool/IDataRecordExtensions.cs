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

        public static Nullable<TEnum> GetConvertToEnum_Nullable<TEnum>(this IDataRecord dr, int i) where TEnum : struct
        {
            TEnum ret = GetConvertToEnum<TEnum>(dr, i);
            return new Nullable<TEnum>(ret);
        }

        public static TEnum GetConvertToEnum<TEnum>(this IDataRecord dr, int i) where TEnum : struct
        {
            Type t = typeof(TEnum);
            TEnum ret = (TEnum)Enum.ToObject(t, dr.GetValue(i));
            return ret;
        }

    }
}
