using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlSugar.DuckDB
{
    public class DuckDBDbBind : DbBindProvider
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
            dbTypeName = dbTypeName.ToLower();
            var propertyTypes = MappingTypes.Where(it => it.Value.ToString().ToLower() == dbTypeName || it.Key.ToLower() == dbTypeName);
            if (propertyTypes == null)
            {
                return "other";
            }
            else if (dbTypeName == "xml" || dbTypeName == "string"|| dbTypeName == "jsonb"|| dbTypeName == "json")
            {
                return "string";
            }else if (dbTypeName == "bpchar")//数据库char datatype 查询出来的时候是 bpchar
            {
                return "char";
            }
            if (dbTypeName == "byte[]")
            {
                return "byte[]";
            }
            else if (propertyTypes == null || propertyTypes.Count() == 0)
            {
                if (dbTypeName.StartsWith("_"))
                {
                    var dbTypeName2 = dbTypeName.TrimStart('_');
                    return MappingTypes.Where(it => it.Value.ToString().ToLower() == dbTypeName2 || it.Key.ToLower() == dbTypeName2).Select(it => it.Value + "[]").First();
                }
                else if (dbTypeName?.ToLower()?.StartsWith("decimal")==true) 
                {
                    return "decimal";
                }
                Check.ThrowNotSupportedException(string.Format(" \"{0}\" Type NotSupported, DbBindProvider.GetPropertyTypeName error.", dbTypeName));
                return null!;
            }
            else if (propertyTypes.First().Value == CSharpDataType.byteArray)
            {
                return "byte[]";
            }
            else
            {
                return propertyTypes.First().Value.ToString();
            }
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

                    // 整数类型
                new KeyValuePair<string, CSharpDataType>("TINYINT", CSharpDataType.@byte),       // 1字节
                new KeyValuePair<string, CSharpDataType>("SMALLINT", CSharpDataType.@short),    // 2字节
                new KeyValuePair<string, CSharpDataType>("INTEGER", CSharpDataType.@int),       // 4字节
                new KeyValuePair<string, CSharpDataType>("BIGINT", CSharpDataType.@long),       // 8字节
                new KeyValuePair<string, CSharpDataType>("UTINYINT", CSharpDataType.@byte),     // 无符号
                new KeyValuePair<string, CSharpDataType>("USMALLINT", CSharpDataType.@short),  // 无符号
                new KeyValuePair<string, CSharpDataType>("UINTEGER", CSharpDataType.@int),     // 无符号
                new KeyValuePair<string, CSharpDataType>("UBIGINT", CSharpDataType.@long),     // 无符号

                // 浮点数类型
                new KeyValuePair<string, CSharpDataType>("FLOAT", CSharpDataType.@float),       // 4字节
                new KeyValuePair<string, CSharpDataType>("DOUBLE", CSharpDataType.@double),     // 8字节
                new KeyValuePair<string, CSharpDataType>("REAL", CSharpDataType.@float),        // 别名 FLOAT
                new KeyValuePair<string, CSharpDataType>("DECIMAL", CSharpDataType.@decimal),   // 精确小数
                new KeyValuePair<string, CSharpDataType>("NUMERIC", CSharpDataType.@decimal),   // 别名 DECIMAL

                // 布尔类型
                new KeyValuePair<string, CSharpDataType>("BOOLEAN", CSharpDataType.@bool),
                new KeyValuePair<string, CSharpDataType>("BOOL", CSharpDataType.@bool),         // 别名

                // 字符串类型
                new KeyValuePair<string, CSharpDataType>("VARCHAR", CSharpDataType.@string),
                new KeyValuePair<string, CSharpDataType>("CHAR", CSharpDataType.@string),
                new KeyValuePair<string, CSharpDataType>("TEXT", CSharpDataType.@string),       // 不限长度
                new KeyValuePair<string, CSharpDataType>("STRING", CSharpDataType.@string),      // 别名

                // 二进制类型
                new KeyValuePair<string, CSharpDataType>("BLOB", CSharpDataType.byteArray),
                new KeyValuePair<string, CSharpDataType>("BYTEA", CSharpDataType.byteArray),    // PostgreSQL风格

                // 日期时间类型 
                new KeyValuePair<string, CSharpDataType>("TIMESTAMP", CSharpDataType.DateTime), // 无时区
                new KeyValuePair<string, CSharpDataType>("DATE", CSharpDataType.DateTime),
                new KeyValuePair<string, CSharpDataType>("TIME", CSharpDataType.TimeSpan),
                new KeyValuePair<string, CSharpDataType>("TIMESTAMP_S", CSharpDataType.DateTime), // 秒级精度
                new KeyValuePair<string, CSharpDataType>("TIMESTAMP_MS", CSharpDataType.DateTime), // 毫秒级
                new KeyValuePair<string, CSharpDataType>("TIMESTAMP_NS", CSharpDataType.DateTime), // 纳秒级

                // 特殊类型
                new KeyValuePair<string, CSharpDataType>("UUID", CSharpDataType.Guid),
                new KeyValuePair<string, CSharpDataType>("JSON", CSharpDataType.@string),       // 存储为字符串
                new KeyValuePair<string, CSharpDataType>("ENUM", CSharpDataType.@string),        // 枚举实际存储为字符串

                // 几何类型（DuckDB支持简单几何类型）
                new KeyValuePair<string, CSharpDataType>("POINT", CSharpDataType.@object),       // 需自定义解析
                new KeyValuePair<string, CSharpDataType>("LINESTRING", CSharpDataType.@object),
                new KeyValuePair<string, CSharpDataType>("POLYGON", CSharpDataType.@object),

                // 数组类型（需特殊处理）
                new KeyValuePair<string, CSharpDataType>("INTEGER[]", CSharpDataType.@object),   // 实际为int[]
                new KeyValuePair<string, CSharpDataType>("VARCHAR[]", CSharpDataType.@object),   // 实际为string[]
                new KeyValuePair<string, CSharpDataType>("DOUBLE[]", CSharpDataType.@object),    // 实际为double[]

                // 结构体/Map类型（DuckDB 1.0+支持）
                new KeyValuePair<string, CSharpDataType>("STRUCT", CSharpDataType.@object),      // 需动态解析
                new KeyValuePair<string, CSharpDataType>("MAP", CSharpDataType.@object)          // 键值对
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
