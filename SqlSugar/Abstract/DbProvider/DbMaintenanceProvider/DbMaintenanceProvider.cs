using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
namespace SqlSugar
{
    public abstract partial class DbMaintenanceProvider : IDbMaintenance
    {
        public SqlSugarClient Context { get; set; }

        protected abstract string GetViewInfoListSql { get; }
        public List<DbTableInfo> GetViewInfoList()
        {
            if (this.IsSystemTables())
            {
                string key = "DbMaintenanceProvider.GetViewInfoList";
                return GetListOrCache<DbTableInfo>(key, this.GetViewInfoListSql);
            }
            else
            {
                return new List<DbTableInfo>();
            }
        }

        protected abstract string GetTableInfoListSql { get; }
        public List<DbTableInfo> GetTableInfoList()
        {
            if (this.IsSystemTables())
            {
                string key = "DbMaintenanceProvider.GetTableInfoList";
                return GetListOrCache<DbTableInfo>(key, this.GetTableInfoListSql);
            }
            else
            {
                return CacheFactory.Func<List<DbTableInfo>>("DbMaintenanceProvider.GetTableInfoList2",
                    (cm, key) =>
                    {
                        return cm[key];
                    },
                    (cm, key) =>
                    {
                        List<DbTableInfo> reval = new List<DbTableInfo>();
                        var classes = Assembly.Load(this.Context.EntityNamespace.Split('.').First()).GetTypes();
                        foreach (var item in classes)
                        {
                            if (item.Namespace == this.Context.EntityNamespace)
                            {
                                var sugarTableObj = item.GetCustomAttributes(typeof(SugarTable), true).Where(it => it is SugarTable).SingleOrDefault();
                                if (sugarTableObj.IsNullOrEmpty())
                                {
                                    reval.Add(new DbTableInfo() { Name = item.Name });
                                }
                                else
                                {
                                    var sugarTable = (SugarTable)sugarTableObj;
                                    reval.Add(new DbTableInfo() { Name = sugarTable.TableName });
                                }
                            }
                        }
                        return reval;
                    });
            }
        }

        protected abstract string GetColumnInfosByTableNameSql { get; }
        public virtual List<DbColumnInfo> GetColumnInfosByTableName(string tableName)
        {
            if (this.IsSystemTables())
            {
                string key = "DbMaintenanceProvider.GetColumnInfosByTableName." + tableName.ToLower();
                return GetListOrCache<DbColumnInfo>(key, this.GetColumnInfosByTableNameSql);
            }
            else
            {
                string cacheKey = "DbMaintenanceProvider.GetColumnInfosByTableName." + tableName.ToLower() + "2";
                return CacheFactory.Func<List<DbColumnInfo>>(cacheKey,
                     (cm, key) =>
                     {
                         return cm[key];
                     },
                     (cm, key) =>
                     {
                         List<DbColumnInfo> reval = new List<DbColumnInfo>();
                         if (this.Context.MappingTables.IsNullOrEmpty())
                         {
                             this.GetTableInfoList();
                         }
                         var entities = this.Context.MappingTables.Where(it => it.DbTableName.Equals(tableName, StringComparison.CurrentCultureIgnoreCase)).ToList();
                         foreach (var entity in entities)
                         {
                             var entityName = entity == null ? tableName : entity.EntityName;
                             var assembly = Assembly.Load(this.Context.EntityNamespace.Split('.').First());
                             foreach (var item in assembly.GetType(this.Context.EntityNamespace + "." + entityName).GetProperties())
                             {
                                 var isVirtual = item.GetGetMethod().IsVirtual;
                                 if (isVirtual) continue;
                                 var sugarColumn = item.GetCustomAttributes(typeof(SugarColumn), true)
                                 .Where(it => it is SugarColumn)
                                 .Select(it => (SugarColumn)it)
                                 .Where(it => it.ColumnName.IsValuable())
                                 .FirstOrDefault();
                                 if (sugarColumn.IsNullOrEmpty())
                                 {
                                     reval.Add(new DbColumnInfo() { ColumnName = item.Name });
                                 }
                                 else
                                 {
                                     if (sugarColumn.IsIgnore == false)
                                     {
                                         var columnInfo = new DbColumnInfo();
                                         columnInfo.ColumnName = sugarColumn.ColumnName.IsNullOrEmpty() ? item.Name : sugarColumn.ColumnName;
                                         columnInfo.IsPrimarykey = sugarColumn.IsPrimaryKey;
                                         columnInfo.IsIdentity = sugarColumn.IsIdentity;
                                         columnInfo.ColumnDescription = sugarColumn.ColumnDescription;
                                         columnInfo.TableName = entity.IsNullOrEmpty() ? tableName : entity.DbTableName;
                                         reval.Add(columnInfo);
                                     }
                                 }
                             }
                         }
                         return reval;
                     });
            }
        }

        public virtual List<string> GetIsIdentities(string tableName)
        {
            var result = GetColumnInfosByTableName(tableName).Where(it => it.IsIdentity).ToList();
            return result.Select(it=>it.ColumnName).ToList();
        }

        public virtual List<string> GetPrimaries(string tableName)
        {
            var result = GetColumnInfosByTableName(tableName).Where(it => it.IsPrimarykey).ToList();
            return result.Select(it => it.ColumnName).ToList();
        }

        protected abstract string AddColumnToTableSql { get; }
        public bool AddColumnToTable(string tableName, DbColumnInfo column)
        {
            throw new NotImplementedException();
        }

        protected abstract string BackupDataBaseSql { get; }
        public bool BackupDataBase()
        {
            throw new NotImplementedException();
        }

        protected abstract string CreateTableSql { get; }
        public virtual bool CreateTable(string tableName, List<DbColumnInfo> columns)
        {
            this.Context.Database.ExecuteCommand(this.CreateTableSql);
            return true;
        }

        protected abstract string TruncateTableSql { get; }
        public virtual bool TruncateTable(string tableName)
        {
            this.Context.Database.ExecuteCommand(this.TruncateTableSql);
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
                 var isEnableLogEvent = this.Context.Database.IsEnableLogEvent;
                 this.Context.Database.IsEnableLogEvent = false;
                 var reval = this.Context.SqlQuery<T>(this.GetColumnInfosByTableNameSql);
                 this.Context.Database.IsEnableLogEvent = isEnableLogEvent;
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
