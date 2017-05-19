using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
namespace SqlSugar
{
    public abstract partial class DbMaintenanceProvider : IDbMaintenance
    {
        protected abstract string GetViewInfoListSql { get; }
        protected abstract string GetTableInfoListSql { get; }
        protected abstract string GetColumnInfosByTableNameSql { get; }
        protected abstract string AddColumnToTableSql { get; }
        protected abstract string BackupDataBaseSql { get; }
        protected abstract string CreateTableSql { get; }
        protected abstract string TruncateTableSql { get; }

        public SqlSugarClient Context { get; set; }
        public List<DbTableInfo> GetViewInfoList()
        {
            string key = "DbMaintenanceProvider.GetViewInfoList";
            return GetListOrCache<DbTableInfo>(key, this.GetViewInfoListSql);
        }
        public List<DbTableInfo> GetTableInfoList()
        {
            string key = "DbMaintenanceProvider.GetTableInfoList";
            return GetListOrCache<DbTableInfo>(key, this.GetTableInfoListSql);
        }
        public virtual List<DbColumnInfo> GetColumnInfosByTableName(string tableName)
        {
            string key = "DbMaintenanceProvider.GetColumnInfosByTableName." + tableName.ToLower();
            return GetListOrCache<DbColumnInfo>(key, this.GetColumnInfosByTableNameSql);
        }

        public virtual List<string> GetIsIdentities(string tableName)
        {
            var result = GetColumnInfosByTableName(tableName).Where(it => it.IsIdentity).ToList();
            return result.Select(it => it.ColumnName).ToList();
        }

        public virtual List<string> GetPrimaries(string tableName)
        {
            var result = GetColumnInfosByTableName(tableName).Where(it => it.IsPrimarykey).ToList();
            return result.Select(it => it.ColumnName).ToList();
        }

        public bool AddColumnToTable(string tableName, DbColumnInfo column)
        {
            throw new NotImplementedException();
        }
        public bool BackupDataBase()
        {
            throw new NotImplementedException();
        }

        public virtual bool CreateTable(string tableName, List<DbColumnInfo> columns)
        {
            this.Context.Ado.ExecuteCommand(this.CreateTableSql);
            return true;
        }

        public virtual bool TruncateTable(string tableName)
        {
            this.Context.Ado.ExecuteCommand(this.TruncateTableSql);
            return true;
        }

        #region Private
        private List<T> GetListOrCache<T>(string cacheKey, string sql)
        {
            return CacheFactory.Func<List<T>>(cacheKey,
             (cm, key) =>
             {
                 return cm[cacheKey];

             }, (cm, key) =>
             {
                 var isEnableLogEvent = this.Context.Ado.IsEnableLogEvent;
                 this.Context.Ado.IsEnableLogEvent = false;
                 var reval = this.Context.Ado.SqlQuery<T>(this.GetColumnInfosByTableNameSql);
                 this.Context.Ado.IsEnableLogEvent = isEnableLogEvent;
                 return reval;
             });
        }
        private bool IsSystemTables()
        {
            var isSystemTables = Context.CurrentConnectionConfig is SystemTablesConfig;
            return isSystemTables;
        }
        #endregion
    }
}
