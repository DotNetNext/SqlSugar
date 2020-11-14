using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlSugar
{
    public class PostgreSQLDbBind : DbBindProvider
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

                    new KeyValuePair<string, CSharpDataType>("int2",CSharpDataType.@short),
                    new KeyValuePair<string, CSharpDataType>("smallint",CSharpDataType.@short),
                    new KeyValuePair<string, CSharpDataType>("int4",CSharpDataType.@int),
                    new KeyValuePair<string, CSharpDataType>("integer",CSharpDataType.@int),
                    new KeyValuePair<string, CSharpDataType>("int8",CSharpDataType.@long),
                    new KeyValuePair<string, CSharpDataType>("bigint",CSharpDataType.@long),
                    new KeyValuePair<string, CSharpDataType>("float4",CSharpDataType.@float),
                    new KeyValuePair<string, CSharpDataType>("real",CSharpDataType.@float),
                    new KeyValuePair<string, CSharpDataType>("float8",CSharpDataType.@double),
                    new KeyValuePair<string, CSharpDataType>("double precision",CSharpDataType.@int),
                    new KeyValuePair<string, CSharpDataType>("numeric",CSharpDataType.@decimal),
                    new KeyValuePair<string, CSharpDataType>("decimal",CSharpDataType.@decimal),
                    new KeyValuePair<string, CSharpDataType>("path",CSharpDataType.@decimal),
                    new KeyValuePair<string, CSharpDataType>("point",CSharpDataType.@decimal),
                    new KeyValuePair<string, CSharpDataType>("polygon",CSharpDataType.@decimal),

                    new KeyValuePair<string, CSharpDataType>("boolean",CSharpDataType.@bool),
                    new KeyValuePair<string, CSharpDataType>("bool",CSharpDataType.@bool),
                    new KeyValuePair<string, CSharpDataType>("box",CSharpDataType.@bool),
                    new KeyValuePair<string, CSharpDataType>("bytea",CSharpDataType.@bool),

                    new KeyValuePair<string, CSharpDataType>("varchar",CSharpDataType.@string),
                    new KeyValuePair<string, CSharpDataType>("character varying",CSharpDataType.@string),
                    new KeyValuePair<string, CSharpDataType>("geometry",CSharpDataType.@string),
                    new KeyValuePair<string, CSharpDataType>("name",CSharpDataType.@string),
                    new KeyValuePair<string, CSharpDataType>("text",CSharpDataType.@string),
                    new KeyValuePair<string, CSharpDataType>("char",CSharpDataType.@string),
                    new KeyValuePair<string, CSharpDataType>("character",CSharpDataType.@string),
                    new KeyValuePair<string, CSharpDataType>("cidr",CSharpDataType.@string),
                    new KeyValuePair<string, CSharpDataType>("circle",CSharpDataType.@string),
                    new KeyValuePair<string, CSharpDataType>("tsquery",CSharpDataType.@string),
                    new KeyValuePair<string, CSharpDataType>("tsvector",CSharpDataType.@string),
                    new KeyValuePair<string, CSharpDataType>("txid_snapshot",CSharpDataType.@string),
                    new KeyValuePair<string, CSharpDataType>("uuid",CSharpDataType.Guid),
                    new KeyValuePair<string, CSharpDataType>("xml",CSharpDataType.@string),
                    new KeyValuePair<string, CSharpDataType>("json",CSharpDataType.@string),

                    new KeyValuePair<string, CSharpDataType>("interval",CSharpDataType.@decimal),
                    new KeyValuePair<string, CSharpDataType>("lseg",CSharpDataType.@decimal),
                    new KeyValuePair<string, CSharpDataType>("macaddr",CSharpDataType.@decimal),
                    new KeyValuePair<string, CSharpDataType>("money",CSharpDataType.@decimal),
                    new KeyValuePair<string, CSharpDataType>("timestamp",CSharpDataType.DateTime),
                    new KeyValuePair<string, CSharpDataType>("timestamp with time zone",CSharpDataType.DateTime),
                    new KeyValuePair<string, CSharpDataType>("timestamptz",CSharpDataType.DateTime),
                    new KeyValuePair<string, CSharpDataType>("timestamp without time zone",CSharpDataType.DateTime),
                    new KeyValuePair<string, CSharpDataType>("date",CSharpDataType.DateTime),
                    new KeyValuePair<string, CSharpDataType>("time",CSharpDataType.DateTime),
                    new KeyValuePair<string, CSharpDataType>("time with time zone",CSharpDataType.DateTime),
                    new KeyValuePair<string, CSharpDataType>("timetz",CSharpDataType.DateTime),
                    new KeyValuePair<string, CSharpDataType>("time without time zone",CSharpDataType.DateTime),

                    new KeyValuePair<string, CSharpDataType>("bit",CSharpDataType.byteArray),
                    new KeyValuePair<string, CSharpDataType>("bit varying",CSharpDataType.byteArray),
                    new KeyValuePair<string, CSharpDataType>("varbit",CSharpDataType.@byte),
                    
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
