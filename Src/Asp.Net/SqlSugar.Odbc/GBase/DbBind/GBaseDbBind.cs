using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace SqlSugar.Odbc
{
    public class OdbcDbBind : DbBindProvider
    {
        public override string GetDbTypeName(string csharpTypeName)
        {
            if (csharpTypeName == nameof(DateTimeOffset))
            {
                return nameof(DateTimeOffset);
            }
            else
            {
                return base.GetDbTypeName(csharpTypeName);
            }
        }
        public override List<KeyValuePair<string, CSharpDataType>> MappingTypes
        {
            get
            {
                var extService = this.Context.CurrentConnectionConfig.ConfigureExternalServices;
                if (extService != null&& extService.AppendDataReaderTypeMappings.HasValue())
                {
                    return extService.AppendDataReaderTypeMappings.Union(MappingTypesConst).ToList();
                }
                else
                {
                    return MappingTypesConst;
                }
            }
        }
        public static List<KeyValuePair<string, CSharpDataType>> MappingTypesConst = new List<KeyValuePair<string, CSharpDataType>>()
                {
                  new KeyValuePair<string, CSharpDataType>("bigint",CSharpDataType.@long),
                  new KeyValuePair<string, CSharpDataType>("blob",CSharpDataType.byteArray),
                  new KeyValuePair<string, CSharpDataType>("boolean",CSharpDataType.@bool),
                  new KeyValuePair<string, CSharpDataType>("byte",CSharpDataType.@byte),
                  new KeyValuePair<string, CSharpDataType>("varchar", CSharpDataType.@string),
                  new KeyValuePair<string, CSharpDataType>("char",CSharpDataType.@string),
                  new KeyValuePair<string, CSharpDataType>("clob",CSharpDataType.@string),
                  new KeyValuePair<string, CSharpDataType>("DATETIME YEAR TO FRACTION(3)", CSharpDataType.DateTime),
                  new KeyValuePair<string, CSharpDataType>("datetime", CSharpDataType.DateTime),
                  new KeyValuePair<string, CSharpDataType>("date", CSharpDataType.DateTime),
                  new KeyValuePair<string, CSharpDataType>("decimal", CSharpDataType.@decimal),
                  new KeyValuePair<string, CSharpDataType>("float", CSharpDataType.@float),
                  new KeyValuePair<string, CSharpDataType>("int", CSharpDataType.@int),
                  new KeyValuePair<string, CSharpDataType>("integer", CSharpDataType.@int),
                  new KeyValuePair<string, CSharpDataType>("money", CSharpDataType.@decimal),
                  new KeyValuePair<string, CSharpDataType>("nchar", CSharpDataType.@string),
                  new KeyValuePair<string, CSharpDataType>("numeric", CSharpDataType.@decimal),
                  new KeyValuePair<string, CSharpDataType>("nvarchar", CSharpDataType.@string),
                  new KeyValuePair<string, CSharpDataType>("varchar", CSharpDataType.@string),
                  new KeyValuePair<string, CSharpDataType>("text", CSharpDataType.@string),
                  new KeyValuePair<string, CSharpDataType>("smallfloat", CSharpDataType.@decimal),
                  new KeyValuePair<string, CSharpDataType>("serial", CSharpDataType.@int),
        };
    };

}
