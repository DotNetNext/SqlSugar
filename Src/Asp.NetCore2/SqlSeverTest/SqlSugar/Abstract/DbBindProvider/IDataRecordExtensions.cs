using System;
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
            var reval = Guid.Parse(dr.GetValue(i).ToString());
            return reval;
        }

        public static Guid? GetConvertStringGuid(this IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return Guid.Empty;
            }
            var reval = Guid.Parse(dr.GetValue(i).ToString());
            return reval;
        }

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

        public static float? GetConvertFloat(this IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return null;
            }
            var reval = dr.GetFloat(i);
            return reval;
        }

        public static string GetConvertString(this IDataRecord dr, int i)
        {
            if (dr.IsDBNull(i))
            {
                return null;
            }
            var reval = Convert.ToString(dr.GetValue(i));
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
            T t = (T)Enum.ToObject(typeof(T), value);
            return t;
        }

        public static object GetEntity(this IDataReader dr, SqlSugarClient context)
        {
            return null;
        }

        #endregion

        #region Sqlite Extensions
        public static Nullable<T> GetSqliteTypeNull<T>(this IDataReader dr, int i) where T : struct
        {
            var type = PubMethod.GetUnderType(typeof(T));
            if (dr.IsDBNull(i))
            {
                return null;
            }
            return SqliteTypeConvert<T>(dr, i, type);
        }

        public static T GetSqliteType<T>(this IDataReader dr, int i) where T : struct
        {
            var type = typeof(T);
            return SqliteTypeConvert<T>(dr, i, type);
        }

        private static T SqliteTypeConvert<T>(IDataReader dr, int i, Type type) where T : struct
        {
            if (type.IsIn(PubConst.IntType))
            {
                return (T)((object)(dr.GetInt32(i)));
            }
            else if (type == PubConst.DateType)
            {
                return (T)Convert.ChangeType(Convert.ToDateTime(dr.GetString(i)), type);
            }
            else if (type == PubConst.DecType)
            {
                return (T)Convert.ChangeType(dr.GetDecimal(i), type);
            }
            else if (type == PubConst.DobType)
            {
                return (T)Convert.ChangeType(dr.GetDouble(i), type);
            }
            else if (type == PubConst.BoolType)
            {
                return (T)Convert.ChangeType(dr.GetBoolean(i), type);
            }
            else if (type == PubConst.GuidType)
            {
                string guidString = dr.GetString(i);
                string changeValue = guidString.IsNullOrEmpty() ? Guid.Empty.ToString() : guidString;
                return (T)Convert.ChangeType(Guid.Parse(changeValue), type);
            }
            else
            {
                return (T)Convert.ChangeType((dr.GetString(i)), type);
            }
        } 
        #endregion
    }
}
