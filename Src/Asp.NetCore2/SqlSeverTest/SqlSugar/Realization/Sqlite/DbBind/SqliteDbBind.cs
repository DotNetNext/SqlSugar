using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlSugar
{
    public class SqliteDbBind : DbBindProvider
    {
        public override string GetDbTypeName(string csharpTypeName)
        {
            if (csharpTypeName == UtilConstants.ByteArrayType.Name)
            {
                return "blob";
            }
            if (csharpTypeName == "Int32")
                csharpTypeName = "int";
            if (csharpTypeName == "Int16")
                csharpTypeName = "short";
            if (csharpTypeName == "Int64")
                csharpTypeName = "long";
            if (csharpTypeName == "Boolean")
                csharpTypeName = "bool";
            var mappings = this.MappingTypes.Where(it => it.Value.ToString().Equals(csharpTypeName, StringComparison.CurrentCultureIgnoreCase));
            return mappings.HasValue() ? mappings.First().Key : "varchar";
        }      
        public override List<KeyValuePair<string, CSharpDataType>> MappingTypes
        {
            get
            {
                var extService = this.Context.CurrentConnectionConfig.ConfigureExternalServices;
                if (extService != null && extService.AppendDataReaderTypeMappings.HasValue())
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

                    new KeyValuePair<string, CSharpDataType>("integer",CSharpDataType.@int),
                    new KeyValuePair<string, CSharpDataType>("int",CSharpDataType.@int),
                    new KeyValuePair<string, CSharpDataType>("int32",CSharpDataType.@int),
                    new KeyValuePair<string, CSharpDataType>("integer32",CSharpDataType.@int),
                    new KeyValuePair<string, CSharpDataType>("number",CSharpDataType.@int),

                    new KeyValuePair<string, CSharpDataType>("varchar",CSharpDataType.@string),
                    new KeyValuePair<string, CSharpDataType>("varchar2",CSharpDataType.@string),
                    new KeyValuePair<string, CSharpDataType>("nvarchar",CSharpDataType.@string),
                    new KeyValuePair<string, CSharpDataType>("nvarchar2",CSharpDataType.@string),
                    new KeyValuePair<string, CSharpDataType>("text",CSharpDataType.@string),
                    new KeyValuePair<string, CSharpDataType>("ntext",CSharpDataType.@string),
                    new KeyValuePair<string, CSharpDataType>("blob_text",CSharpDataType.@string),
                    new KeyValuePair<string, CSharpDataType>("char",CSharpDataType.@string),
                    new KeyValuePair<string, CSharpDataType>("nchar",CSharpDataType.@string),
                    new KeyValuePair<string, CSharpDataType>("num",CSharpDataType.@string),
                    new KeyValuePair<string, CSharpDataType>("currency",CSharpDataType.@string),
                    new KeyValuePair<string, CSharpDataType>("datetext",CSharpDataType.@string),
                    new KeyValuePair<string, CSharpDataType>("word",CSharpDataType.@string),
                    new KeyValuePair<string, CSharpDataType>("graphic",CSharpDataType.@string),

                    new KeyValuePair<string, CSharpDataType>("tinyint",CSharpDataType.@byte),
                    new KeyValuePair<string, CSharpDataType>("unsignedinteger8",CSharpDataType.@byte),
                    new KeyValuePair<string, CSharpDataType>("smallint",CSharpDataType.@short),
                    new KeyValuePair<string, CSharpDataType>("int16",CSharpDataType.@short),
                    new KeyValuePair<string, CSharpDataType>("bigint",CSharpDataType.@long),
                    new KeyValuePair<string, CSharpDataType>("int64",CSharpDataType.@long),
                    new KeyValuePair<string, CSharpDataType>("long",CSharpDataType.@long),
                    new KeyValuePair<string, CSharpDataType>("integer64",CSharpDataType.@long),
                    new KeyValuePair<string, CSharpDataType>("bit",CSharpDataType.@bool),
                    new KeyValuePair<string, CSharpDataType>("bool",CSharpDataType.@bool),
                    new KeyValuePair<string, CSharpDataType>("boolean",CSharpDataType.@bool),
                    new KeyValuePair<string, CSharpDataType>("real",CSharpDataType.@double),
                    new KeyValuePair<string, CSharpDataType>("double",CSharpDataType.@double),
                    new KeyValuePair<string, CSharpDataType>("float",CSharpDataType.@float),
                    new KeyValuePair<string, CSharpDataType>("float",CSharpDataType.Single),
                    new KeyValuePair<string, CSharpDataType>("decimal",CSharpDataType.@decimal),
                    new KeyValuePair<string, CSharpDataType>("dec",CSharpDataType.@decimal),
                    new KeyValuePair<string, CSharpDataType>("numeric",CSharpDataType.@decimal),
                    new KeyValuePair<string, CSharpDataType>("money",CSharpDataType.@decimal),
                    new KeyValuePair<string, CSharpDataType>("smallmoney",CSharpDataType.@decimal),

                    new KeyValuePair<string, CSharpDataType>("datetime",CSharpDataType.DateTime),
                    new KeyValuePair<string, CSharpDataType>("timestamp",CSharpDataType.DateTime),
                    new KeyValuePair<string, CSharpDataType>("date",CSharpDataType.DateTime),
                    new KeyValuePair<string, CSharpDataType>("time",CSharpDataType.DateTime),

                    new KeyValuePair<string, CSharpDataType>("blob",CSharpDataType.byteArray),
                    new KeyValuePair<string, CSharpDataType>("clob",CSharpDataType.byteArray),
                    new KeyValuePair<string, CSharpDataType>("raw",CSharpDataType.byteArray),
                    new KeyValuePair<string, CSharpDataType>("oleobject",CSharpDataType.byteArray),
                    new KeyValuePair<string, CSharpDataType>("binary",CSharpDataType.byteArray),
                    new KeyValuePair<string, CSharpDataType>("photo",CSharpDataType.byteArray),
                    new KeyValuePair<string, CSharpDataType>("picture",CSharpDataType.byteArray),

                    new KeyValuePair<string, CSharpDataType>("uniqueidentifier",CSharpDataType.Guid),
                    new KeyValuePair<string, CSharpDataType>("varchar",CSharpDataType.Guid),
                    new KeyValuePair<string, CSharpDataType>("guid",CSharpDataType.Guid)
         };
        public override List<string> StringThrow
        {
            get
            {
                return new List<string>() { "int32", "datetime", "decimal", "double", "byte"};
            }
        }
    }
}
