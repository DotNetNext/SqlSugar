using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlSugar
{
    public class DmDbBind : DbBindProvider
    {
        public override string GetDbTypeName(string csharpTypeName)
        {
            if (csharpTypeName == UtilConstants.ByteArrayType.Name)
                return "blob";
            if (csharpTypeName.ToLower() == "int32")
                csharpTypeName = "int";
            if (csharpTypeName.ToLower() == "int16")
                csharpTypeName = "short";
            if (csharpTypeName.ToLower() == "int64")
                csharpTypeName = "long";
            if (csharpTypeName.ToLower().IsIn("boolean", "bool"))
                csharpTypeName = "bool";
            var mappings = this.MappingTypes.Where(it => it.Value.ToString().Equals(csharpTypeName, StringComparison.CurrentCultureIgnoreCase));
            return mappings.HasValue() ? mappings.First().Key : "varchar";
        }
        public override string GetPropertyTypeName(string dbTypeName)
        {
            dbTypeName = dbTypeName.ToLower();
            var propertyTypes = MappingTypes.Where(it => it.Value.ToString().ToLower() == dbTypeName || it.Key.ToLower() == dbTypeName);
            if (dbTypeName == "int32")
            {
                return "int";
            }
            else if (dbTypeName == "int64")
            {
                return "long";
            }
            else if (dbTypeName == "int16")
            {
                return "short";
            }
            else if (propertyTypes == null)
            {
                return "other";
            }
            else if (dbTypeName == "xml" || dbTypeName == "string")
            {
                return "string";
            }
            if (dbTypeName == "byte[]")
            {
                return "byte[]";
            }
            else if (propertyTypes == null || propertyTypes.Count() == 0)
            {
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
        public static List<KeyValuePair<string, CSharpDataType>> MappingTypesConst = new List<KeyValuePair<string, CSharpDataType>>()
                {
                  new KeyValuePair<string, CSharpDataType>("int",CSharpDataType.@int),
                  new KeyValuePair<string, CSharpDataType>("integer",CSharpDataType.@int),
                  new KeyValuePair<string, CSharpDataType>("interval year to  month",CSharpDataType.@int),
                  new KeyValuePair<string, CSharpDataType>("interval day to  second",CSharpDataType.@int),

                  new KeyValuePair<string, CSharpDataType>("number",CSharpDataType.@int),
                  new KeyValuePair<string, CSharpDataType>("number",CSharpDataType.@float),
                  new KeyValuePair<string, CSharpDataType>("number",CSharpDataType.@short),
                  new KeyValuePair<string, CSharpDataType>("number",CSharpDataType.@byte),
                  new KeyValuePair<string, CSharpDataType>("number",CSharpDataType.@double),
                  new KeyValuePair<string, CSharpDataType>("number",CSharpDataType.@long),
                  new KeyValuePair<string, CSharpDataType>("number",CSharpDataType.@bool),
                  new KeyValuePair<string, CSharpDataType>("bit",CSharpDataType.@bool),
                  new KeyValuePair<string, CSharpDataType>("number",CSharpDataType.@decimal),
                  new KeyValuePair<string, CSharpDataType>("number",CSharpDataType.Single),
                  new KeyValuePair<string, CSharpDataType>("decimal",CSharpDataType.@decimal),
                  new KeyValuePair<string, CSharpDataType>("decimal",CSharpDataType.Single),

                  new KeyValuePair<string, CSharpDataType>("varchar",CSharpDataType.@string),
                  new KeyValuePair<string, CSharpDataType>("varchar",CSharpDataType.@Guid),
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
        public override List<string> StringThrow
        {
            get
            {
                return new List<string>() { "int32", "datetime", "decimal", "double", "byte" };
            }
        }
    }
}
