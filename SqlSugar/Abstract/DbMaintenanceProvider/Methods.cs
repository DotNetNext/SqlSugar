using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public abstract partial class DbMaintenanceProvider : IDbMaintenance
    {
        #region DML
        public List<DbTableInfo> GetViewInfoList()
        {
            string key = "DbMaintenanceProvider.GetViewInfoList";
            var result = GetListOrCache<DbTableInfo>(key, this.GetViewInfoListSql);
            foreach (var item in result)
            {
                item.DbObjectType = DbObjectType.View;
            }
            return result;
        }

        public List<DbTableInfo> GetTableInfoList()
        {
            string key = "DbMaintenanceProvider.GetTableInfoList";
            var result = GetListOrCache<DbTableInfo>(key, this.GetTableInfoListSql);
            foreach (var item in result)
            {
                item.DbObjectType = DbObjectType.Table;
            }
            return result;
        }

        public virtual List<DbColumnInfo> GetColumnInfosByTableName(string tableName)
        {
            if (string.IsNullOrEmpty(tableName)) return new List<DbColumnInfo>();
            string key = "DbMaintenanceProvider.GetColumnInfosByTableName." + tableName.ToLower();
            return GetListOrCache<DbColumnInfo>(key, string.Format(this.GetColumnInfosByTableNameSql, tableName));
        }

        public virtual List<string> GetIsIdentities(string tableName)
        {
            var result = GetColumnInfosByTableName(tableName).Where(it => it.IsIdentity).ToList();
            return result.Select(it => it.DbColumnName).ToList();
        }

        public virtual List<string> GetPrimaries(string tableName)
        {
            var result = GetColumnInfosByTableName(tableName).Where(it => it.IsPrimarykey).ToList();
            return result.Select(it => it.DbColumnName).ToList();
        }
        #endregion

        #region Check
        public bool IsAnyTable(string tableName)
        {
            var tables = GetTableInfoList();
            if (tables == null) return false;
            else return tables.Any(it => it.Name.Equals(tableName, StringComparison.CurrentCultureIgnoreCase));
        }
        public bool IsAnyColumn(string tableName, string columnName)
        {
            var isAny = IsAnyTable(tableName);
            Check.Exception(!isAny, string.Format("Table {0} does not exist", tableName));
            var columns = GetColumnInfosByTableName(tableName);
            if (columns.IsNullOrEmpty()) return false;
            return columns.Any(it => it.DbColumnName.Equals(columnName, StringComparison.CurrentCultureIgnoreCase));
        }
        public bool IsPrimaryKey(string tableName, string columnName)
        {
            var isAny = IsAnyTable(tableName);
            Check.Exception(!isAny, string.Format("Table {0} does not exist", tableName));
            var columns = GetColumnInfosByTableName(tableName);
            if (columns.IsNullOrEmpty()) return false;
            return columns.Any(it => it.IsPrimarykey = true && it.DbColumnName.Equals(columnName, StringComparison.CurrentCultureIgnoreCase));
        }
        public bool IsIdentity(string tableName, string columnName)
        {
            var isAny = IsAnyTable(tableName);
            Check.Exception(!isAny, string.Format("Table {0} does not exist", tableName));
            var columns = GetColumnInfosByTableName(tableName);
            if (columns.IsNullOrEmpty()) return false;
            return columns.Any(it => it.IsIdentity = true && it.DbColumnName.Equals(columnName, StringComparison.CurrentCultureIgnoreCase));
        }
        #endregion

        #region Get Sql
        public virtual string GetCreateTableSql(string tableName, List<DbColumnInfo> columns)
        {
            List<string> columnArray = new List<string>();
            Check.Exception(columns.IsNullOrEmpty(), "No columns found ");
            foreach (var item in columns)
            {
                string columnName = item.DbColumnName;
                string dataType = item.DataType;
                string dataSize = item.Length > 0 ? string.Format("({0})", item.Length) : null;
                string nullType = item.IsNullable ? this.CreateTableNull : CreateTableNotNull;
                string primaryKey = item.IsPrimarykey ? this.CreateTablePirmaryKey : null;
                string identity = item.IsIdentity ? this.CreateTableIdentity : null;
                string addItem = string.Format(this.CreateTableColumn, columnName, dataType, dataSize, nullType, primaryKey, identity);
                columnArray.Add(addItem);
            }
            string tableString = string.Format(this.CreateTableSql, tableName, string.Join(",\r\n", columnArray));
            return tableString;
        }
        public virtual string GetAddColumnSql(string tableName, DbColumnInfo columnInfo)
        {
            string columnName = columnInfo.DbColumnName;
            string dataType = columnInfo.DataType;
            string dataSize = columnInfo.Length > 0 ? string.Format("({0})", columnInfo.Length) : null;
            string nullType = columnInfo.IsNullable ? this.CreateTableNull : CreateTableNotNull;
            string primaryKey = columnInfo.IsPrimarykey ? this.CreateTablePirmaryKey : null;
            string identity = columnInfo.IsIdentity ? this.CreateTableIdentity : null;
            string result = string.Format(this.AddColumnToTableSql, tableName, columnName, dataType, dataSize, nullType, primaryKey, identity);
            return result;
        }
        #endregion

        #region DDL
        public bool AddColumnToTable(string tableName, DbColumnInfo columnName)
        {
            string sql = GetAddColumnSql(tableName, columnName);
            this.Context.Ado.ExecuteCommand(sql);
            return true;
        }

        public virtual bool CreateTable(string tableName, List<DbColumnInfo> columns)
        {
            string sql = GetCreateTableSql(tableName, columns);
            this.Context.Ado.ExecuteCommand(sql);
            return true;
        }
        public bool DropTable(string tableName)
        {
            this.Context.Ado.ExecuteCommand(string.Format(this.DropTableSql, tableName));
            return true;
        }
        public virtual bool TruncateTable(string tableName)
        {
            this.Context.Ado.ExecuteCommand(string.Format(this.TruncateTableSql, tableName));
            return true;
        }

        public bool BackupDataBase(string databaseName, string fullFileName)
        {
            var directory = FileHelper.GetDirectoryFromFilePath(fullFileName);
            if (!FileHelper.IsExistDirectory(directory))
            {
                FileHelper.CreateDirectory(directory);
            }
            this.Context.Ado.ExecuteCommand(string.Format(this.BackupDataBaseSql, databaseName, fullFileName));
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
        #endregion
    }
}
