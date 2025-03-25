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
                return "varbinary";
            if (csharpTypeName.ToLower().IsIn("int","int32"))
                csharpTypeName = "int";
            else if (csharpTypeName.ToLower() == "int16")
                csharpTypeName = "short";
            else if (csharpTypeName.ToLower() == "int64")
                csharpTypeName = "long";
            else if (csharpTypeName.ToLower().IsIn("boolean", "bool"))
                csharpTypeName = "bool";
            else if (csharpTypeName.ToLower().IsIn("dateTime","datetimeoffset"))
                csharpTypeName = "dateTime";
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
            if (propertyTypes == null || !propertyTypes.Any())
            {
                return "string";
            }
            else if (dbTypeName == "xml" || dbTypeName == "string" || dbTypeName == "jsonb" || dbTypeName == "json")
            {
                return "string";
            }
            else if (dbTypeName == "character")
            {
                return "char";
            }
            else if (dbTypeName == "byte[]")
            {
                return "byte[]";
            }
            else if (dbTypeName == "boolean")
            {
                return "char";
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


                    new KeyValuePair<string, CSharpDataType>("boolean",CSharpDataType.@bool),
                    new KeyValuePair<string, CSharpDataType>("varbinary",CSharpDataType.@byteArray),
                    new KeyValuePair<string, CSharpDataType>("binary",CSharpDataType.@byteArray),
                    new KeyValuePair<string, CSharpDataType>("blob",CSharpDataType.@byteArray),

                    new KeyValuePair<string, CSharpDataType>("varchar",CSharpDataType.@string),
                    new KeyValuePair<string, CSharpDataType>("char",CSharpDataType.@string),
                    new KeyValuePair<string, CSharpDataType>("clob",CSharpDataType.@string),
                    new KeyValuePair<string, CSharpDataType>("vargraphic",CSharpDataType.@string),

                    new KeyValuePair<string, CSharpDataType>("timestamp",CSharpDataType.@DateTime),
                    new KeyValuePair<string, CSharpDataType>("date",CSharpDataType.@DateTime),
                    new KeyValuePair<string, CSharpDataType>("time",CSharpDataType.@DateTime),

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

        public static List<KeyValuePair<string, System.Data.DbType>> MappingDbTypesConst = new List<KeyValuePair<string, System.Data.DbType>>(){


            new KeyValuePair<string, System.Data.DbType>("boolean",System.Data.DbType.Boolean),
            new KeyValuePair<string, System.Data.DbType>("varbinary",System.Data.DbType.Byte),
            new KeyValuePair<string, System.Data.DbType>("varchar",System.Data.DbType.String),

            new KeyValuePair<string, System.Data.DbType>("date",System.Data.DbType.Date),
            new KeyValuePair<string, System.Data.DbType>("time",System.Data.DbType.Time),
                        new KeyValuePair<string, System.Data.DbType>("timestamp",System.Data.DbType.DateTime),
            new KeyValuePair<string, System.Data.DbType>("timestamp",System.Data.DbType.DateTime2),
             new KeyValuePair<string, System.Data.DbType>("timestamp",System.Data.DbType.DateTimeOffset),

            new KeyValuePair<string, System.Data.DbType>("integer",System.Data.DbType.Int32),
            new KeyValuePair<string, System.Data.DbType>("smallint",System.Data.DbType.Int16),
            new KeyValuePair<string, System.Data.DbType>("bigint",System.Data.DbType.Int64),

            new KeyValuePair<string, System.Data.DbType>("float",System.Data.DbType.Single),
            new KeyValuePair<string, System.Data.DbType>("double",System.Data.DbType.Double),
            new KeyValuePair<string, System.Data.DbType>("numeric",System.Data.DbType.VarNumeric),

                };
    }
}
