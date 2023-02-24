﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class KdbndpDbMaintenance : DbMaintenanceProvider
    {
        #region DML
        protected override string GetDataBaseSql
        {
            get
            {
                return "SELECT datname FROM sys_database";
            }
        }
        protected override string GetColumnInfosByTableNameSql
        {
            get
            {
                string sql = $@"select cast (pclass.oid as int4) as TableId,cast(ptables.tablename as varchar) as TableName,
                                pcolumn.column_name as DbColumnName,pcolumn.udt_name as DataType,
                                pcolumn.character_maximum_length as Length,
                                pcolumn.column_default as DefaultValue,
                                col_description(pclass.oid, pcolumn.ordinal_position) as ColumnDescription,
                                case when pkey.colname = pcolumn.column_name
                                then true else false end as IsPrimaryKey,
                                case when UPPER(pcolumn.column_default) like 'NEXTVAL%'
                                then true else false end as IsIdentity,
                                case when UPPER(pcolumn.is_nullable) = 'YES'
                                then true else false end as IsNullable
                                 from (select * from sys_tables where  UPPER(tablename) = UPPER('{{0}}') and  lower(schemaname)='{GetSchema()}') ptables inner join sys_class pclass
                                on ptables.tablename = pclass.relname inner join (SELECT *
                                FROM information_schema.columns
                                ) pcolumn on pcolumn.table_name = ptables.tablename
                                left join (
	                                select  sys_class.relname,sys_attribute.attname as colname from 
	                                sys_constraint  inner join sys_class 
	                                on sys_constraint.conrelid = sys_class.oid 
	                                inner join sys_attribute on sys_attribute.attrelid = sys_class.oid 
	                                and  sys_attribute.attnum = sys_constraint.conkey[1]
	                                inner join sys_type on sys_type.oid = sys_attribute.atttypid
	                                where sys_constraint.contype='p'
                                ) pkey on pcolumn.table_name = pkey.relname
                                order by ptables.tablename";
                return sql;
            }
        }
        protected override string GetTableInfoListSql
        {
            get
            {
                return @"select cast(relname as varchar) as Name,
                        cast(obj_description(relfilenode,'sys_class') as varchar) as Description from sys_class c 
                        where  relkind = 'r' and  c.oid > 16384 and c.relnamespace != 99 and c.relname not like '%pl_profiler_saved%' order by relname";
            }
        }
        protected override string GetViewInfoListSql
        {
            get
            {
                return @"select cast(relname as varchar) as Name,cast(Description as varchar) from sys_description
                         join sys_class on sys_description.objoid = sys_class.oid
                         where objsubid = 0 and relname in (SELECT viewname from sys_views  
                         WHERE schemaname ='public')";
            }
        }
        #endregion

        #region DDL
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
                return "ALTER TABLE {0} ADD COLUMN {1} {2}{3} {4} {5} {6}";
            }
        }
        protected override string AlterColumnToTableSql
        {
            get
            {
                return "alter table {0} ALTER COLUMN {1} {2}{3} {4} {5} {6}";
            }
        }
        protected override string BackupDataBaseSql
        {
            get
            {
                return "mysqldump.exe  {0} -uroot -p > {1}  ";
            }
        }
        protected override string CreateTableSql
        {
            get
            {
                return "CREATE TABLE {0}(\r\n{1} $PrimaryKey)";
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
                return "create table {0} as (select * from {1} limit {2} offset 0)";
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
                return "ALTER TABLE {0} DROP CONSTRAINT {1}";
            }
        }
        protected override string RenameColumnSql
        {
            get
            {
                return "ALTER TABLE {0} RENAME {1} TO {2}";
            }
        }
        protected override string AddColumnRemarkSql => "comment on column {1}.{0} is '{2}'";

        protected override string DeleteColumnRemarkSql => "comment on column {1}.{0} is ''";

        protected override string IsAnyColumnRemarkSql { get { throw new NotSupportedException(); } }

        protected override string AddTableRemarkSql => "comment on table {0} is '{1}'";

        protected override string DeleteTableRemarkSql => "comment on table {0} is ''";

        protected override string IsAnyTableRemarkSql { get { throw new NotSupportedException(); } }

        protected override string RenameTableSql => "alter table  {0} to {1}";

        protected override string CreateIndexSql
        {
            get
            {
                return "CREATE {3} INDEX Index_{0}_{2} ON {0} ({1})";
            }
        }
        protected override string AddDefaultValueSql
        {
            get
            {
                return "ALTER TABLE {0} ALTER COLUMN {1} SET DEFAULT {2}";
            }
        }
        protected override string IsAnyIndexSql
        {
            get
            {
                return "  Select count(1) from (SELECT to_regclass('Index_UnitCodeTest1_Id_CreateDate') as c ) t where t.c is not null";
            }
        }
        protected override string IsAnyProcedureSql => throw new NotImplementedException();
        #endregion

        #region Check
        protected override string CheckSystemTablePermissionsSql
        {
            get
            {
                return "select 1 from information_schema.columns limit 1 offset 0";
            }
        }
        #endregion

        #region Scattered
        protected override string CreateTableNull
        {
            get
            {
                return "DEFAULT NULL";
            }
        }
        protected override string CreateTableNotNull
        {
            get
            {
                return "NOT NULL";
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
                return "serial";
            }
        }
        #endregion

        #region Methods
        private string GetSchema()
        {
            var schema = "public";
            if (System.Text.RegularExpressions.Regex.IsMatch(this.Context.CurrentConnectionConfig.ConnectionString.ToLower(), "searchpath="))
            {
                var regValue = System.Text.RegularExpressions.Regex.Match(this.Context.CurrentConnectionConfig.ConnectionString.ToLower(), @"searchpath\=(\w+)").Groups[1].Value;
                if (regValue.HasValue())
                {
                    schema = regValue;
                }
            }
            else if (System.Text.RegularExpressions.Regex.IsMatch(this.Context.CurrentConnectionConfig.ConnectionString.ToLower(), "search path="))
            {
                var regValue = System.Text.RegularExpressions.Regex.Match(this.Context.CurrentConnectionConfig.ConnectionString.ToLower(), @"search path\=(\w+)").Groups[1].Value;
                if (regValue.HasValue())
                {
                    schema = regValue;
                }
            }

            return schema.ToLower();
        }
        public override bool UpdateColumn(string tableName, DbColumnInfo columnInfo)
        {

            tableName = this.SqlBuilder.GetTranslationTableName(tableName);
            var columnName = this.SqlBuilder.GetTranslationColumnName(columnInfo.DbColumnName);
            string type = GetType(tableName, columnInfo);
            //this.Context.Ado.ExecuteCommand(sql);

            string sql = @"ALTER TABLE {table} ALTER  {column} TYPE {type};ALTER TABLE {table} ALTER COLUMN {column} {null}";

            var isnull = columnInfo.IsNullable ? " DROP NOT NULL " : " SET NOT NULL ";

            sql = sql.Replace("{table}", tableName)
                .Replace("{type}", type)
                .Replace("{column}", columnName)
                .Replace("{null}", isnull);
            this.Context.Ado.ExecuteCommand(sql);
            return true;
        }
        protected string GetType(string tableName, DbColumnInfo columnInfo)
        {
            string columnName = this.SqlBuilder.GetTranslationColumnName(columnInfo.DbColumnName);
            tableName = this.SqlBuilder.GetTranslationTableName(tableName);
            string dataSize = GetSize(columnInfo);
            string dataType = columnInfo.DataType;
            //if (!string.IsNullOrEmpty(dataType))
            //{
            //    dataType =  dataType;
            //}
            return dataType + "" + dataSize;
        }
        //public override bool IsAnyColumn(string tableName, string columnName, bool isCache = true)
        //{
        //    var sql =
        //        $"select count(*) from information_schema.columns WHERE table_schema = '{GetSchema()}'  and UPPER(table_name) = '{tableName.ToUpper(IsUpper)}' and UPPER(column_name) = '{columnName.ToUpper(IsUpper)}'";
        //    return this.Context.Ado.GetInt(sql) > 0;
        //}

        public override bool IsAnyTable(string tableName, bool isCache = true)
        {
            var sql = $"select count(*) from information_schema.tables where UPPER(table_schema)=UPPER('{GetSchema()}') and UPPER(table_type)=UPPER('BASE TABLE') and UPPER(table_name)=UPPER('{tableName.ToUpper(IsUpper)}')";
            return this.Context.Ado.GetInt(sql)>0;
        }

        /// <summary>
        ///by current connection string
        /// </summary>
        /// <param name="databaseDirectory"></param>
        /// <returns></returns>
        public override bool CreateDatabase(string databaseName, string databaseDirectory = null)
        {
            if (this.Context.Ado.IsValidConnection())
            {
                return true;
            }
            if (databaseDirectory != null)
            {
                if (!FileHelper.IsExistDirectory(databaseDirectory))
                {
                    FileHelper.CreateDirectory(databaseDirectory);
                }
            }
            var oldDatabaseName = this.Context.Ado.Connection.Database;
            var connection = this.Context.CurrentConnectionConfig.ConnectionString;
            connection = connection.Replace(oldDatabaseName, "test");
            var newDb = new SqlSugarClient(new ConnectionConfig()
            {
                DbType = this.Context.CurrentConnectionConfig.DbType,
                IsAutoCloseConnection = true,
                ConnectionString = connection
            });
            if (!GetDataBaseList(newDb).Any(it => it.Equals(databaseName, StringComparison.CurrentCultureIgnoreCase)))
            {
                newDb.Ado.ExecuteCommand(string.Format(CreateDataBaseSql, this.SqlBuilder.SqlTranslationLeft+databaseName+this.SqlBuilder.SqlTranslationRight, databaseDirectory));
            }
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
                    db.DbMaintenance.AddColumnRemark(SqlBuilder.GetTranslationColumnName(item.DbColumnName).ToUpper(IsUpper), SqlBuilder.GetTranslationColumnName(item.DbTableName).ToUpper(IsUpper), item.ColumnDescription);

                }
            }
            //table remak
            if (entity.TableDescription != null)
            {
                db.DbMaintenance.AddTableRemark(entity.DbTableName, entity.TableDescription);
            }
            return true;
        }
        public override bool CreateTable(string tableName, List<DbColumnInfo> columns, bool isCreatePrimaryKey = true)
        {
            if (columns.HasValue())
            {
                foreach (var item in columns)
                {
                    if (item.DbColumnName.Equals("GUID", StringComparison.CurrentCultureIgnoreCase) && item.Length == 0)
                    {
                        item.Length = 10;
                    }
                }
            }
            string sql = GetCreateTableSql(tableName, columns);
            string primaryKeyInfo = null;
            if (columns.Any(it => it.IsPrimarykey) && isCreatePrimaryKey)
            {
                primaryKeyInfo = string.Format(", Primary key({0})", string.Join(",", columns.Where(it => it.IsPrimarykey).Select(it => this.SqlBuilder.GetTranslationColumnName(it.DbColumnName.ToUpper(IsUpper)))));

            }
            sql = sql.Replace("$PrimaryKey", primaryKeyInfo);
            this.Context.Ado.ExecuteCommand(sql);
            return true;
        }
        protected override string GetCreateTableSql(string tableName, List<DbColumnInfo> columns)
        {
            List<string> columnArray = new List<string>();
            Check.Exception(columns.IsNullOrEmpty(), "No columns found ");
            foreach (var item in columns)
            {
                string columnName = item.DbColumnName;
                string dataType = item.DataType;
                if (dataType == "varchar" && item.Length == 0)
                {
                    item.Length = 1;
                }
                if (dataType == "uuid")
                {
                    item.Length = 50;
                    dataType = "varchar";
                }
                string dataSize = item.Length > 0 ? string.Format("({0})", item.Length) : null;
                string nullType = item.IsNullable ? this.CreateTableNull : CreateTableNotNull;
                string primaryKey = null;
                string addItem = string.Format(this.CreateTableColumn, this.SqlBuilder.GetTranslationColumnName(columnName.ToUpper(IsUpper)), dataType, dataSize, nullType, primaryKey, "");
                if (item.IsIdentity)
                {
                    string length = dataType.Substring(dataType.Length - 1);
                    string identityDataType = "serial" + length;
                    addItem = addItem.Replace(dataType, identityDataType);
                }
                columnArray.Add(addItem);
            }
            string tableString = string.Format(this.CreateTableSql, this.SqlBuilder.GetTranslationTableName(tableName.ToUpper(IsUpper)), string.Join(",\r\n", columnArray));
            return tableString;
        }
        public override bool IsAnyConstraint(string constraintName)
        {
            throw new NotSupportedException("PgSql IsAnyConstraint NotSupportedException");
        }
        public override bool BackupDataBase(string databaseName, string fullFileName)
        {
            Check.ThrowNotSupportedException("PgSql BackupDataBase NotSupported");
            return false;
        }

        public override List<DbColumnInfo> GetColumnInfosByTableName(string tableName, bool isCache = true)
        {
            return base.GetColumnInfosByTableName(tableName.TrimEnd('"').TrimStart('"').ToLower(), isCache);
        }
        public bool IsUpper
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
        #endregion
    }
}
