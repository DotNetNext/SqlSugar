﻿using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlSugar
{
    public class SqliteDbMaintenance : DbMaintenanceProvider
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
                return @"select Name from sqlite_master where type='table' and name<>'sqlite_sequence' order by name;";
            }
        }
        protected override string GetViewInfoListSql
        {
            get
            {
                return @"select Name from sqlite_master where type='view'  order by name;";
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
                throw new NotSupportedException();
            }
        }
        protected override string AddColumnToTableSql
        {
            get
            {
                return "ALTER TABLE {0} ADD COLUMN {1} {2}{3}";
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
                return "select Name from sqlite_master limit 0,1";
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

        protected override string AddColumnRemarkSql
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        protected override string DeleteColumnRemarkSql
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        protected override string IsAnyColumnRemarkSql
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        protected override string AddTableRemarkSql
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        protected override string DeleteTableRemarkSql
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        protected override string IsAnyTableRemarkSql
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        protected override string RenameTableSql
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        protected override string CreateIndexSql => throw new NotSupportedException();

        protected override string IsAnyIndexSql => throw new NotSupportedException();

        protected override string AddDefaultValueSql => throw new NotSupportedException();
        #endregion

        #region Methods
        /// <summary>
        ///by current connection string
        /// </summary>
        /// <param name="databaseDirectory"></param>
        /// <returns></returns>
        public override bool CreateDatabase(string databaseName, string databaseDirectory = null)
        {
            var connString = this.Context.CurrentConnectionConfig.ConnectionString;
            var path = Regex.Match(connString, @"[a-z,A-Z]\:\\.+\\").Value;
            if (path.IsNullOrEmpty())
            {
                path = Regex.Match(connString, @"\/.+\/").Value;
            }
            if (!FileHelper.IsExistDirectory(path))
            {
                FileHelper.CreateDirectory(path);
            }
            this.Context.Ado.Connection.Open();
            this.Context.Ado.Connection.Close();
            return true;
        }
        public override List<DbColumnInfo> GetColumnInfosByTableName(string tableName, bool isCache = true)
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
            else
            {
                return GetColumnInfosByTableName(tableName);
            }
        }

        private List<DbColumnInfo> GetColumnInfosByTableName(string tableName)
        {
            var columns = GetColumnsByTableName2(tableName);
            string sql = "PRAGMA table_info(" +SqlBuilder.GetTranslationTableName(tableName) + ")";
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
                    bool isIdentity = columns.FirstOrDefault(it => it.DbColumnName.Equals(dataReader.GetString(1),StringComparison.CurrentCultureIgnoreCase)).IsIdentity;
                    DbColumnInfo column = new DbColumnInfo()
                    {
                        TableName = tableName,
                        DataType = type,
                        IsNullable = !dataReader.GetBoolean(3),
                        IsIdentity = isIdentity,
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
        private List<DbColumnInfo> GetColumnsByTableName2(string tableName)
        {
            tableName = SqlBuilder.GetTranslationTableName(tableName);
            string sql = "select * from " + tableName + " limit 0,1";
            var oldIsEnableLog = this.Context.Ado.IsEnableLogEvent;
            this.Context.Ado.IsEnableLogEvent = false;
            using (DbDataReader reader = (SqliteDataReader)this.Context.Ado.GetDataReader(sql))
            {
                this.Context.Ado.IsEnableLogEvent = oldIsEnableLog;
                List<DbColumnInfo> result = new List<DbColumnInfo>();
                var schemaTable = reader.GetSchemaTable();
                foreach (DataRow row in schemaTable.Rows)
                {
                    DbColumnInfo column = new DbColumnInfo()
                    {
                        TableName = tableName,
                        IsIdentity = (bool)row["IsAutoIncrement"],
                        DbColumnName = row["ColumnName"].ToString(),
                    };
                    result.Add(column);
                }
                return result;
            }
        }
        public override bool AddRemark(EntityInfo entity)
        {
            return true;
        }
  

        public override bool BackupTable(string oldTableName, string newTableName, int maxBackupDataRows = int.MaxValue)
        {
            oldTableName = this.SqlBuilder.GetTranslationTableName(oldTableName);
            newTableName = this.SqlBuilder.GetTranslationTableName(newTableName);
            string sql = string.Format(this.BackupTableSql, newTableName, oldTableName, maxBackupDataRows);
            this.Context.Ado.ExecuteCommand(sql);
            return true;
        }
        public override bool CreateTable(string tableName, List<DbColumnInfo> columns, bool isCreatePrimaryKey = true)
        {
            if (columns.HasValue())
            {
                foreach (var item in columns)
                {
                    //if (item.DbColumnName.Equals("GUID", StringComparison.CurrentCultureIgnoreCase))
                    //{
                    //    item.Length = 20;
                    //}
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
                string primaryKey = item.IsPrimarykey ? this.CreateTablePirmaryKey : null;
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
            return this.Context.Utilities.GetReflectionInoCacheInstance().GetOrCreate(cacheKey,
             () =>
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
