using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class EnumMethod
    {
        public static Nullable<TEnum> ConvertToEnum_Nullable<TEnum>(object val) where TEnum : struct
        {
            TEnum ret = ConvertToEnum<TEnum>(val);
            return new Nullable<TEnum>(ret);
        }
        public static TEnum ConvertToEnum<TEnum>(object val) where TEnum : struct
        {
            Type t = typeof(TEnum);
            TEnum ret = (TEnum)Enum.ToObject(t, val);
            return ret;
        }

        public static bool IsNullable(Type type)
        {
            Type unType;
            return IsNullable(type, out unType);
        }
        public static bool IsNullable(Type type, out Type unType)
        {
            unType = Nullable.GetUnderlyingType(type);
            return unType != null;
        }
    }
}
