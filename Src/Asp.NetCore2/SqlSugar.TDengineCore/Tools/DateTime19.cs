using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SqlSugar 
{
    internal enum Date19 
    {
        time=19
    }
    public class DateTime19: ISugarDataConverter
    {
        public SugarParameter ParameterConverter<T>(object columnValue, int columnIndex)
        {
            var name = "@Common" + columnIndex;
            Type undertype = SqlSugar.UtilMethods.GetUnderType(typeof(T));//获取没有nullable的枚举类型
            return new SugarParameter(name, columnValue, undertype) { CustomDbType= typeof(Date19) };
        }

        public T QueryConverter<T>(IDataRecord dr, int i)
        {

            var value = dr.GetValue(i);
            return (T)UtilMethods.ChangeType2(value, typeof(T));
        }
    }
}
