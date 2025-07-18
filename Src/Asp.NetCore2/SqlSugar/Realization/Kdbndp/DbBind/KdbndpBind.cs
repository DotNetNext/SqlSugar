﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlSugar
{
    public class KdbndpDbBind : DbBindProvider
    {
        public override string GetDbTypeName(string csharpTypeName)
        {
            if (csharpTypeName == UtilConstants.ByteArrayType.Name)
                return "bytea";

            var result = base.GetDbTypeName(csharpTypeName);
            return result;
        }
        public override string GetPropertyTypeName(string dbTypeName)
        {
            dbTypeName = dbTypeName.Replace("pg_catalog.", "");
            dbTypeName = dbTypeName.Replace("sys.", "");
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
                    new KeyValuePair<string, CSharpDataType>("uint2",CSharpDataType.@short),
                    //new KeyValuePair<string, CSharpDataType>("int1",CSharpDataType.@byte),
                    new KeyValuePair<string, CSharpDataType>("smallint",CSharpDataType.@short),
                    new KeyValuePair<string, CSharpDataType>("smallint",CSharpDataType.@byte),
                    new KeyValuePair<string, CSharpDataType>("tinyint",CSharpDataType.@byte),
                    new KeyValuePair<string, CSharpDataType>("int4",CSharpDataType.@int),
                    new KeyValuePair<string, CSharpDataType>("uint4",CSharpDataType.@int),
                    new KeyValuePair<string, CSharpDataType>("integer",CSharpDataType.@int),
                    new KeyValuePair<string, CSharpDataType>("tinyint",CSharpDataType.@int),
                    new KeyValuePair<string, CSharpDataType>("int8",CSharpDataType.@long),
                    new KeyValuePair<string, CSharpDataType>("uint8",CSharpDataType.@long),
                    new KeyValuePair<string, CSharpDataType>("bigint",CSharpDataType.@long),
                    new KeyValuePair<string, CSharpDataType>("float4",CSharpDataType.@float),
                    new KeyValuePair<string, CSharpDataType>("float4",CSharpDataType.Single),
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
                    new KeyValuePair<string, CSharpDataType>("name",CSharpDataType.@string),
                    new KeyValuePair<string, CSharpDataType>("text",CSharpDataType.@string),
                    new KeyValuePair<string, CSharpDataType>("char",CSharpDataType.@string),
                    new KeyValuePair<string, CSharpDataType>("nchar",CSharpDataType.@string),
                    new KeyValuePair<string, CSharpDataType>("character",CSharpDataType.@string),
                    new KeyValuePair<string, CSharpDataType>("cidr",CSharpDataType.@string),
                    new KeyValuePair<string, CSharpDataType>("circle",CSharpDataType.@string),
                    new KeyValuePair<string, CSharpDataType>("tsquery",CSharpDataType.@string),
                    new KeyValuePair<string, CSharpDataType>("tsvector",CSharpDataType.@string),
                    new KeyValuePair<string, CSharpDataType>("txid_snapshot",CSharpDataType.@string),
                    new KeyValuePair<string, CSharpDataType>("varcharbyte",CSharpDataType.@string),
                    new KeyValuePair<string, CSharpDataType>("varcharbyte varying",CSharpDataType.@string),
                    new KeyValuePair<string, CSharpDataType>("bpcharbyte",CSharpDataType.@string),
                    new KeyValuePair<string, CSharpDataType>("nvarchar",CSharpDataType.@string),
                    new KeyValuePair<string, CSharpDataType>("characterbyte",CSharpDataType.@string),
                    new KeyValuePair<string, CSharpDataType>("information_schema.character_data",CSharpDataType.@string),
                    new KeyValuePair<string, CSharpDataType>("uuid",CSharpDataType.Guid),
                    new KeyValuePair<string, CSharpDataType>("uniqueidentifier",CSharpDataType.Guid),
                    new KeyValuePair<string, CSharpDataType>("xml",CSharpDataType.@string),
                    new KeyValuePair<string, CSharpDataType>("json",CSharpDataType.@string),
                    new KeyValuePair<string, CSharpDataType>("rowid",CSharpDataType.@string),
                    new KeyValuePair<string, CSharpDataType>("information_schema.sql_identifier",CSharpDataType.@string),
                    new KeyValuePair<string, CSharpDataType>("information_schema.cardinal_number",CSharpDataType.@string),
                    new KeyValuePair<string, CSharpDataType>("interval",CSharpDataType.@decimal),
                    new KeyValuePair<string, CSharpDataType>("lseg",CSharpDataType.@decimal),
                    new KeyValuePair<string, CSharpDataType>("macaddr",CSharpDataType.@decimal),
                    new KeyValuePair<string, CSharpDataType>("money",CSharpDataType.@decimal),
                    new KeyValuePair<string, CSharpDataType>("timestamp",CSharpDataType.DateTime),
                    new KeyValuePair<string, CSharpDataType>("datetime2",CSharpDataType.DateTime),
                    new KeyValuePair<string, CSharpDataType>("datetime",CSharpDataType.DateTime),
                    new KeyValuePair<string, CSharpDataType>("timestamp with time zone",CSharpDataType.DateTime),
                    new KeyValuePair<string, CSharpDataType>("timestamptz",CSharpDataType.DateTime),
                    new KeyValuePair<string, CSharpDataType>("timestamp without time zone",CSharpDataType.DateTime),
                    new KeyValuePair<string, CSharpDataType>("date",CSharpDataType.DateTime),
                    new KeyValuePair<string, CSharpDataType>("time",CSharpDataType.DateTime),
                    new KeyValuePair<string, CSharpDataType>("time with time zone",CSharpDataType.DateTime),
                    new KeyValuePair<string, CSharpDataType>("timetz",CSharpDataType.DateTime),
                    new KeyValuePair<string, CSharpDataType>("time without time zone",CSharpDataType.DateTime),

                    new KeyValuePair<string, CSharpDataType>("bit",CSharpDataType.byteArray),
                    new KeyValuePair<string, CSharpDataType>("blob",CSharpDataType.byteArray),
                    new KeyValuePair<string, CSharpDataType>("bit varying",CSharpDataType.byteArray),
                    new KeyValuePair<string, CSharpDataType>("binary",CSharpDataType.byteArray),
                    new KeyValuePair<string, CSharpDataType>("varbinary",CSharpDataType.byteArray),
                    new KeyValuePair<string, CSharpDataType>("image",CSharpDataType.byteArray),
                    new KeyValuePair<string, CSharpDataType>("varbit",CSharpDataType.@byte),
                    new KeyValuePair<string, CSharpDataType>("rowversion",CSharpDataType.byteArray),
                    new KeyValuePair<string, CSharpDataType>("regclass",CSharpDataType.@object),

                    new KeyValuePair<string, CSharpDataType>("geometry",CSharpDataType.@object),
                    new KeyValuePair<string, CSharpDataType>("public.geometry",CSharpDataType.@object),
                    new KeyValuePair<string, CSharpDataType>("geography",CSharpDataType.@object),
                    new KeyValuePair<string, CSharpDataType>("public.geography",CSharpDataType.@object),

                    new KeyValuePair<string, CSharpDataType>("dsinterval",CSharpDataType.TimeSpan),
                    new KeyValuePair<string, CSharpDataType>("yminterval",CSharpDataType.@int),
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
