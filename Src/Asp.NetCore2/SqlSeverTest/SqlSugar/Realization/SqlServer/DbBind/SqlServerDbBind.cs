using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace SqlSugar
{
    public class SqlServerDbBind : DbBindProvider
    {
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
                  new KeyValuePair<string, CSharpDataType>("int",CSharpDataType.@int),
                  new KeyValuePair<string, CSharpDataType>("varchar",CSharpDataType.@string),
                  new KeyValuePair<string, CSharpDataType>("nvarchar",CSharpDataType.@string),
                  new KeyValuePair<string, CSharpDataType>("sql_variant",CSharpDataType.@string),
                  new KeyValuePair<string, CSharpDataType>("text",CSharpDataType.@string),
                  new KeyValuePair<string, CSharpDataType>("char",CSharpDataType.@string),
                  new KeyValuePair<string, CSharpDataType>("ntext",CSharpDataType.@string),
                  new KeyValuePair<string, CSharpDataType>("nchar",CSharpDataType.@string),
                  new KeyValuePair<string, CSharpDataType>("hierarchyid",CSharpDataType.@string),
                  new KeyValuePair<string, CSharpDataType>("bigint",CSharpDataType.@long),
                  new KeyValuePair<string, CSharpDataType>("bit",CSharpDataType.@bool),
                  new KeyValuePair<string, CSharpDataType>("datetime",CSharpDataType.DateTime),
                  new KeyValuePair<string, CSharpDataType>("time",CSharpDataType.DateTime),
                  new KeyValuePair<string, CSharpDataType>("smalldatetime",CSharpDataType.DateTime),
                  new KeyValuePair<string, CSharpDataType>("timestamp",CSharpDataType.byteArray),
                  new KeyValuePair<string, CSharpDataType>("datetime2",CSharpDataType.DateTime),
                  new KeyValuePair<string, CSharpDataType>("date",CSharpDataType.DateTime),
                  new KeyValuePair<string, CSharpDataType>("decimal",CSharpDataType.@decimal),
                  new KeyValuePair<string, CSharpDataType>("single",CSharpDataType.@decimal),
                  new KeyValuePair<string, CSharpDataType>("money",CSharpDataType.@decimal),
                  new KeyValuePair<string, CSharpDataType>("numeric",CSharpDataType.@decimal),
                  new KeyValuePair<string, CSharpDataType>("smallmoney",CSharpDataType.@decimal),
                  new KeyValuePair<string, CSharpDataType>("float",CSharpDataType.@double),
                  new KeyValuePair<string, CSharpDataType>("float",CSharpDataType.Single),
                  new KeyValuePair<string, CSharpDataType>("real",CSharpDataType.@float),
                  new KeyValuePair<string, CSharpDataType>("smallint",CSharpDataType.@short),
                  new KeyValuePair<string, CSharpDataType>("tinyint",CSharpDataType.@byte),
                  new KeyValuePair<string, CSharpDataType>("uniqueidentifier",CSharpDataType.Guid),
                  new KeyValuePair<string, CSharpDataType>("binary",CSharpDataType.byteArray),
                  new KeyValuePair<string, CSharpDataType>("image",CSharpDataType.byteArray),
                  new KeyValuePair<string, CSharpDataType>("varbinary",CSharpDataType.byteArray),
                  new KeyValuePair<string, CSharpDataType>("datetimeoffset", CSharpDataType.DateTimeOffset),
                  new KeyValuePair<string, CSharpDataType>("datetimeoffset", CSharpDataType.DateTime)};
    };

}
