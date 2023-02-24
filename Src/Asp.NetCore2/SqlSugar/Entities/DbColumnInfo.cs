﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace SqlSugar
{
    public class DbColumnInfo
    {
        public string TableName { get; set; }
        public int TableId { get; set; }
        public string DbColumnName { get; set; }
        public string PropertyName { get; set; }
        public string DataType { get; set; }
        public string OracleDataType { get; set; }
        public Type PropertyType { get; set; }
        public int Length { get; set; }
        public string ColumnDescription { get; set; }
        public string DefaultValue { get; set; }
        public bool IsNullable { get; set; }
        public bool IsIdentity { get; set; }
        public bool IsPrimarykey { get; set; }
        public object Value { get; set; }
        public int DecimalDigits { get; set; }
        public int Scale { get; set; }
        public bool IsArray { get;  set; }
        public bool IsJson { get;  set; }
        public bool? IsUnsigned { get; set; }
        public int CreateTableFieldSort { get; set; }
        public bool InsertServerTime { get;  set; }
        public string InsertSql { get;  set; }
        public bool UpdateServerTime { get; set; }
        public string UpdateSql { get; set; }
        internal object SqlParameterDbType { get;  set; }
    }
}
