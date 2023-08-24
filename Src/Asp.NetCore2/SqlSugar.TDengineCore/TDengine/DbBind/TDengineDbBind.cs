using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlSugar.TDengine
{
    public class TDengineDbBind : DbBindProvider
    {
        public override string GetDbTypeName(string csharpTypeName)
        {
            if (csharpTypeName == UtilConstants.ByteArrayType.Name)
                return "bytea";
            if (csharpTypeName.ToLower() == "int32")
                csharpTypeName = "int";
            if (csharpTypeName.ToLower() == "int16")
                csharpTypeName = "short";
            if (csharpTypeName.ToLower() == "int64")
                csharpTypeName = "long";
            if (csharpTypeName.ToLower().IsIn("boolean", "bool"))
                csharpTypeName = "bool";
            if (csharpTypeName == "DateTimeOffset")
                csharpTypeName = "DateTime";
            var mappings = this.MappingTypes.Where(it => it.Value.ToString().Equals(csharpTypeName, StringComparison.CurrentCultureIgnoreCase)).ToList();
            if (mappings != null && mappings.Count > 0)
                return mappings.First().Key;
            else
                return "varchar";
        }
        public override string GetPropertyTypeName(string dbTypeName)
        {
            if (dbTypeName.ToLower() == "int32")
            {
                dbTypeName = "int";
            }
            else if (dbTypeName.ToLower() == "int16")
            {
                dbTypeName = "short";
            }
            else if (dbTypeName.ToLower() == "int64")
            {
                dbTypeName = "long";
            }
            else if (dbTypeName.ToLower() == "string")
            {
                dbTypeName = "string";
            }
            else if (dbTypeName.ToLower() == "boolean")
            {
                dbTypeName = "bool";
            }
            else if (dbTypeName.ToLower() == "sbyte") 
            {
                dbTypeName = "sbyte";
            }
            else if (dbTypeName.ToLower() == "double")
            {
                dbTypeName = "double";
            }
            return dbTypeName;
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
        public static List<KeyValuePair<string, CSharpDataType>> MappingTypesConst = new List<KeyValuePair<string, CSharpDataType>>(){

                    new KeyValuePair<string, CSharpDataType>("BOOL",CSharpDataType.@bool),
                    new KeyValuePair<string, CSharpDataType>("TINYINT",CSharpDataType.@byte),
                    new KeyValuePair<string, CSharpDataType>("SMALLINT",CSharpDataType.@short),
                    new KeyValuePair<string, CSharpDataType>("INT",CSharpDataType.@int),
                    new KeyValuePair<string, CSharpDataType>("BIGINT",CSharpDataType.@long),
                    new KeyValuePair<string, CSharpDataType>("TINYINT UNSIGNED",CSharpDataType.@byte),
                    new KeyValuePair<string, CSharpDataType>("SMALLINT UNSIGNED",CSharpDataType.@short),
                    new KeyValuePair<string, CSharpDataType>("INT UNSIGNED",CSharpDataType.@int),
                    new KeyValuePair<string, CSharpDataType>("BIGINT UNSIGNED",CSharpDataType.@long),
                    new KeyValuePair<string, CSharpDataType>("FLOAT",CSharpDataType.Single),
                    new KeyValuePair<string, CSharpDataType>("DOUBLE",CSharpDataType.@double),
                    new KeyValuePair<string, CSharpDataType>("float8",CSharpDataType.@double),
                    new KeyValuePair<string, CSharpDataType>("BINARY",CSharpDataType.@string),
                    new KeyValuePair<string, CSharpDataType>("TIMESTAMP",CSharpDataType.DateTime),
                    new KeyValuePair<string, CSharpDataType>("NCHAR",CSharpDataType.@string),
                    new KeyValuePair<string, CSharpDataType>("JSON",CSharpDataType.@string)
                };
        public override List<string> StringThrow
        {
            get
            {
                return new List<string>() { "int32", "datetime", "decimal", "double", "byte" };
            }
        }
    }
}
