using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlSugar.SonnetDB
{
    public sealed class SonnetDBDbBind : DbBindProvider
    {
        public static readonly List<KeyValuePair<string, CSharpDataType>> MappingTypesConst =
            new List<KeyValuePair<string, CSharpDataType>>
            {
                new KeyValuePair<string, CSharpDataType>("INT", CSharpDataType.@long),
                new KeyValuePair<string, CSharpDataType>("FLOAT", CSharpDataType.@double),
                new KeyValuePair<string, CSharpDataType>("DECIMAL", CSharpDataType.@decimal),
                new KeyValuePair<string, CSharpDataType>("NUMERIC", CSharpDataType.@decimal),
                new KeyValuePair<string, CSharpDataType>("BOOL", CSharpDataType.@bool),
                new KeyValuePair<string, CSharpDataType>("STRING", CSharpDataType.@string),
                new KeyValuePair<string, CSharpDataType>("DATETIME", CSharpDataType.DateTime),
                new KeyValuePair<string, CSharpDataType>("TIME", CSharpDataType.TimeSpan),
                new KeyValuePair<string, CSharpDataType>("BLOB", CSharpDataType.byteArray),
                new KeyValuePair<string, CSharpDataType>("JSON", CSharpDataType.@string)
            };

        public override List<KeyValuePair<string, CSharpDataType>> MappingTypes
        {
            get
            {
                var externalMappings = Context.CurrentConnectionConfig.ConfigureExternalServices?.AppendDataReaderTypeMappings;
                return externalMappings != null && externalMappings.Count > 0
                    ? externalMappings.Union(MappingTypesConst).ToList()
                    : MappingTypesConst;
            }
        }

        public override string GetDbTypeName(string csharpTypeName)
        {
            return csharpTypeName switch
            {
                "Byte[]" => "BLOB",
                "Int16" or "Int32" or "Int64" or "Byte" or "SByte" or "UInt16" or "UInt32" or "UInt64" => "INT",
                "Single" or "Double" => "FLOAT",
                "Decimal" => "DECIMAL",
                "Boolean" => "BOOL",
                "DateTime" or "DateTimeOffset" or "DateOnly" => "DATETIME",
                "TimeOnly" or "TimeSpan" => "TIME",
                "Guid" => "STRING",
                _ => "STRING"
            };
        }

        public override string GetConvertString(string dbTypeName)
        {
            return dbTypeName.ToUpperInvariant() switch
            {
                "INT" => "Convert.ToInt64",
                "FLOAT" => "Convert.ToDouble",
                "DECIMAL" or "NUMERIC" => "Convert.ToDecimal",
                "BOOL" => "Convert.ToBoolean",
                "STRING" or "JSON" => "Convert.ToString",
                "DATETIME" => "Convert.ToDateTime",
                "TIME" => "TimeOnly.Parse",
                _ => base.GetConvertString(dbTypeName)
            };
        }

    }
}
