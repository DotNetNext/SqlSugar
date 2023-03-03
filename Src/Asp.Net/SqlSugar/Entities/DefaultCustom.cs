using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar.DbConvert
{
    public static class EnumToStringConvert
    {
        public static SugarParameter ParameterConverter<T>(object value, int i)
        {
            var name = "@myenmu" + i;
            Type undertype = SqlSugar.UtilMethods.GetUnderType(typeof(T));//获取没有nullable的枚举类型
            if (value == null)
            {
                return new SugarParameter(name, null);
            }
            else
            {
                var enumObjString = Enum.Parse(undertype, value + "").ToString();
                return new SugarParameter(name, enumObjString);
            }
        }

        public static T QueryConverter<T>(this IDataRecord dr, int i)
        {

            var str = dr.GetString(i);
            Type undertype = SqlSugar.UtilMethods.GetUnderType(typeof(T));//获取没有nullable的枚举类型
            return (T)Enum.Parse(undertype, str);
        }
    }
}
