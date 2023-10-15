using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlSugar
{
    public class QuestDBDbMaintenance : DbMaintenanceProvider
    {
        #region DML
        protected override string GetDataBaseSql
        {
            get
            {
                return CreateDataBaseSql;
            }
        }
        protected override string GetColumnInfosByTableNameSql
        {
            get
            {
                string schema = GetSchema();
                string sql = @"SHOW COLUMNS FROM {0}";
                return sql;
            }
        }

        protected override string GetTableInfoListSql
        {
            get
            {
                var schema = GetSchema();
                return @"SHOW TABLES";
            }
        }
        protected override string GetViewInfoListSql
        {
            get
            {
                return "select * from (select 1 as id) t where id=0";
            }
        }
        #endregion

        #region DDL
        protected override string CreateDataBaseSql
        {
            get
            {
                return "select * from (select 1 as id) t where id=0";
            }
        }
        protected override string AddPrimaryKeySql
        {
            get
            {
                return "";
            }
        }
        protected override string AddColumnToTableSql
        {
            get
            {
                return "ALTER TABLE {0} ADD COLUMN {1} {2}";
            }
        }
        protected override string AlterColumnToTableSql
        {
            get
            {
                throw new NotSupportedException("Alter Column ");
            }
        }
        protected override string BackupDataBaseSql
        {
            get
            {
                return "BACKUP DATABASE";
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
                return " ";
            }
        }
        protected override string RenameColumnSql
        {
            get
            {
                return "ALTER TABLE {0} RENAME COLUMN {1} TO {2}";
            }
        }
        protected override string AddColumnRemarkSql => " ";

        protected override string DeleteColumnRemarkSql => " ";

        protected override string IsAnyColumnRemarkSql { get { throw new NotSupportedException(); } }

        protected override string AddTableRemarkSql => " ";

        protected override string DeleteTableRemarkSql => " ";

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
        protected override string IsAnyProcedureSql => throw new NotImplementedException();
        #endregion

        #region Check
        protected override string CheckSystemTablePermissionsSql
        {
            get
            {
                return "select 1 ";
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
        public override void AddIndex(EntityInfo entityInfo)
        {
            if (entityInfo.Indexs != null)
            {
                foreach (var item in entityInfo.Indexs)
                { 
                    CreateIndex(entityInfo.DbTableName, item.IndexFields.Select(it => it.Key).ToArray(),item.IsUnique);
                }
            }
        }
        public override bool CreateIndex(string tableName, string[] columnNames, bool isUnique = false)
        {
            if (isUnique)
            {
                this.Context.Ado.ExecuteCommand($"ALTER TABLE {tableName} DEDUP ENABLE UPSERT KEYS({string.Join(",",columnNames)})");
                return true;
            }
            var columnInfos = this.Context.Ado.SqlQuery<QuestDbColumn>("SHOW COLUMNS FROM  '" + tableName + "'");
            foreach (var columnInfo in columnInfos)
            {
                if (columnNames.Any(z => z.EqualCase(columnInfo.Column)))
                {
                    if (!columnInfo.Type.EqualCase("SYMBOL"))
                    {
                        Check.ExceptionEasy(true, "Only the SYMBOL type can be indexed", $"字段{columnInfo.Column} 不是SYMBOL并且实体是string才能添加索引,CodeFirst需要指定类型： SYMBOL");
                    }
                    if (columnInfo.Indexed == false)
                    {
                        var indexSql = $"ALTER TABLE  '{tableName}'  ALTER COLUMN  {columnInfo.Column}   ADD INDEX ";
                        this.Context.Ado.ExecuteCommand(indexSql);
                    }
                }
            }
            return true;
        }
        public override bool CreateIndex(string tableName, string[] columnNames, string IndexName, bool isUnique = false)
        {
            if(isUnique)
                throw new Exception("no support  unique index");
            return  CreateIndex(tableName, columnNames, isUnique);
        }

        public override bool CreateUniqueIndex(string tableName, string[] columnNames)
        {
            throw new Exception("no support  unique index");
        }
        public override bool IsAnyIndex(string indexName)
        {
            return false;
        }
        public override List<DbTableInfo> GetTableInfoList(bool isCache = true)
        {
            var dt = this.Context.Ado.GetDataTable(GetTableInfoListSql);
            List<DbTableInfo> result = new List<DbTableInfo>();
            foreach (System.Data.DataRow dr in dt.Rows)
            {
                DbTableInfo di = new DbTableInfo();
                di.Name = dr[0] + "";
                if (!di.Name.Contains("sys.") && !di.Name.IsIn("telemetry", "telemetry_config") )
                {
                    result.Add(di);
                }

            }
            return result;
        }
        public override bool AddDefaultValue(string tableName, string columnName, string defaultValue)
        {
            return base.AddDefaultValue(this.SqlBuilder.GetTranslationTableName(tableName), this.SqlBuilder.GetTranslationTableName(columnName), defaultValue);
        }
        public override bool AddColumnRemark(string columnName, string tableName, string description)
        {
            //tableName = this.SqlBuilder.GetTranslationTableName(tableName);
            //string sql = string.Format(this.AddColumnRemarkSql, columnName, tableName, description);
            //this.Context.Ado.ExecuteCommand(sql);
            return true;
        }
        public override bool AddTableRemark(string tableName, string description)
        {
            //tableName = this.SqlBuilder.GetTranslationTableName(tableName);
            //return base.AddTableRemark(tableName, description);
            return true;
        }
        public override bool UpdateColumn(string tableName, DbColumnInfo columnInfo)
        {
            //no support
            return false;
        }
        public override bool AddPrimaryKey(string tableName, string columnName)
        {
            return true;
        }
        //protected override string GetUpdateColumnSql(string tableName, DbColumnInfo columnInfo)
        //{
        //    string columnName = this.SqlBuilder.GetTranslationColumnName(columnInfo.DbColumnName);
        //    tableName = this.SqlBuilder.GetTranslationTableName(tableName);
        //    string dataSize = GetSize(columnInfo);
        //    string dataType = columnInfo.DataType;
        //    if (!string.IsNullOrEmpty(dataType))
        //    {
        //        dataType = " type " + dataType;
        //    }
        //    string nullType = "";
        //    string primaryKey = null;
        //    string identity = null;
        //    string result = string.Format(this., tableName, columnName, dataType, dataSize, nullType, primaryKey, identity);
        //    return result;
        //}

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
            var splitSql = "";
            if (tableName.Contains("_TIMESTAMP(")) 
            {
                splitSql = Regex.Match(tableName,@"_TIMESTAMP\(.+$").Value;
                tableName = tableName.Replace(splitSql, "");
            }
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
                primaryKeyInfo = string.Format(", Primary key({0})", string.Join(",", columns.Where(it => it.IsPrimarykey).Select(it => this.SqlBuilder.GetTranslationColumnName(it.DbColumnName.ToLower()))));

            }
            sql = sql.Replace("$PrimaryKey", primaryKeyInfo);
            this.Context.Ado.ExecuteCommand(sql+ splitSql.TrimStart('_'));
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
                //if (dataType == "varchar" && item.Length == 0)
                //{
                //    item.Length = 1;
                //}
                //if (dataType == "uuid")
                //{
                //    item.Length = 50;
                //    dataType = "varchar";
                //}
                string dataSize = "";
                //if (item.DecimalDigits > 0&&item.Length>0 && dataType == "numeric") 
                //{
                //    dataSize = $"({item.Length},{item.DecimalDigits})";
                //}
                string nullType = "";
                string primaryKey = null;
                string addItem = string.Format(this.CreateTableColumn, this.SqlBuilder.GetTranslationColumnName(columnName.ToLower()), dataType, dataSize, nullType, primaryKey, "");
                //if (item.IsIdentity)
                //{
                //    string length = dataType.Substring(dataType.Length - 1);
                //    string identityDataType = "serial" + length;
                //    addItem = addItem.Replace(dataType, identityDataType);
                //}
                columnArray.Add(addItem);
            }
            string tableString = string.Format(this.CreateTableSql, this.SqlBuilder.GetTranslationTableName(tableName), string.Join(",\r\n", columnArray));
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
            var sql =  String.Format(GetColumnInfosByTableNameSql,tableName);
            List<DbColumnInfo> result = new List<DbColumnInfo>();
            var dt = this.Context.Ado.GetDataTable(sql);
            foreach (System.Data.DataRow column in dt.Rows) 
            {
                DbColumnInfo dbColumnInfo = new DbColumnInfo();
                dbColumnInfo.DbColumnName = column[0]+"";
                dbColumnInfo.DataType = column[1]+"";
                result.Add(dbColumnInfo);
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

        #region HelperClass
        internal class QuestDbColumn
        {
            public string Column  { get; set; }
            public string Type { get; set; }
            public bool Indexed { get; set; }
        }
        #endregion
    }
}
