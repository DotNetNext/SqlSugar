using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public abstract partial class DbMaintenanceProvider : IDbMaintenance
    {
        #region DML
        public virtual List<DbTableInfo> GetViewInfoList()
        {
            string cacheKey = "DbMaintenanceProvider.GetViewInfoList";
            cacheKey = GetCacheKey(cacheKey);
            var result = GetListOrCache<DbTableInfo>(cacheKey, this.GetViewInfoListSql);
            foreach (var item in result)
            {
                item.DbObjectType = DbObjectType.View;
            }
            return result;
        }
        public virtual List<DbTableInfo> GetTableInfoList()
        {
            string cacheKey = "DbMaintenanceProvider.GetTableInfoList";
            cacheKey = GetCacheKey(cacheKey);
            var result = GetListOrCache<DbTableInfo>(cacheKey, this.GetTableInfoListSql);
            foreach (var item in result)
            {
                item.DbObjectType = DbObjectType.Table;
            }
            return result;
        }
        public virtual List<DbColumnInfo> GetColumnInfosByTableName(string tableName)
        {
            if (string.IsNullOrEmpty(tableName)) return new List<DbColumnInfo>();
            string cacheKey = "DbMaintenanceProvider.GetColumnInfosByTableName." + this.SqlBuilder.GetNoTranslationColumnName(tableName).ToLower();
            cacheKey = GetCacheKey(cacheKey);
            return GetListOrCache<DbColumnInfo>(cacheKey, string.Format(this.GetColumnInfosByTableNameSql, tableName));
        }
        public virtual List<string> GetIsIdentities(string tableName)
        {
            string cacheKey = "DbMaintenanceProvider.GetIsIdentities" + this.SqlBuilder.GetNoTranslationColumnName(tableName).ToLower();
            cacheKey = GetCacheKey(cacheKey);
            return this.Context.RewritableMethods.GetCacheInstance<List<string>>().Func(cacheKey,
                    (cm, key) =>
                    {
                        return cm[cacheKey];

                    }, (cm, key) =>
                    {
                        var result = GetColumnInfosByTableName(tableName).Where(it => it.IsIdentity).ToList();
                        return result.Select(it => it.DbColumnName).ToList();
                    });
        }
        public virtual List<string> GetPrimaries(string tableName)
        {
            string cacheKey = "DbMaintenanceProvider.GetPrimaries" + this.SqlBuilder.GetNoTranslationColumnName(tableName).ToLower();
            cacheKey = GetCacheKey(cacheKey);
            return this.Context.RewritableMethods.GetCacheInstance<List<string>>().Func(cacheKey,
            (cm, key) =>
            {
                return cm[cacheKey];

            }, (cm, key) =>
            {
                var result = GetColumnInfosByTableName(tableName).Where(it => it.IsPrimarykey).ToList();
                return result.Select(it => it.DbColumnName).ToList();
            });
        }
        #endregion

        #region Check
        public virtual bool IsAnyTable(string tableName)
        {
            tableName = this.SqlBuilder.GetNoTranslationColumnName(tableName);
            var tables = GetTableInfoList();
            if (tables == null) return false;
            else return tables.Any(it => it.Name.Equals(tableName, StringComparison.CurrentCultureIgnoreCase));
        }
        public virtual bool IsAnyColumn(string tableName, string columnName)
        {
            columnName = this.SqlBuilder.GetTranslationColumnName(columnName);
            tableName = this.SqlBuilder.GetTranslationTableName(tableName);
            var isAny = IsAnyTable(tableName);
            Check.Exception(!isAny, string.Format("Table {0} does not exist", tableName));
            var columns = GetColumnInfosByTableName(tableName);
            if (columns.IsNullOrEmpty()) return false;
            return columns.Any(it => it.DbColumnName.Equals(columnName, StringComparison.CurrentCultureIgnoreCase));
        }
        public virtual bool IsPrimaryKey(string tableName, string columnName)
        {
            columnName = this.SqlBuilder.GetTranslationColumnName(columnName);
            var isAny = IsAnyTable(tableName);
            Check.Exception(!isAny, string.Format("Table {0} does not exist", tableName));
            var columns = GetColumnInfosByTableName(tableName);
            if (columns.IsNullOrEmpty()) return false;
            return columns.Any(it => it.IsPrimarykey = true && it.DbColumnName.Equals(columnName, StringComparison.CurrentCultureIgnoreCase));
        }
        public virtual bool IsIdentity(string tableName, string columnName)
        {
            columnName = this.SqlBuilder.GetTranslationColumnName(columnName);
            var isAny = IsAnyTable(tableName);
            Check.Exception(!isAny, string.Format("Table {0} does not exist", tableName));
            var columns = GetColumnInfosByTableName(tableName);
            if (columns.IsNullOrEmpty()) return false;
            return columns.Any(it => it.IsIdentity = true && it.DbColumnName.Equals(columnName, StringComparison.CurrentCultureIgnoreCase));
        }
        public virtual bool IsAnyConstraint(string constraintName)
        {
            return this.Context.Ado.GetInt("select  object_id('" + constraintName + "')") > 0;
        }
        public virtual bool IsAnySystemTablePermissions()
        {
            this.Context.Ado.CheckConnection();
            string sql = this.CheckSystemTablePermissionsSql;
            try
            {
                var oldIsEnableLog = this.Context.Ado.IsEnableLogEvent;
                this.Context.Ado.IsEnableLogEvent = false;
                this.Context.Ado.ExecuteCommand(sql);
                this.Context.Ado.IsEnableLogEvent = oldIsEnableLog;
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region DDL
        public virtual bool AddPrimaryKey(string tableName, string columnName)
        {
            tableName = this.SqlBuilder.GetTranslationTableName(tableName);
            columnName = this.SqlBuilder.GetTranslationTableName(columnName);
            string sql = string.Format(this.AddPrimaryKeySql, tableName, string.Format("PK_{0}_{1}", this.SqlBuilder.GetNoTranslationColumnName(tableName), this.SqlBuilder.GetNoTranslationColumnName(columnName)), columnName);
            this.Context.Ado.ExecuteCommand(sql);
            return true;
        }
        public virtual bool AddColumn(string tableName, DbColumnInfo columnInfo)
        {
            tableName = this.SqlBuilder.GetTranslationTableName(tableName);
            string sql = GetAddColumnSql(tableName, columnInfo);
            this.Context.Ado.ExecuteCommand(sql);
            return true;
        }
        public virtual bool UpdateColumn(string tableName, DbColumnInfo column)
        {
            tableName = this.SqlBuilder.GetTranslationTableName(tableName);
            string sql = GetUpdateColumnSql(tableName, column);
            this.Context.Ado.ExecuteCommand(sql);
            return true;
        }
        public virtual bool CreateTable(string tableName, List<DbColumnInfo> columns)
        {
            tableName = this.SqlBuilder.GetTranslationTableName(tableName);
            string sql = GetCreateTableSql(tableName, columns);
            this.Context.Ado.ExecuteCommand(sql);
            return true;
        }
        public virtual bool DropTable(string tableName)
        {
            tableName = this.SqlBuilder.GetTranslationTableName(tableName);
            this.Context.Ado.ExecuteCommand(string.Format(this.DropTableSql, tableName));
            return true;
        }
        public virtual bool DropColumn(string tableName, string columnName)
        {
            columnName = this.SqlBuilder.GetTranslationColumnName(columnName);
            tableName = this.SqlBuilder.GetTranslationTableName(tableName);
            this.Context.Ado.ExecuteCommand(string.Format(this.DropColumnToTableSql, tableName, columnName));
            return true;
        }
        public virtual bool DropConstraint(string tableName, string constraintName)
        {
            tableName = this.SqlBuilder.GetTranslationTableName(tableName);
            string sql = string.Format(this.DropConstraintSql, tableName, constraintName);
            this.Context.Ado.ExecuteCommand(sql);
            return true;
        }
        public virtual bool TruncateTable(string tableName)
        {
            tableName = this.SqlBuilder.GetTranslationTableName(tableName);
            this.Context.Ado.ExecuteCommand(string.Format(this.TruncateTableSql, tableName));
            return true;
        }
        public virtual bool BackupDataBase(string databaseName, string fullFileName)
        {
            var directory = FileHelper.GetDirectoryFromFilePath(fullFileName);
            if (!FileHelper.IsExistDirectory(directory))
            {
                FileHelper.CreateDirectory(directory);
            }
            this.Context.Ado.ExecuteCommand(string.Format(this.BackupDataBaseSql, databaseName, fullFileName));
            return true;
        }
        public virtual bool BackupTable(string oldTableName, string newTableName, int maxBackupDataRows = int.MaxValue)
        {
            oldTableName = this.SqlBuilder.GetTranslationTableName(oldTableName);
            newTableName = this.SqlBuilder.GetTranslationTableName(newTableName);
            string sql = string.Format(this.BackupTableSql, newTableName, oldTableName, maxBackupDataRows);
            this.Context.Ado.ExecuteCommand(sql);
            return true;
        }
        public virtual bool RenameColumn(string tableName, string oldColumnName, string newColumnName)
        {
            tableName = this.SqlBuilder.GetTranslationTableName(tableName);
            oldColumnName = this.SqlBuilder.GetTranslationColumnName(oldColumnName);
            newColumnName = this.SqlBuilder.GetTranslationColumnName(newColumnName);
            string sql = string.Format(this.RenameColumnSql, tableName, oldColumnName, newColumnName);
            this.Context.Ado.ExecuteCommand(sql);
            return true;
        }
        #endregion

        #region Private
        private List<T> GetListOrCache<T>(string cacheKey, string sql)
        {
            return this.Context.RewritableMethods.GetCacheInstance<List<T>>().Func(cacheKey,
             (cm, key) =>
             {
                 return cm[cacheKey];

             }, (cm, key) =>
             {
                 var isEnableLogEvent = this.Context.Ado.IsEnableLogEvent;
                 this.Context.Ado.IsEnableLogEvent = false;
                 var reval = this.Context.Ado.SqlQuery<T>(sql);
                 this.Context.Ado.IsEnableLogEvent = isEnableLogEvent;
                 return reval;
             });
        }
        protected virtual string GetCreateTableSql(string tableName, List<DbColumnInfo> columns)
        {
            List<string> columnArray = new List<string>();
            Check.Exception(columns.IsNullOrEmpty(), "No columns found ");
            foreach (var item in columns)
            {
                string columnName = this.SqlBuilder.GetTranslationTableName(item.DbColumnName);
                string dataType = item.DataType;
                string dataSize = item.Length > 0 ? string.Format("({0})", item.Length) : null;
                string nullType = item.IsNullable ? this.CreateTableNull : CreateTableNotNull;
                string primaryKey = null;
                string identity = item.IsIdentity ? this.CreateTableIdentity : null;
                string addItem = string.Format(this.CreateTableColumn, columnName, dataType, dataSize, nullType, primaryKey, identity);
                columnArray.Add(addItem);
            }
            string tableString = string.Format(this.CreateTableSql, this.SqlBuilder.GetTranslationTableName(tableName), string.Join(",\r\n", columnArray));
            return tableString;
        }
        protected virtual string GetAddColumnSql(string tableName, DbColumnInfo columnInfo)
        {
            string columnName = this.SqlBuilder.GetTranslationColumnName(columnInfo.DbColumnName);
            tableName = this.SqlBuilder.GetTranslationTableName(tableName);
            string dataType = columnInfo.DataType;
            string dataSize = columnInfo.Length > 0 ? string.Format("({0})", columnInfo.Length) : null;
            string nullType = columnInfo.IsNullable ? this.CreateTableNull : CreateTableNotNull;
            string primaryKey = null;
            string identity = null;
            string result = string.Format(this.AddColumnToTableSql, tableName, columnName, dataType, dataSize, nullType, primaryKey, identity);
            return result;
        }
        protected virtual string GetUpdateColumnSql(string tableName, DbColumnInfo columnInfo)
        {
            string columnName = this.SqlBuilder.GetTranslationTableName(columnInfo.DbColumnName);
            tableName = this.SqlBuilder.GetTranslationTableName(tableName);
            string dataType = columnInfo.DataType;
            string dataSize = columnInfo.Length > 0 ? string.Format("({0})", columnInfo.Length) : null;
            string nullType = columnInfo.IsNullable ? this.CreateTableNull : CreateTableNotNull;
            string primaryKey = null;
            string identity = null;
            string result = string.Format(this.AlterColumnToTableSql, tableName, columnName, dataType, dataSize, nullType, primaryKey, identity);
            return result;
        }
        protected virtual string GetCacheKey(string cacheKey)
        {
            return this.Context.CurrentConnectionConfig.DbType + "." + this.Context.Ado.Connection.Database +"."+ cacheKey;
        }
        #endregion
    }
}
