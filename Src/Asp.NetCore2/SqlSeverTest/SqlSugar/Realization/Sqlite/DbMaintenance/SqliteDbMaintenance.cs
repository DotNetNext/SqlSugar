using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Sqlite;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class SqliteDbMaintenance : DbMaintenanceProvider
    {
        #region DML
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
                return @"select Name from Sqlite_master where type='table' and name<>'Sqlite_sequence' and name<>'sqlite_sequence' order by name;";
            }
        }
        protected override string GetViewInfoListSql
        {
            get
            {
                return @"select Name from Sqlite_master where type='view'  order by name;";
            }
        }
        #endregion

        #region DDL
        protected override string AddPrimaryKeySql
        {
            get
            {
                throw new NotSupportedException();
            }
        }
        protected override string AddColumnToTableSql
        {
            get
            {
                throw new NotSupportedException();
            }
        }
        protected override string AlterColumnToTableSql
        {
            get
            {
                // return "ALTER TABLE {0} ALTER COLUMN {1} {2}{3} {4} {5} {6}";
                throw new NotSupportedException();
            }
        }
        protected override string BackupDataBaseSql
        {
            get
            {
                throw new NotSupportedException();
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
                return "DELETE FROM {0}";
            }
        }
        protected override string BackupTableSql
        {
            get
            {
                return " CREATE TABLE {0} AS SELECT * FROM {1} limit 0,{2}";
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
                throw new NotSupportedException();
            }
        }
        protected override string DropConstraintSql
        {
            get
            {
                throw new NotSupportedException();
            }
        }
        protected override string RenameColumnSql
        {
            get
            {
                throw new NotSupportedException();
            }
        }
        #endregion

        #region Check
        protected override string CheckSystemTablePermissionsSql
        {
            get
            {
                return "select Name from Sqlite_master limit 0,1";
            }
        }
        #endregion

        #region Scattered
        protected override string CreateTableNull
        {
            get
            {
                return "NULL";
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
                return "AUTOINCREMENT";
            }
        }
        #endregion

        #region Methods
        public override List<DbColumnInfo> GetColumnInfosByTableName(string tableName,bool isCache=true)
        {
            string cacheKey = "DbMaintenanceProvider.GetColumnInfosByTableName." + this.SqlBuilder.GetNoTranslationColumnName(tableName).ToLower();
            cacheKey = GetCacheKey(cacheKey);
            if (isCache)
            {
                return this.Context.Utilities.GetReflectionInoCacheInstance().GetOrCreate<List<DbColumnInfo>>(cacheKey, () =>
                {
                    return GetColumnInfosByTableName(tableName);

                });
            }
            else {
                return GetColumnInfosByTableName(tableName);
            }
        }

        private List<DbColumnInfo> GetColumnInfosByTableName(string tableName)
        {
            string sql = "PRAGMA table_info(" + tableName + ")";
            var oldIsEnableLog = this.Context.Ado.IsEnableLogEvent;
            this.Context.Ado.IsEnableLogEvent = false;
            using (DbDataReader dataReader = (SqliteDataReader)this.Context.Ado.GetDataReader(sql))
            {
                List<DbColumnInfo> result = new List<DbColumnInfo>();
                while (dataReader.Read())
                {
                    var type = dataReader.GetValue(2).ObjToString();
                    var length = 0;
                    if (type.Contains("("))
                    {
                        type = type.Split('(').First();
                        length = type.Split('(').Last().TrimEnd(')').ObjToInt();
                    }
                    DbColumnInfo column = new DbColumnInfo()
                    {
                        TableName = tableName,
                        DataType = type,
                        IsNullable = !dataReader.GetBoolean(3),
                        IsIdentity = dataReader.GetBoolean(3) && dataReader.GetBoolean(5).ObjToBool() && (type.IsIn("integer", "int", "int32", "int64", "long")),
                        ColumnDescription = null,
                        DbColumnName = dataReader.GetString(1),
                        DefaultValue = dataReader.GetValue(4).ObjToString(),
                        IsPrimarykey = dataReader.GetBoolean(5).ObjToBool(),
                        Length = length
                    };
                    result.Add(column);
                }
                return result;
            }
        }

        public override bool CreateTable(string tableName, List<DbColumnInfo> columns,bool isCreatePrimaryKey=true)
        {
            if (columns.HasValue())
            {
                foreach (var item in columns)
                {
                    if (item.DbColumnName.Equals("GUID", StringComparison.CurrentCultureIgnoreCase))
                    {
                        item.Length = 20;
                    }
                    if (item.IsIdentity && !item.IsPrimarykey)
                    {
                        item.IsPrimarykey = true;
                        Check.Exception(item.DataType == "integer", "Identity only integer type");
                    }
                }
            }
            string sql = GetCreateTableSql(tableName, columns);
            if (!isCreatePrimaryKey)
            {
                sql = sql.Replace("PRIMARY KEY AUTOINCREMENT", "").Replace("PRIMARY KEY", "");
            }
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
                string dataSize = item.Length > 0 ? string.Format("({0})", item.Length) : null;
                string nullType = item.IsNullable ? this.CreateTableNull : CreateTableNotNull;
                string primaryKey = item.IsPrimarykey?this.CreateTablePirmaryKey:null;
                string identity = item.IsIdentity ? this.CreateTableIdentity : null;
                string addItem = string.Format(this.CreateTableColumn, this.SqlBuilder.GetTranslationColumnName(columnName), dataType, dataSize, nullType, primaryKey, identity);
                columnArray.Add(addItem);
            }
            string tableString = string.Format(this.CreateTableSql, this.SqlBuilder.GetTranslationTableName(tableName), string.Join(",\r\n", columnArray));
            tableString = tableString.Replace("`", "\"");
            return tableString;
        }
        public override bool IsAnyConstraint(string constraintName)
        {
            throw new NotSupportedException("MySql IsAnyConstraint NotSupportedException");
        }
        public override bool BackupDataBase(string databaseName, string fullFileName)
        {
            Check.ThrowNotSupportedException("MySql BackupDataBase NotSupported");
            return false;
        }
        private List<T> GetListOrCache<T>(string cacheKey, string sql)
        {
            return this.Context.Utilities.GetReflectionInoCacheInstance().GetOrCreate<List<T>>(cacheKey, () =>
             {
                 var isEnableLogEvent = this.Context.Ado.IsEnableLogEvent;
                 this.Context.Ado.IsEnableLogEvent = false;
                 var reval = this.Context.Ado.SqlQuery<T>(sql);
                 this.Context.Ado.IsEnableLogEvent = isEnableLogEvent;
                 return reval;
             });
        }
        #endregion
    }
}
