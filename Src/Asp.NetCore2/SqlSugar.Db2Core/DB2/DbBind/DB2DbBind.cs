using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlSugar.DB2
{
    public class DB2DbBind : DbBindProvider
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
            else if (dbTypeName == "xml" || dbTypeName == "string" || dbTypeName == "jsonb" || dbTypeName == "json")
            {
                return "string";
            }
            else if (dbTypeName == "bpchar")//数据库char datatype 查询出来的时候是 bpchar
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
                Check.ThrowNotSupportedException(string.Format(" \"{0}\" Type NotSupported, DbBindProvider.GetPropertyTypeName error.", dbTypeName));
                return null;
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

                    new KeyValuePair<string, CSharpDataType>("varbinary",CSharpDataType.@byteArray),
                    new KeyValuePair<string, CSharpDataType>("binary",CSharpDataType.@byteArray),
                    new KeyValuePair<string, CSharpDataType>("blob",CSharpDataType.@byteArray),

                    new KeyValuePair<string, CSharpDataType>("varchar",CSharpDataType.@string),
                    new KeyValuePair<string, CSharpDataType>("char",CSharpDataType.@string),
                    new KeyValuePair<string, CSharpDataType>("clob",CSharpDataType.@string),

                    new KeyValuePair<string, CSharpDataType>("date",CSharpDataType.@DateTime),
                    new KeyValuePair<string, CSharpDataType>("time",CSharpDataType.@DateTime),
                    new KeyValuePair<string, CSharpDataType>("timestamp",CSharpDataType.@DateTime),

                    new KeyValuePair<string, CSharpDataType>("integer",CSharpDataType.@int),
                    new KeyValuePair<string, CSharpDataType>("smallint",CSharpDataType.@short),
                    new KeyValuePair<string, CSharpDataType>("smallint",CSharpDataType.@byte),
                    new KeyValuePair<string, CSharpDataType>("bigint",CSharpDataType.@long),

                    new KeyValuePair<string, CSharpDataType>("real",CSharpDataType.Single),
                    new KeyValuePair<string, CSharpDataType>("float",CSharpDataType.@float),
                    new KeyValuePair<string, CSharpDataType>("double",CSharpDataType.@double),
                    new KeyValuePair<string, CSharpDataType>("decimal",CSharpDataType.@decimal),
                    new KeyValuePair<string, CSharpDataType>("numeric",CSharpDataType.@decimal),

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
