﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
namespace SqlSugar
{
    public static partial class IDataRecordExtensions
    {

        #region Common Extensions
        public static Guid GetStringGuid(this IDataRecord dr, int i)
        {
            var result = Guid.Parse(dr.GetValue(i).ToString());
            return result;
        }

        public static Guid? GetConvertStringGuid(this IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return Guid.Empty;
            }
            var result = Guid.Parse(dr.GetValue(i).ToString());
            return result;
        }

        public static bool? GetConvertBoolean(this IDataRecord dr, int i)
        {
            var result = dr.GetBoolean(i);
            return result;
        }

        public static byte? GetConvertByte(this IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return null;
            }
            var result = dr.GetByte(i);
            return result;
        }

        public static char? GetConvertChar(this IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return null;
            }
            var result = dr.GetChar(i);
            return result;
        }

        public static DateTime? GetConvertDateTime(this IDataRecord dr, int i)
        {
            var value = dr.GetValue(i);

            if (value == DBNull.Value)
            {
                return null;
            }

            if (value is DateTime result)
            {
                if (result == DateTime.MinValue)
                {
                    return null;
                }

                return result;
            }

            return Convert.ToDateTime(value.ToString());
        }

        public static DateTime? GetConvertTime(this IDataRecord dr, int i)
        {
            var result = dr.GetValue(i);
            if (result == DBNull.Value)
            {
                return null; ;
            }
            return Convert.ToDateTime(result.ToString());
        }
        public static DateTime GetTime(this IDataRecord dr, int i)
        {
            return Convert.ToDateTime(dr.GetValue(i).ToString());
        }

        public static decimal? GetConvertDecimal(this IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return null;
            }
            var result = dr.GetDecimal(i);
            return result;
        }


        public static double? GetConvertDouble(this IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return null;
            }
            var result = dr.GetDouble(i);
            return result;
        }


        public static float? GetConvertDoubleToFloat(this IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return null;
            }
            var result = dr.GetDouble(i);
            return Convert.ToSingle(result);
        }


        public static Guid? GetConvertGuid(this IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return null;
            }
            var result = dr.GetGuid(i);
            return result;
        }

        public static short? GetConvertInt16(this IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return null;
            }
            var result = dr.GetInt16(i);
            return result;
        }

        public static Int32? GetConvertInt32(this IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return null;
            }
            var result = dr.GetInt32(i);
            return result;
        }
        public static T GetConvertValue<T>(this IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return default(T);
            }
            var result = dr.GetValue(i);
            return UtilMethods.To<T>(result);
        }

        public static long? GetConvetInt64(this IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return null;
            }
            var result = dr.GetInt64(i);
            return result;
        }

        public static float? GetConvertFloat(this IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return null;
            }
            var result = dr.GetFloat(i);
            return result;
        }

        public static DateTime GetdatetimeoffsetDate(this IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return DateTime.MinValue;
            }
            var offsetValue = (DateTimeOffset)dr.GetValue(i);
            var result = offsetValue.DateTime;
            return result;
        }

        public static DateTime? GetConvertdatetimeoffsetDate(this IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return DateTime.MinValue;
            }
            var offsetValue = (DateTimeOffset)dr.GetValue(i);
            var result = offsetValue.DateTime;
            return result;
        }

        public static DateTimeOffset Getdatetimeoffset(this IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return default(DateTimeOffset);
            }
            var result = (DateTimeOffset)dr.GetValue(i);
            return result;
        }

        public static DateTimeOffset? GetConvertdatetimeoffset(this IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return default(DateTimeOffset);
            }
            var result = (DateTimeOffset)dr.GetValue(i);
            return result;
        }


        public static string GetConvertString(this IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return null;
            }
            var result = Convert.ToString(dr.GetValue(i));
            return result;
        }

        public static Nullable<T> GetOtherNull<T>(this IDataReader dr, int i) where T : struct
        {
            if (dr.IsDBNull(i))
            {
                return null;
            }
            var result = dr.GetValue(i);
            return UtilMethods.To<T>(result);

        }

        public static T GetOther<T>(this IDataReader dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return default(T);
            }
            var result = dr.GetValue(i);
            return UtilMethods.To<T>(result);
        }

        public static T GetJson<T>(this IDataReader dr, int i)
        {
            var obj = dr.GetValue(i);
            if (obj == null)
                return default(T);
            var value = obj.ObjToString();
            return new SerializeService().DeserializeObject<T>(value);
        }

        public static Nullable<T> GetConvertEnum_Null<T>(this IDataReader dr, int i) where T : struct
        {
            if (dr.IsDBNull(i))
            {
                return null;
            }
            object value = dr.GetValue(i);
            T t = (T)Enum.ToObject(typeof(T), value);
            return t;
        }

        public static T GetEnum<T>(this IDataReader dr, int i) where T : struct
        {
            object value = dr.GetValue(i);
            if (value != null)
            {
                if (value.GetType() == UtilConstants.DecType)
                {
                    value = Convert.ToUInt32(value);
                }
                else if (value.GetType() == UtilConstants.StringType)
                {
                    return (T)Enum.Parse(typeof(T), value.ObjToString());
                }
            }
            T t = (T)Enum.ToObject(typeof(T), value);
            return t;
        }

        public static object GetEntity(this IDataReader dr, SqlSugarProvider context)
        {
            return null;
        }

        #endregion
    }
}
