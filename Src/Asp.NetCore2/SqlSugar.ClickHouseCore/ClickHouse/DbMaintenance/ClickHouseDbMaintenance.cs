using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar.ClickHouse
{
    public class ClickHouseDbMaintenance : DbMaintenanceProvider
    {
        #region DML
        protected override string GetDataBaseSql
        {
            get
            {
                return "SELECT name FROM system.databases where name not in ('system','information_schema','INFORMATION_SCHEMA' )";
            }
        }
        protected override string GetColumnInfosByTableNameSql
        {
            get
            {
                string schema = GetSchema();
                string sql = @"select * from information_schema.columns a where lower(table_name) =lower('{0}')";
                return sql;
            }
        }

        protected override string GetTableInfoListSql
        {
            get
            {
                return @"SELECT name  FROM system.tables where   database not in('INFORMATION_SCHEMA','system','information_schema'  )";
            }
        }
        protected override string GetViewInfoListSql
        {
            get
            {
                return @"select cast(relname as varchar) as Name,cast(Description as varchar) from pg_description
                         join pg_class on pg_description.objoid = pg_class.oid
                         where objsubid = 0 and relname in (SELECT viewname from pg_views  
                         WHERE schemaname ='"+GetSchema()+"')";
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
                return "CREATE TABLE {0}(\r\n{1})";
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

        protected override string RenameTableSql => "alter table 表名 {0} to {1}";

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
                return "  Select count(1) from (SELECT to_regclass('{0}') as c ) t where t.c is not null";
            }
        }

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
                return " NULL";
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
        public override bool AddDefaultValue(string tableName, string columnName, string defaultValue)
        {
            return base.AddDefaultValue(this.SqlBuilder.GetTranslationTableName(tableName), this.SqlBuilder.GetTranslationTableName(columnName), defaultValue);
        }
        public override bool AddColumnRemark(string columnName, string tableName, string description)
        {
            tableName = this.SqlBuilder.GetTranslationTableName(tableName);
            string sql = string.Format(this.AddColumnRemarkSql, columnName, tableName, description);
            this.Context.Ado.ExecuteCommand(sql);
            return true;
        }
        public override bool AddTableRemark(string tableName, string description)
        {
            tableName = this.SqlBuilder.GetTranslationTableName(tableName);
            return base.AddTableRemark(tableName, description);
        }
        public override bool UpdateColumn(string tableName, DbColumnInfo columnInfo)
        {
            tableName = this.SqlBuilder.GetTranslationTableName(tableName);
            var columnName= this.SqlBuilder.GetTranslationColumnName(columnInfo.DbColumnName);
            string sql = GetUpdateColumnSql(tableName, columnInfo);
            this.Context.Ado.ExecuteCommand(sql);
            var isnull = columnInfo.IsNullable?" DROP NOT NULL ": " SET NOT NULL ";
            this.Context.Ado.ExecuteCommand(string.Format("alter table {0} alter {1} {2}",tableName,columnName, isnull));
            return true;
        }

        protected override string GetUpdateColumnSql(string tableName, DbColumnInfo columnInfo)
        {
            string columnName = this.SqlBuilder.GetTranslationColumnName(columnInfo.DbColumnName);
            tableName = this.SqlBuilder.GetTranslationTableName(tableName);
            string dataSize = GetSize(columnInfo);
            string dataType = columnInfo.DataType;
            if (!string.IsNullOrEmpty(dataType))
            {
                dataType = " type " + dataType;
            }
            string nullType = "";
            string primaryKey = null;
            string identity = null;
            string result = string.Format(this.AlterColumnToTableSql, tableName, columnName, dataType, dataSize, nullType, primaryKey, identity);
            return result;
        }

        /// <summary>
        ///by current connection string
        /// </summary>
        /// <param name="databaseDirectory"></param>
        /// <returns></returns>
        public override bool CreateDatabase(string databaseName, string databaseDirectory = null)
        {
            if (databaseDirectory != null)
            {
                if (!FileHelper.IsExistDirectory(databaseDirectory))
                {
                    FileHelper.CreateDirectory(databaseDirectory);
                }
            }
            var oldDatabaseName = this.Context.Ado.Connection.Database;
            var connection = this.Context.CurrentConnectionConfig.ConnectionString;
            connection = connection.Replace(oldDatabaseName, "postgres");
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
                    db.DbMaintenance.AddColumnRemark(item.DbColumnName, item.DbTableName, item.ColumnDescription);

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
            string sql = GetCreateTableSql(tableName, columns);
            this.Context.Ado.ExecuteCommand(sql);
            return true;
        }
        protected override string GetCreateTableSql(string tableName, List<DbColumnInfo> columns)
        {
            List<string> columnArray = new List<string>();
            Check.Exception(columns.IsNullOrEmpty(), "No columns found ");
            var pkName = "";
            foreach (var item in columns)
            {
                string columnName = item.DbColumnName;
                string dataType = item.DataType;
                if (dataType == "varchar" && item.Length == 0)
                {
                    item.Length = 1;
                }
                string dataSize =item.Length > 0 ? string.Format("({0})", item.Length) : "";
                if (item.DecimalDigits > 0&&item.Length>0 && dataType == "numeric") 
                {
                    dataSize = $"({item.Length},{item.DecimalDigits})";
                }
                string nullType = item.IsNullable ? this.CreateTableNull : CreateTableNotNull;
                string primaryKey = "";
                string addItem = string.Format(this.CreateTableColumn, this.SqlBuilder.GetTranslationColumnName(columnName.ToLower()), dataType, dataSize, nullType, primaryKey, "");
                columnArray.Add(addItem);
                if (pkName.IsNullOrEmpty()&&item.IsPrimarykey) 
                {
                    pkName = item.DbColumnName;
                }
            }
            string tableString = string.Format(this.CreateTableSql, this.SqlBuilder.GetTranslationTableName(tableName.ToLower()), string.Join(",\r\n", columnArray));
            if (pkName.HasValue())
            {
                pkName = this.SqlBuilder.GetTranslationColumnName(pkName);
                tableString += $"ENGINE = MergeTree()  ORDER BY ( {pkName} )  PRIMARY KEY {pkName} SETTINGS index_granularity = 8192";
            }
            else 
            {
                pkName = this.SqlBuilder.GetTranslationColumnName(columns.First().DbColumnName);
                tableString += $"ENGINE = MergeTree()  ORDER BY ( {pkName} )";
            }
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
            var result= base.GetColumnInfosByTableName(tableName.TrimEnd('"').TrimStart('"').ToLower(), isCache);
            if (result == null || result.Count() == 0)
            {
                result = base.GetColumnInfosByTableName(tableName, isCache);
            }
            try
            {
                string sql = $@"select  
                               kcu.column_name as key_column
                               from information_schema.table_constraints tco
                               join information_schema.key_column_usage kcu 
                               on kcu.constraint_name = tco.constraint_name
                               and kcu.constraint_schema = tco.constraint_schema
                               and kcu.constraint_name = tco.constraint_name
                               where tco.constraint_type = 'PRIMARY KEY'
                               and kcu.table_schema='{GetSchema()}' and 
                               upper(kcu.table_name)=upper('{tableName.TrimEnd('"').TrimStart('"')}')";
                List<string> pkList = new List<string>();
                if (isCache)
                {
                    pkList=GetListOrCache<string>("GetColumnInfosByTableName_N_Pk"+tableName, sql);
                }
                else
                {
                    pkList = this.Context.Ado.SqlQuery<string>(sql);
                }
                if (pkList.Count >1) 
                {
                    foreach (var item in result)
                    {
                        if (pkList.Select(it=>it.ToUpper()).Contains(item.DbColumnName.ToUpper())) 
                        {
                            item.IsPrimarykey = true;
                        }
                    }
                }
            }
            catch  
            {

            }
            return result;
        }
        #endregion

        #region Helper
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

            return schema;
        }

        #endregion
    }
}
