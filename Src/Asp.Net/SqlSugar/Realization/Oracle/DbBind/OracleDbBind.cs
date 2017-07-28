using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace SqlSugar
{
    public class OracleDbBind : DbBindProvider
    {
        public override List<KeyValuePair<string, CSharpDataType>> MappingTypes
        {
            get
            {
                return new List<KeyValuePair<string, CSharpDataType>>()
                {
                  new KeyValuePair<string, CSharpDataType>("int",CSharpDataType.@int),
                  new KeyValuePair<string, CSharpDataType>("integer",CSharpDataType.@int),
                  new KeyValuePair<string, CSharpDataType>("interval year to  month",CSharpDataType.@int),
                  new KeyValuePair<string, CSharpDataType>("interval day to  second",CSharpDataType.@int),

                  new KeyValuePair<string, CSharpDataType>("number_int",CSharpDataType.@int),
                  new KeyValuePair<string, CSharpDataType>("number_float",CSharpDataType.@float),
                  new KeyValuePair<string, CSharpDataType>("number_short",CSharpDataType.@short),
                  new KeyValuePair<string, CSharpDataType>("number_byte",CSharpDataType.@byte),
                  new KeyValuePair<string, CSharpDataType>("number_double",CSharpDataType.@double),
                  new KeyValuePair<string, CSharpDataType>("number_long",CSharpDataType.@long),
                  new KeyValuePair<string, CSharpDataType>("number_bool",CSharpDataType.@bool),

                  new KeyValuePair<string, CSharpDataType>("varchar",CSharpDataType.@string),
                  new KeyValuePair<string, CSharpDataType>("varchar2",CSharpDataType.@string),
                  new KeyValuePair<string, CSharpDataType>("nvarchar2",CSharpDataType.@string),
                  new KeyValuePair<string, CSharpDataType>("char",CSharpDataType.@string),
                  new KeyValuePair<string, CSharpDataType>("nchar",CSharpDataType.@string),
                  new KeyValuePair<string, CSharpDataType>("clob",CSharpDataType.@string),
                  new KeyValuePair<string, CSharpDataType>("long",CSharpDataType.@string),
                  new KeyValuePair<string, CSharpDataType>("nclob",CSharpDataType.@string),
                  new KeyValuePair<string, CSharpDataType>("rowid",CSharpDataType.@string),              

                  new KeyValuePair<string, CSharpDataType>("date",CSharpDataType.DateTime),
                  new KeyValuePair<string, CSharpDataType>("timestamp",CSharpDataType.DateTime),
                  new KeyValuePair<string, CSharpDataType>("timestamp with local time zone",CSharpDataType.DateTime),
                  new KeyValuePair<string, CSharpDataType>("timestamp with time zone",CSharpDataType.DateTime),
                  new KeyValuePair<string, CSharpDataType>("timestamp with time zone",CSharpDataType.DateTime),

                  new KeyValuePair<string, CSharpDataType>("float",CSharpDataType.@decimal),

                  new KeyValuePair<string, CSharpDataType>("blob",CSharpDataType.byteArray),
                  new KeyValuePair<string, CSharpDataType>("long raw",CSharpDataType.byteArray),
                  new KeyValuePair<string, CSharpDataType>("raw",CSharpDataType.byteArray),
                  new KeyValuePair<string, CSharpDataType>("bfile",CSharpDataType.byteArray),
                  new KeyValuePair<string, CSharpDataType>("varbinary",CSharpDataType.byteArray) };

            }
        }
    }
}
