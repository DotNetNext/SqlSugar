using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar.DbConvert
{
    public  class EnumToStringConvert: ISugarDataConverter
    {
        public  SugarParameter ParameterConverter<T>(object columnValue, int columnIndex)
        {
            var name = "@MyEnmu" + columnIndex;
            Type undertype = SqlSugar.UtilMethods.GetUnderType(typeof(T));//获取没有nullable的枚举类型
            if (columnValue == null)
            {
                return new SugarParameter(name, null);
            }
            else
            {
                var enumObjString = Enum.Parse(undertype, columnValue + "").ToString();
                return new SugarParameter(name, enumObjString);
            }
        }

        public  T QueryConverter<T>(IDataRecord dr, int i)
        {

            var str = dr.GetString(i);
            Type undertype = SqlSugar.UtilMethods.GetUnderType(typeof(T));//获取没有nullable的枚举类型
            return (T)Enum.Parse(undertype, str);
        }
    }


    public class CommonPropertyConvert : ISugarDataConverter
    {
        public SugarParameter ParameterConverter<T>(object columnValue, int columnIndex)
        {
            var name = "@Common" + columnIndex;
            Type undertype = SqlSugar.UtilMethods.GetUnderType(typeof(T));//获取没有nullable的枚举类型
            return new SugarParameter(name, columnValue, undertype);
        }

        public T QueryConverter<T>(IDataRecord dr, int i)
        {

            var value = dr.GetValue(i);
            return (T)UtilMethods.ChangeType2(value, typeof(T));
        }
    }
}
