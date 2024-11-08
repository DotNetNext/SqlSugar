﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlSugar
{
    public class DmDbMaintenance : DbMaintenanceProvider
    {
        #region DML
        protected override string GetDataBaseSql
        {
            get
            {
                throw new NotSupportedException();
            }
        }
        protected override string GetColumnInfosByTableNameSql
        {
            get
            {
                throw new NotSupportedException();
            }
        }
        protected override string GetTableInfoListSql
        {
            get
            {
                return @"SELECT  
                        table_name name,
                        (select TOP 1 COMMENTS from user_tab_comments where t.table_name=table_name )  as Description
                        from user_tables t where
                        table_name!='HELP' 
                        AND table_name NOT LIKE '%$%'
                        AND table_name NOT LIKE 'LOGMNRC_%'
                        AND table_name!='LOGMNRP_CTAS_PART_MAP'
                        AND table_name!='LOGMNR_LOGMNR_BUILDLOG'
                        AND table_name!='SQLPLUS_PRODUCT_PROFILE'  
                         ";
            }
        }
        protected override string GetViewInfoListSql
        {
            get
            {
                return @"select VIEW_NAME name from all_views WHERE OWNER=SF_GET_SCHEMA_NAME_BY_ID(CURRENT_SCHID) order by VIEW_NAME";
            }
        }
        #endregion

        #region DDL
        protected override string IsAnyIndexSql
        {
            get
            {
                return "select count(1) from user_ind_columns where upper(index_name)=upper('{0}')";
            }
        }
        protected override string CreateIndexSql
        {
            get
            {
                return "CREATE {3} INDEX Index_{0}_{2} ON {0}({1})";
            }
        }
        protected override string AddDefaultValueSql
        {
            get
            {
                return "ALTER TABLE {0} MODIFY({1} DEFAULT '{2}')";
            }
        }
        protected override string CreateDataBaseSql
        {
            get
            {
                return "CREATE DATABASE {0}";
            }
        }
        protected override string AddPrimaryKeySql
        {
            get
            {
                return "ALTER TABLE {0} ADD PRIMARY KEY({2}) /*{1}*/";
            }
        }
        protected override string AddColumnToTableSql
        {
            get
            {
                return "ALTER TABLE {0} ADD ({1} {2}{3} {4} {5} {6})";
            }
        }
        protected override string AlterColumnToTableSql
        {
            get
            {
                return "ALTER TABLE {0} modify ({1} {2}{3} {4} {5} {6}) ";
            }
        }
        protected override string BackupDataBaseSql
        {
            get
            {
                return @"USE master;BACKUP DATABASE {0} TO disk = '{1}'";
            }
        }
        protected override string CreateTableSql
        {
            get
            {
                return "CREATE TABLE {0}(\r\n{1} )";
            }
        }
        protected override string CreateTableColumn
        {
            get
            {
                return "{0} {1}{2} {3} {4} {5}";
            }
        }
        protected override string TruncateTableSql
        {
            get
            {
                return "TRUNCATE TABLE {0}";
            }
        }
        protected override string BackupTableSql
        {
            get
            {
                return "create table {1} as select * from {2}  where ROWNUM<={0}";
            }
        }
        protected override string DropTableSql
        {
            get
            {
                return "DROP TABLE {0}";
            }
        }
        protected override string DropColumnToTableSql
        {
            get
            {
                return "ALTER TABLE {0} DROP COLUMN {1}";
            }
        }
        protected override string DropConstraintSql
        {
            get
            {
                return "ALTER TABLE {0} DROP CONSTRAINT  {1}";
            }
        }
        protected override string RenameColumnSql
        {
            get
            {
                return "ALTER TABLE {0} rename   column  {1} to {2}";
            }
        }
        protected override string AddColumnRemarkSql
        {
            get
            {
                return "comment on column {1}.{0} is '{2}'";
            }
        }

        protected override string DeleteColumnRemarkSql
        {
            get
            {
                return "comment on column {1}.{0} is ''";
            }
        }

        protected override string IsAnyColumnRemarkSql
        {
            get
            {
                return "select * from user_col_comments where Table_Name='{1}' AND COLUMN_NAME='{0}' order by column_name";
            }
        }

        protected override string AddTableRemarkSql
        {
            get
            {
                return "comment on table {0}  is  '{1}'";
            }
        }

        protected override string DeleteTableRemarkSql
        {
            get
            {
                return "comment on table {0}  is  ''";
            }
        }

        protected override string IsAnyTableRemarkSql
        {
            get
            {
                return "select * from user_tab_comments where Table_Name='{0}'order by Table_Name";
            }
        }

        protected override string RenameTableSql
        {
            get
            {
                return "alter table {0} rename to {1}";
            }
        }
        protected override string IsAnyProcedureSql => throw new NotImplementedException();
        #endregion

        #region Check
        protected override string CheckSystemTablePermissionsSql
        {
            get
            {
                return "select  t.table_name from user_tables t  where rownum=1";
            }
        }
        #endregion

        #region Scattered
        protected override string CreateTableNull
        {
            get
            {
                return " null ";
            }
        }
        protected override string CreateTableNotNull
        {
            get
            {
                return " not null ";
            }
        }
        protected override string CreateTablePirmaryKey
        {
            get
            {
                return "PRIMARY KEY";
            }
        }
        protected override string CreateTableIdentity
        {
            get
            {
                return "IDENTITY(1,1)";
            }
        }
        #endregion

        #region Methods
        public override bool UpdateColumn(string tableName, DbColumnInfo column)
        {
            ConvertCreateColumnInfo(column);
            var oldColumn = this.Context.DbMaintenance.GetColumnInfosByTableName(tableName, false)
                .FirstOrDefault(it => it.DbColumnName.EqualCase(column.DbColumnName));
            if (oldColumn != null)
            {
                if (oldColumn.IsNullable == column.IsNullable)
                {
                    var sql = GetUpdateColumnSqlOnlyType(tableName, column);
                    this.Context.Ado.ExecuteCommand(sql);
                    return true;
                }
            }
            return base.UpdateColumn(tableName, column);
        }
        protected virtual string GetUpdateColumnSqlOnlyType(string tableName, DbColumnInfo columnInfo)
        {
            string columnName = this.SqlBuilder.GetTranslationColumnName(columnInfo.DbColumnName);
            tableName = this.SqlBuilder.GetTranslationTableName(tableName);
            string dataSize = GetSize(columnInfo);
            string dataType = columnInfo.DataType;
            string nullType = "";
            string primaryKey = null;
            string identity = null;
            string result = string.Format(this.AlterColumnToTableSql, tableName, columnName, dataType, dataSize, nullType, primaryKey, identity);
            return result;
        }
        public override List<string> GetDbTypes()
        {
            var result = this.Context.Ado.SqlQuery<string>(@"SELECT DISTINCT DATA_TYPE
FROM DBA_TAB_COLUMNS
WHERE OWNER = user ");
            result.Add("TIMESTAMP");
            result.Add("NCLOB");
            return result.Distinct().ToList();
        }
        public override List<string> GetTriggerNames(string tableName)
        {
            return this.Context.Ado.SqlQuery<string>(@"SELECT trigger_name
FROM all_triggers
WHERE table_name = '" + tableName + "'");
        }
        public override List<string> GetFuncList()
        {
            return this.Context.Ado.SqlQuery<string>(" SELECT object_name\r\nFROM all_objects\r\nWHERE object_type = 'FUNCTION' AND owner = USER ");
        }
        public override bool RenameTable(string oldTableName, string newTableName)
        {
            oldTableName = SqlBuilder.GetTranslationColumnName(oldTableName);
            newTableName = SqlBuilder.GetTranslationColumnName(newTableName); 
            return base.RenameTable(oldTableName, newTableName);
        }
        public override List<string> GetIndexList(string tableName)
        {
            var sql = $"SELECT index_name FROM user_ind_columns\r\nWHERE upper(table_name) = upper('{tableName}')";
            return this.Context.Ado.SqlQuery<string>(sql);
        }
        public override bool AddColumn(string tableName, DbColumnInfo columnInfo)
        {
            if (columnInfo.DataType == "varchar" && columnInfo.Length == 0)
            {
                columnInfo.DataType = "varchar2";
                columnInfo.Length = 50;
            }
            return base.AddColumn(tableName, columnInfo);
        }
        public override bool CreateIndex(string tableName, string[] columnNames, bool isUnique = false)
        {
            string sql = string.Format(CreateIndexSql, tableName, string.Join(",", columnNames), string.Join("_", columnNames.Select(it => (it + "abc").Substring(0, 3))), isUnique ? "UNIQUE" : "");
            this.Context.Ado.ExecuteCommand(sql);
            return true;
        }
        public override bool AddDefaultValue(string tableName, string columnName, string defaultValue)
        {
            columnName = SqlBuilder.GetTranslationColumnName(columnName);
            tableName = SqlBuilder.GetTranslationColumnName(tableName);
            if (defaultValue == "''")
            {
                defaultValue = "";
            }
            if (defaultValue.ToLower().IsIn("sysdate"))
            {
                var template = AddDefaultValueSql.Replace("'", "");
                string sql = string.Format(template, tableName, columnName, defaultValue);
                this.Context.Ado.ExecuteCommand(sql);
                return true;
            }
            else
            {
                return base.AddDefaultValue(tableName, columnName, defaultValue);
            }
        }
        public override bool CreateDatabase(string databaseDirectory = null)
        {
            if (this.Context.Ado.IsValidConnection())
            {
                return true;
            }
            Check.ExceptionEasy("dm no support create database ", "达梦不支持建库方法，请写有效连接字符串可以正常运行该方法。");
            return true;
        }
        public override bool CreateDatabase(string databaseName, string databaseDirectory = null)
        {
            if (this.Context.Ado.IsValidConnection())
            {
                return true;
            }
            Check.ExceptionEasy("dm no support create database ", "达梦不支持建库方法，请写有效连接字符串可以正常运行该方法。");
            return true;
        }
        public override bool AddRemark(EntityInfo entity)
        {
            var db = this.Context;
            var columns = entity.Columns.Where(it => it.IsIgnore == false).ToList();

            foreach (var item in columns)
            {
                if (item.ColumnDescription != null)
                {
                    //column remak
                    if (db.DbMaintenance.IsAnyColumnRemark(item.DbColumnName.ToUpper(IsUppper), item.DbTableName.ToUpper(IsUppper)))
                    {
                        db.DbMaintenance.DeleteColumnRemark(this.SqlBuilder.GetTranslationColumnName(item.DbColumnName), this.SqlBuilder.GetTranslationColumnName(item.DbTableName));
                        db.DbMaintenance.AddColumnRemark(this.SqlBuilder.GetTranslationColumnName(item.DbColumnName), this.SqlBuilder.GetTranslationColumnName(item.DbTableName), item.ColumnDescription);
                    }
                    else
                    {
                        db.DbMaintenance.AddColumnRemark(this.SqlBuilder.GetTranslationColumnName(item.DbColumnName), this.SqlBuilder.GetTranslationColumnName(item.DbTableName), item.ColumnDescription);
                    }
                }
            }

            //table remak
            if (entity.TableDescription != null)
            {
                if (db.DbMaintenance.IsAnyTableRemark(entity.DbTableName))
                {
                    db.DbMaintenance.DeleteTableRemark(SqlBuilder.GetTranslationColumnName(entity.DbTableName));
                    db.DbMaintenance.AddTableRemark(SqlBuilder.GetTranslationColumnName(entity.DbTableName), entity.TableDescription);
                }
                else
                {
                    db.DbMaintenance.AddTableRemark(SqlBuilder.GetTranslationColumnName(entity.DbTableName), entity.TableDescription);
                }
            }
            return true;
        }

        public override bool AddTableRemark(string tableName, string description)
        {
            return base.AddTableRemark(SqlBuilder.GetTranslationColumnName(tableName), description);
        }
        public override bool AddColumnRemark(string columnName, string tableName, string description)
        {
            return base.AddColumnRemark(SqlBuilder.GetTranslationColumnName(columnName), SqlBuilder.GetTranslationColumnName(tableName), description);
        }
        public override List<DbColumnInfo> GetColumnInfosByTableName(string tableName, bool isCache = true)
        {
            string cacheKey = "DbMaintenanceProvider.GetColumnInfosByTableName." + this.SqlBuilder.GetNoTranslationColumnName(tableName).ToLower();
            cacheKey = GetCacheKey(cacheKey);
            if (!isCache)
                return GetColumnInfosByTableName(tableName);
            else
                return this.Context.Utilities.GetReflectionInoCacheInstance().GetOrCreate(cacheKey,
                    () =>
                    {
                        return GetColumnInfosByTableName(tableName);

                    });
        }

        private List<DbColumnInfo> GetColumnInfosByTableName(string tableName)
        {
            List<DbColumnInfo> columns = GetOracleDbType(tableName);
            string sql = "select * from " + SqlBuilder.GetTranslationTableName(tableName) + " WHERE 1=2 ";
            if (!this.GetTableInfoList(false).Any(it => it.Name == SqlBuilder.GetTranslationTableName(tableName).TrimStart('\"').TrimEnd('\"')))
            {
                sql = "select * from \"" + tableName + "\" WHERE 1=2 ";
            }
            var oldIsEnableLog = this.Context.Ado.IsEnableLogEvent;
            this.Context.Ado.IsEnableLogEvent = false;
            using(DbDataReader reader = (DbDataReader) this.Context.Ado.GetDataReader(sql))
            {
                this.Context.Ado.IsEnableLogEvent = oldIsEnableLog;
                List<DbColumnInfo> result = new List<DbColumnInfo>();
                var schemaTable = reader.GetSchemaTable();
                foreach (System.Data.DataRow row in schemaTable.Rows)
                {
                    DbColumnInfo column = new DbColumnInfo()
                    {
                        TableName = tableName,
                        DataType = row["DataType"].ToString().Replace("System.", "").Trim(),
                        IsNullable = (bool) row["AllowDBNull"],
                        IsIdentity = (bool) row["IsIdentity"],
                        ColumnDescription = GetFieldComment(tableName, row["ColumnName"].ToString()),
                        DbColumnName = row["ColumnName"].ToString(),
                        //DefaultValue = row["defaultValue"].ToString(),
                        IsPrimarykey = GetPrimaryKeyByTableNames(tableName).Any(it => it.Equals(row["ColumnName"].ToString(), StringComparison.CurrentCultureIgnoreCase)),
                        Length = row["ColumnSize"].ObjToInt(),
                        Scale = row["numericscale"].ObjToInt()
                    };
                    if (column.DataType.EqualCase("number")|| column.DataType.EqualCase("decimal"))
                    {
                        column.Length = row["numericprecision"].ObjToInt();
                        column.Scale = row["numericscale"].ObjToInt();
                        column.DecimalDigits = row["numericscale"].ObjToInt();
                        if (column.Length == 38 && column.Scale == 0)
                        {
                            column.Length = 22;
                        }
                    }
                    var current = columns.FirstOrDefault(it => it.DbColumnName.EqualCase(column.DbColumnName));
                    if (current != null)
                    {
                        column.OracleDataType = current.DataType;
                        column.DefaultValue = current.DefaultValue?.TrimStart('\'')?.TrimEnd('\'');
                    }
                    result.Add(column);
                }
                return result;
            }
        }

        private List<DbColumnInfo> GetOracleDbType(string tableName)
        {
            var sql0 = $@"select      
                                 t1.table_name as TableName,   
                                 t6.comments,        
                                 t1.column_id, 
                                 t1.column_name as DbColumnName,     
                                 t5.comments,       
                                 t1.data_type as DataType,     
                                 t1.data_length as Length,   
                                 t1.char_length,   
                                 t1.data_precision,  
                                 t1.data_scale,     
                                 t1.nullable, 
                                 t1.data_default as DefaultValue,
                                 t4.index_name,     
                                 t4.column_position,  
                                 t4.descend          
                            from user_tab_columns t1   
                            left join (select t2.table_name,     
                                              t2.column_name,  
                                              t2.column_position,  
                                              t2.descend,      
                                              t3.index_name        
                                       from user_ind_columns t2   
                                       left join user_indexes t3   
                                         on  t2.table_name = t3.table_name and t2.index_name = t3.index_name
                                        and t3.status = 'valid' and t3.uniqueness = 'unique') t4   --unique:唯一索引
                              on  t1.table_name = t4.table_name and t1.column_name = t4.column_name 
                            left join user_col_comments t5 on   t1.table_name = t5.table_name and t1.column_name = t5.column_name 
                            left join user_tab_comments t6 on  t1.table_name = t6.table_name
                            where upper(t1.table_name)=upper('{tableName}')
                            order by  t1.table_name, t1.column_id";

            var columns = this.Context.Ado.SqlQuery<DbColumnInfo>(sql0);
            return columns;
        }

        private List<string> GetPrimaryKeyByTableNames(string tableName)
        {
            string cacheKey = "DbMaintenanceProvider.GetPrimaryKeyByTableNames." + this.SqlBuilder.GetNoTranslationColumnName(tableName).ToLower();
            cacheKey = GetCacheKey(cacheKey);
            return this.Context.Utilities.GetReflectionInoCacheInstance().GetOrCreate(cacheKey,
                () =>
                {
                    var oldIsEnableLog = this.Context.Ado.IsEnableLogEvent;
                    this.Context.Ado.IsEnableLogEvent = false;
                    string sql = @" select distinct cu.COLUMN_name KEYNAME  from user_cons_columns cu, user_constraints au 
                            where cu.constraint_name = au.constraint_name
                            and au.constraint_type = 'P' and au.table_name = '" + tableName.ToUpper(IsUppper) + @"'";
                    var pks = this.Context.Ado.SqlQuery<string>(sql);
                    this.Context.Ado.IsEnableLogEvent = oldIsEnableLog;
                    return pks;
                });
        }

        public string GetTableComment(string tableName)
        {
            string cacheKey = "DbMaintenanceProvider.GetTableComment." + tableName;
            var comments = this.Context.Utilities.GetReflectionInoCacheInstance().GetOrCreate(cacheKey,
                () =>
                {
                    string sql = "SELECT COMMENTS FROM USER_TAB_COMMENTS WHERE TABLE_NAME =@tableName ORDER BY TABLE_NAME";
                    var oldIsEnableLog = this.Context.Ado.IsEnableLogEvent;
                    this.Context.Ado.IsEnableLogEvent = false;
                    var pks = this.Context.Ado.SqlQuery<string>(sql, new { tableName = tableName.ToUpper(IsUppper) });
                    this.Context.Ado.IsEnableLogEvent = oldIsEnableLog;
                    return pks;
                });
            return comments.HasValue() ? comments.First() : "";
        }

        public string GetFieldComment(string tableName, string filedName)
        {
            string cacheKey = "DbMaintenanceProvider.GetFieldComment." + tableName;
            var comments = this.Context.Utilities.GetReflectionInoCacheInstance().GetOrCreate(cacheKey,
                () =>
                {
                    string sql = "SELECT TVNAME AS TableName, COLNAME as DbColumnName ,COMMENT$ AS ColumnDescription   from SYSCOLUMNCOMMENTS   WHERE TVNAME='" + tableName.ToUpper(IsUppper) + "' ORDER BY TVNAME";
                    var oldIsEnableLog = this.Context.Ado.IsEnableLogEvent;
                    this.Context.Ado.IsEnableLogEvent = false;
                    var pks = this.Context.Ado.SqlQuery<DbColumnInfo>(sql);
                    this.Context.Ado.IsEnableLogEvent = oldIsEnableLog;
                    return pks;
                });
            if (comments.HasValue())
            {
                var comment = comments.FirstOrDefault(it => it.DbColumnName.Equals(filedName, StringComparison.CurrentCultureIgnoreCase));
                return comment?.ColumnDescription;
            }
            else
            {
                return "";
            }

        }

        public override bool CreateTable(string tableName, List<DbColumnInfo> columns, bool isCreatePrimaryKey = true)
        {
            if (columns.HasValue())
            {
                foreach (var item in columns)
                {
                    ConvertCreateColumnInfo(item);
                    if (item.DbColumnName.Equals("GUID", StringComparison.CurrentCultureIgnoreCase) && item.Length == 0)
                    {
                        item.Length = 10;
                    }
                }
            }
            string sql = GetCreateTableSql(tableName, columns);
            //string primaryKeyInfo = null;
            if (columns.Any(it => it.IsPrimarykey) && isCreatePrimaryKey)
            {
                sql= sql.TrimEnd(')')+ string.Format(", Primary key({0})", string.Join(",", columns.Where(it => it.IsPrimarykey).Select(it => this.SqlBuilder.GetTranslationColumnName(it.DbColumnName.ToUpper(IsUppper)))));
                sql = sql + ")";
            }
            //sql = sql.Replace("$PrimaryKey", primaryKeyInfo);
            this.Context.Ado.ExecuteCommand(sql);
            return true;
        }
        public static string ExtractSchema(string connectionString)
        {
            string pattern = @"(?i)(?:^|;)schema=(\w+)";
            Match match = Regex.Match(connectionString?.Replace(" ",""), pattern,RegexOptions.IgnoreCase);
            return match.Success ? match.Groups[1].Value : null;
        }
        public override bool IsAnyTable(string tableName, bool isCache = true)
        {
            var isSchema = this.Context.CurrentConnectionConfig?.ConnectionString?.Replace(" ","")?.ToLower()?.Contains("schema=") == true;
            if (isSchema)
            {
                var schema= ExtractSchema(this.Context.CurrentConnectionConfig?.ConnectionString);
                Check.ExceptionEasy(schema == null, "ConnectionString schema format error, please use schema=(\\w+)", "连接字符串schema格式错误,请用schema=(\\w+)");
                return this.Context.Ado.GetInt($@"SELECT COUNT(*)
FROM ALL_TABLES t
WHERE upper(t.TABLE_NAME) = upper('{tableName}')
  AND upper(t.OWNER) = upper('{schema}')
") > 0;
              
            }
            else {
                return base.IsAnyTable(tableName, isCache);
            }
        }
        #endregion

        #region Helper
        public bool IsUppper 
        {
            get
            {
                if (this.Context.CurrentConnectionConfig.MoreSettings == null)
                {
                    return true;
                }
                else
                {
                    return this.Context.CurrentConnectionConfig.MoreSettings.IsAutoToUpper == true;
                }
            }
        }
        private static void ConvertCreateColumnInfo(DbColumnInfo x)
        {
            string[] array = new string[] { "int" };
            if (array.Contains(x.DataType?.ToLower()))
            {
                x.Length = 0;
                x.DecimalDigits = 0;
            }
        }
        #endregion
    }
}