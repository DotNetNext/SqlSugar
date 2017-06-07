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
            return true;
        }
        public bool IsAnyColumn(string tableName,string column)
        {
            return true;
        }
        public bool IsPrimaryKey(string tableName, string column)
        {
            return true;
        }
        public bool IsIdentity(string tableName, string column)
        {
            return true;
        }
        #endregion

        #region DDL
        public bool AddColumnToTable(string tableName, DbColumnInfo column)
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
            this.Context.Ado.ExecuteCommand(string.Format(this.TruncateTableSql, tableName));
            return true;
        }

        public bool BackupDataBase()
        {
            throw new NotImplementedException();
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
