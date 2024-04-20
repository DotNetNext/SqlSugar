using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Text;

namespace SqlSugar 
{
    public class CsvHelperEnumToIntConverter : ITypeConverter
    {
        public string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            if (value == null) 
            {
                return "null";
            }
            else if (value is Enum enumValue)
            {
                return (Convert.ToInt32(enumValue)).ToString();
            }
            throw new NotSupportedException();
        }

        public object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (int.TryParse(text, out int intValue))
            {
                return text;
            }
            throw new NotSupportedException();
        }
    }
}
