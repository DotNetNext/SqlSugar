using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlSugar.ClickHouse
{
    public class ClickHouseDbBind : DbBindProvider
    {
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
                    return MappingTypes.Where(it => it.Value.ToString().ToLower() == dbTypeName2  || it.Key.ToLower() == dbTypeName2).Select(it => it.Value + "[]").First();
                }
                Check.ThrowNotSupportedException(string.Format(" \"{0}\" Type NotSupported, DbBindProvider.GetPropertyTypeName error.", dbTypeName));
                return String.Empty;
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

                    new KeyValuePair<string, CSharpDataType>("Int32",CSharpDataType.@int),
                    new KeyValuePair<string, CSharpDataType>("Int16",CSharpDataType.@short),
                    new KeyValuePair<string, CSharpDataType>("Int64",CSharpDataType.@long),
                    new KeyValuePair<string, CSharpDataType>("UInt32",CSharpDataType.@int),
                    new KeyValuePair<string, CSharpDataType>("UInt16",CSharpDataType.@short),
                    new KeyValuePair<string, CSharpDataType>("UInt64",CSharpDataType.@long),
                    new KeyValuePair<string, CSharpDataType>("Int8",CSharpDataType.@sbyte),
                    new KeyValuePair<string, CSharpDataType>("UInt8",CSharpDataType.@byte),
                    new KeyValuePair<string, CSharpDataType>("Decimal(38,19)",CSharpDataType.@decimal),
                    new KeyValuePair<string, CSharpDataType>("Decimal",CSharpDataType.@decimal),
                    new KeyValuePair<string, CSharpDataType>("Decimal32",CSharpDataType.@decimal),
                    new KeyValuePair<string, CSharpDataType>("Decimal64",CSharpDataType.@decimal),
                    new KeyValuePair<string, CSharpDataType>("Decimal128",CSharpDataType.@decimal),
                    new KeyValuePair<string, CSharpDataType>("Decimal256",CSharpDataType.@decimal),
                    new KeyValuePair<string, CSharpDataType>("Boolean",CSharpDataType.@bool),
                    new KeyValuePair<string, CSharpDataType>("String",CSharpDataType.@string),
                    new KeyValuePair<string, CSharpDataType>("Fixedstring",CSharpDataType.@string),
                    new KeyValuePair<string, CSharpDataType>("UUID",CSharpDataType.Guid),

                    new KeyValuePair<string, CSharpDataType>("DateTime",CSharpDataType.DateTime),
                                        new KeyValuePair<string, CSharpDataType>("DateTime('Asia/Shanghai')",CSharpDataType.DateTime),
                    new KeyValuePair<string, CSharpDataType>("DATE",CSharpDataType.DateTime),
                    new KeyValuePair<string, CSharpDataType>("Datetime64",CSharpDataType.DateTime),
                    new KeyValuePair<string, CSharpDataType>("Datetime64",CSharpDataType.DateTime),
                    new KeyValuePair<string, CSharpDataType>("Float64",CSharpDataType.@double),
                    new KeyValuePair<string, CSharpDataType>("Float32",CSharpDataType.@float),
                    new KeyValuePair<string, CSharpDataType>("nullable",CSharpDataType.@object),
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
