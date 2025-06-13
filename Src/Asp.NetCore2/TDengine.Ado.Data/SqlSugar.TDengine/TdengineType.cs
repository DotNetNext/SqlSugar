using System;
using System.Collections.Generic;
using System.Text;
namespace SqlSugar.TDengineAdo
{
    internal class TDengineType
    {
         
        public static object GetObjectByType(object dataValue, byte typeValue)
        {
            if (dataValue is string &&dataValue?.ToString().ToUpper() == "NULL") 
            {
                return null;
            }
            switch (typeValue)
            {
                case 1: // BOOL
                    return Convert.ToBoolean(dataValue);
                case 2: // TINYINT
                    return Convert.ToSByte(dataValue);
                case 3: // SMALLINT
                    return Convert.ToInt16(dataValue);
                case 4: // INT
                    return Convert.ToInt32(dataValue);
                case 5: // BIGINT
                    return Convert.ToInt64(dataValue);
                case 11: // TINYINT UNSIGNED
                    return Convert.ToByte(dataValue);
                case 12: // SMALLINT UNSIGNED
                    return Convert.ToUInt16(dataValue);
                case 13: // INT UNSIGNED
                    return Convert.ToUInt32(dataValue);
                case 14: // BIGINT UNSIGNED
                    return Convert.ToUInt64(dataValue);
                case 6: // FLOAT
                    return Convert.ToSingle(dataValue);
                case 7: // DOUBLE
                    return Convert.ToDouble(dataValue);
                case 8: // BINARY
                    return dataValue?.ToString();
                case 9: // TIMESTAMP 
                    if (dataValue?.ToString()?.Length == 16)
                    {
                        var dt = Helper.Long16ToDateTime(Convert.ToInt64(dataValue));
                        return dt;
                    }
                    else if (dataValue?.ToString()?.Length == 19)
                    {
                        var dt = Helper.Long19ToDateTime(Convert.ToInt64(dataValue));
                        return dt;
                    }
                    else
                    {
                        var dt = DateTimeOffset.FromUnixTimeMilliseconds(Convert.ToInt64(dataValue)).DateTime.ToLocalTime();
                        return dt;
                    }
                case 10: // NCHAR
                case 15: // JSON
                    return dataValue?.ToString();
                default:
                    return dataValue; // Return as is if the type is not recognized
            }
        }
        public static string GetDataTypeName(byte typeValue)
        {
            return typeValue switch
            {
                1 => typeof(bool).Name,
                2 => typeof(sbyte).Name,
                3 => typeof(short).Name,
                4 => typeof(int).Name,
                5 => typeof(long).Name,
                11 => typeof(byte).Name,
                12 => typeof(ushort).Name,
                13 => typeof(uint).Name,
                14 => typeof(ulong).Name,
                6 => typeof(float).Name,
                7 => typeof(double).Name,
                8 => typeof(string).Name,
                9 => typeof(DateTime).Name,
                10 => typeof(string).Name,
                15 => typeof(string).Name, // Assuming JSON is represented as a string
                _ => typeof(string).Name,
            };
        }
        public static Type GetDataType(int typeValue)
        {
            return typeValue switch
            {
                1 => typeof(bool),
                2 => typeof(sbyte),
                3 => typeof(short),
                4 => typeof(int),
                5 => typeof(long),
                11 => typeof(byte),
                12 => typeof(ushort),
                13 => typeof(uint),
                14 => typeof(ulong),
                6 => typeof(float),
                7 => typeof(double),
                8 => typeof(string),
                9 => typeof(DateTime),
                10 => typeof(string),
                15 => typeof(string), // Assuming JSON is represented as a string
                _ => typeof(string),
            };
        }
    }
}
