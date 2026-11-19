using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;

namespace SqlSugar.IoTDB
{
    public sealed class IoTDBDbBind : DbBindProvider
    {
        private static readonly List<KeyValuePair<string, CSharpDataType>> Types = new List<KeyValuePair<string, CSharpDataType>>
        {
            new KeyValuePair<string, CSharpDataType>("BOOLEAN", CSharpDataType.@bool),
            new KeyValuePair<string, CSharpDataType>("INT32", CSharpDataType.@int),
            new KeyValuePair<string, CSharpDataType>("INT64", CSharpDataType.@long),
            new KeyValuePair<string, CSharpDataType>("FLOAT", CSharpDataType.Single),
            new KeyValuePair<string, CSharpDataType>("DOUBLE", CSharpDataType.@double),
            new KeyValuePair<string, CSharpDataType>("TEXT", CSharpDataType.@string),
            new KeyValuePair<string, CSharpDataType>("STRING", CSharpDataType.@string),
            new KeyValuePair<string, CSharpDataType>("BLOB", CSharpDataType.byteArray),
            new KeyValuePair<string, CSharpDataType>("TIMESTAMP", CSharpDataType.DateTime),
            new KeyValuePair<string, CSharpDataType>("DATE", CSharpDataType.DateTime)
        };

        public override List<KeyValuePair<string, CSharpDataType>> MappingTypes
        {
            get
            {
                var additional = this.Context.CurrentConnectionConfig.ConfigureExternalServices?.AppendDataReaderTypeMappings;
                return additional != null && additional.Count > 0 ? additional.Union(Types).ToList() : Types;
            }
        }

        public override string GetDbTypeName(string csharpTypeName)
        {
            var type = csharpTypeName?.ToLowerInvariant();
            if (type == "string" || type == "char") return "TEXT";
            if (type == "bool" || type == "boolean") return "BOOLEAN";
            if (type == "byte" || type == "sbyte" || type == "int16" || type == "uint16" || type == "int32" || type == "uint32" || type == "int") return "INT32";
            if (type == "int64" || type == "uint64" || type == "long") return "INT64";
            if (type == "single" || type == "float") return "FLOAT";
            if (type == "double" || type == "decimal") return "DOUBLE";
            if (type == "datetime" || type == "datetimeoffset") return "TIMESTAMP";
            if (type == "byte[]") return "BLOB";
            if (type == "dateonly") return "DATE";
            return "TEXT";
        }

        public override string GetPropertyTypeName(string dbTypeName)
        {
            var type = MappingTypes.FirstOrDefault(it => it.Key.Equals(dbTypeName, StringComparison.OrdinalIgnoreCase));
            if (type.Key != null) return type.Value.ToString();
            return base.GetPropertyTypeName(dbTypeName);
        }

        public override List<string> StringThrow => new List<string> { "int32", "int64", "datetime", "decimal", "double" };
    }
}
