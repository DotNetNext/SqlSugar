using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public abstract partial class DbMaintenanceProvider : IDbMaintenance
    {
        #region DML
        public virtual List<string> GetDataBaseList(SqlSugarClient db)
        {
            return db.Ado.SqlQuery<string>(this.GetDataBaseSql);
        }
        public virtual List<DbTableInfo> GetViewInfoList(bool isCache = true)
        {
            string cacheKey = "DbMaintenanceProvider.GetViewInfoList";
            cacheKey = GetCacheKey(cacheKey);
            var result = new List<DbTableInfo>();
            if (isCache)
                result = GetListOrCache<DbTableInfo>(cacheKey, this.GetViewInfoListSql);
            else
                result = this.Context.Ado.SqlQuery<DbTableInfo>(this.GetViewInfoListSql);
            foreach (var item in result)
            {
                item.DbObjectType = DbObjectType.View;
            }
            return result;
        }
        public virtual List<DbTableInfo> GetTableInfoList(bool isCache = true)
        {
            string cacheKey = "DbMaintenanceProvider.GetTableInfoList";
            cacheKey = GetCacheKey(cacheKey);
            var result = new List<DbTableInfo>();
            if (isCache)
                result = GetListOrCache<DbTableInfo>(cacheKey, this.GetTableInfoListSql);
            else
                result = this.Context.Ado.SqlQuery<DbTableInfo>(this.GetTableInfoListSql);
            foreach (var item in result)
            {
                item.DbObjectType = DbObjectType.Table;
            }
            return result;
        }
        public virtual List<DbColumnInfo> GetColumnInfosByTableName(string tableName, bool isCache = true)
        {
            if (string.IsNullOrEmpty(tableName)) return new List<DbColumnInfo>();
            string cacheKey = "DbMaintenanceProvider.GetColumnInfosByTableName." + this.SqlBuilder.GetNoTranslationColumnName(tableName).ToLower();
            cacheKey = GetCacheKey(cacheKey);
            var sql = string.Format(this.GetColumnInfosByTableNameSql, tableName);
            if (isCache)
                return GetListOrCache<DbColumnInfo>(cacheKey, sql).GroupBy(it => it.DbColumnName).Select(it => it.First()).ToList();
            else
                return this.Context.Ado.SqlQuery<DbColumnInfo>(sql).GroupBy(it => it.DbColumnName).Select(it => it.First()).ToList();

        }
        public virtual List<string> GetIsIdentities(string tableName)
        {
            string cacheKey = "DbMaintenanceProvider.GetIsIdentities" + this.SqlBuilder.GetNoTranslationColumnName(tableName).ToLower();
            cacheKey = GetCacheKey(cacheKey);
            return this.Context.Utilities.GetReflectionInoCacheInstance().GetOrCreate(cacheKey, () =>
                     {
                         var result = GetColumnInfosByTableName(tableName).Where(it => it.IsIdentity).ToList();
                         return result.Select(it => it.DbColumnName).ToList();
                     });
        }
        public virtual List<string> GetPrimaries(string tableName)
        {
            string cacheKey = "DbMaintenanceProvider.GetPrimaries" + this.SqlBuilder.GetNoTranslationColumnName(tableName).ToLower();
            cacheKey = GetCacheKey(cacheKey);
            return this.Context.Utilities.GetReflectionInoCacheInstance().GetOrCreate(cacheKey, () =>
             {
                 var result = GetColumnInfosByTableName(tableName).Where(it => it.IsPrimarykey).ToList();
                 return result.Select(it => it.DbColumnName).ToList();
             });
        }
        #endregion

        #region Check
        public virtual bool IsAnyTable(string tableName, bool isCache = true)
        {
            Check.Exception(string.IsNullOrEmpty(tableName), "IsAnyTable tableName is not null");
            tableName = this.SqlBuilder.GetNoTranslationColumnName(tableName);
            var tables = GetTableInfoList(isCache);
            if (tables == null) return false;
            else return tables.Any(it => it.Name.Equals(tableName, StringComparison.CurrentCultureIgnoreCase));
        }
        public virtual bool IsAnyColumn(string tableName, string columnName)
        {
            columnName = this.SqlBuilder.GetNoTranslationColumnName(columnName);
            tableName = this.SqlBuilder.GetNoTranslationColumnName(tableName);
            var isAny = IsAnyTable(tableName);
            Check.Exception(!isAny, string.Format("Table {0} does not exist", tableName));
            var columns = GetColumnInfosByTableName(tableName);
            if (columns.IsNullOrEmpty()) return false;
            return columns.Any(it => it.DbColumnName.Equals(columnName, StringComparison.CurrentCultureIgnoreCase));
        }
        public virtual bool IsPrimaryKey(string tableName, string columnName)
        {
            columnName = this.SqlBuilder.GetNoTranslationColumnName(columnName);
            var isAny = IsAnyTable(tableName);
            Check.Exception(!isAny, string.Format("Table {0} does not exist", tableName));
            var columns = GetColumnInfosByTableName(tableName);
            if (columns.IsNullOrEmpty()) return false;
            var result=columns.Any(it => it.IsPrimarykey == true && it.DbColumnName.Equals(columnName, StringComparison.CurrentCultureIgnoreCase));
            return result;
        }
        public virtual bool IsIdentity(string tableName, string columnName)
        {
            columnName = this.SqlBuilder.GetNoTranslationColumnName(columnName);
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
        /// <summary>
        ///by current connection string
        /// </summary>
        /// <param name="databaseDirectory"></param>
        /// <returns></returns>
        public virtual bool CreateDatabase(string databaseDirectory = null)
        {
            var seChar = Path.DirectorySeparatorChar.ToString();
            if (databaseDirectory.HasValue())
            {
                databaseDirectory = databaseDirectory.TrimEnd('\\').TrimEnd('/');
            }
            var databaseName= this.Context.Ado.Connection.Database;
            return CreateDatabase(databaseName,databaseDirectory);
        }
        /// <summary>
        /// by databaseName
        /// </summary>
        /// <param name="databaseName"></param>
        /// <param name="databaseDirectory"></param>
        /// <returns></returns>
        public virtual bool CreateDatabase(string databaseName, string databaseDirectory = null)
        {
            this.Context.Ado.ExecuteCommand(string.Format(CreateDataBaseSql, databaseName, databaseDirectory));
            return true;
        }

        public virtual bool AddPrimaryKey(string tableName, string columnName)
        {
            tableName = this.SqlBuilder.GetTranslationTableName(tableName);
            columnName = this.SqlBuilder.GetTranslationTableName(columnName);
            string sql = string.Format(this.AddPrimaryKeySql, tableName, string.Format("PK_{0}_{1}", this.SqlBuilder.GetNoTranslationColumnName(tableName), this.SqlBuilder.GetNoTranslationColumnName(columnName)), columnName);
            this.Context.Ado.ExecuteCommand(sql);
            return true;
        }

        public bool AddPrimaryKeys(string tableName, string[] columnNames)
        {
            tableName = this.SqlBuilder.GetTranslationTableName(tableName);
            var columnName = string.Join(",", columnNames);
            var pkName = string.Format("PK_{0}_{1}", this.SqlBuilder.GetNoTranslationColumnName(tableName), columnName.Replace(",","_"));
            string sql = string.Format(this.AddPrimaryKeySql, tableName,pkName, columnName);
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
        public abstract bool CreateTable(string tableName, List<DbColumnInfo> columns, bool isCreatePrimaryKey = true);
        public virtual bool DropTable(string tableName)
        {
            tableName = this.SqlBuilder.GetTranslationTableName(tableName);
            this.Context.Ado.ExecuteCommand(string.Format(this.DropTableSql, tableName));
            return true;
        }

        public virtual bool TruncateTable<T>()
        {
            this.Context.InitMappingInfo<T>();
            return this.TruncateTable(this.Context.EntityMaintenance.GetEntityInfo<T>().DbTableName);
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
            string sql = string.Format(this.BackupTableSql, maxBackupDataRows, newTableName, oldTableName);
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
        public virtual bool AddColumnRemark(string columnName, string tableName, string description)
        {
            string sql = string.Format(this.AddColumnRemarkSql, columnName, tableName, description);
            this.Context.Ado.ExecuteCommand(sql);
            return true;
        }
        public virtual bool DeleteColumnRemark(string columnName, string tableName)
        {
            string sql = string.Format(this.DeleteColumnRemarkSql, columnName, tableName);
            this.Context.Ado.ExecuteCommand(sql);
            return true;
        }
        public virtual bool IsAnyColumnRemark(string columnName, string tableName)
        {
            string sql = string.Format(this.IsAnyColumnRemarkSql, columnName, tableName);
            var dt=this.Context.Ado.GetDataTable(sql);
            return dt.Rows!=null&&dt.Rows.Count>0;
        }
        public virtual bool AddTableRemark(string tableName, string description)
        {
            string sql = string.Format(this.AddTableRemarkSql,tableName, description);
            this.Context.Ado.ExecuteCommand(sql);
            return true;
        }
        public virtual bool DeleteTableRemark(string tableName)
        {
            string sql = string.Format(this.DeleteTableRemarkSql,tableName);
            this.Context.Ado.ExecuteCommand(sql);
            return true;
        }
        public virtual bool IsAnyTableRemark(string tableName)
        {
            string sql = string.Format(this.IsAnyTableRemarkSql, tableName);
            var dt=this.Context.Ado.GetDataTable(sql);
            return dt.Rows != null && dt.Rows.Count > 0;
        }
        public virtual bool AddDefaultValue(string tableName, string columnName, string defaultValue)
        {
            if (defaultValue == "''")
            {
                defaultValue = "";
            }
            string sql = string.Format(AddDefaultValueSql, tableName, columnName,defaultValue);
            this.Context.Ado.ExecuteCommand(sql);
            return true;
        }
        public virtual bool CreateIndex(string tableName, string[] columnNames, bool isUnique=false)
        {
            string sql = string.Format(CreateIndexSql,tableName,string.Join(",",columnNames), string.Join("_", columnNames) + this.Context.CurrentConnectionConfig.IndexSuffix, isUnique ? "UNIQUE" : "");
            this.Context.Ado.ExecuteCommand(sql);
            return true;
        }
        public virtual bool CreateUniqueIndex(string tableName, string[] columnNames)
        {
            string sql = string.Format(CreateIndexSql, tableName, string.Join(",", columnNames), string.Join("_", columnNames) + this.Context.CurrentConnectionConfig.IndexSuffix + "_Unique","UNIQUE" );
            this.Context.Ado.ExecuteCommand(sql);
            return true;
        }
        public virtual bool IsAnyIndex(string indexName)
        {
            string sql = string.Format(this.IsAnyIndexSql, indexName);
            return this.Context.Ado.GetInt(sql)>0;
        }
        public virtual bool AddRemark(EntityInfo entity)
        {
            var db = this.Context;
            var columns = entity.Columns.Where(it => it.IsIgnore == false).ToList();

            foreach (var item in columns)
            {
                if (item.ColumnDescription != null)
                {
                    //column remak
                    if (db.DbMaintenance.IsAnyColumnRemark(item.DbColumnName, item.DbTableName))
                    {
                        db.DbMaintenance.DeleteColumnRemark(item.DbColumnName, item.DbTableName);
                        db.DbMaintenance.AddColumnRemark(item.DbColumnName, item.DbTableName, item.ColumnDescription);
                    }
                    else
                    {
                        db.DbMaintenance.AddColumnRemark(item.DbColumnName, item.DbTableName, item.ColumnDescription);
                    }
                }
            }

            //table remak
            if (entity.TableDescription != null)
            {
                if (db.DbMaintenance.IsAnyTableRemark(entity.DbTableName))
                {
                    db.DbMaintenance.DeleteTableRemark(entity.DbTableName);
                    db.DbMaintenance.AddTableRemark(entity.DbTableName, entity.TableDescription);
                }
                else
                {
                    db.DbMaintenance.AddTableRemark(entity.DbTableName, entity.TableDescription);
                }
            }
            return true;
        }

        public virtual void AddIndex(EntityInfo entityInfo)
        {
            var db = this.Context;
            var columns = entityInfo.Columns.Where(it => it.IsIgnore == false).ToList();
            var indexColumns = columns.Where(it => it.IndexGroupNameList.HasValue()).ToList();
            if (indexColumns.HasValue())
            {
                var groups = indexColumns.SelectMany(it => it.IndexGroupNameList).GroupBy(it => it).Select(it=>it.Key).ToList();
                foreach (var item in groups)
                {
                    var columnNames = indexColumns.Where(it => it.IndexGroupNameList.Any(i => i.Equals(item, StringComparison.CurrentCultureIgnoreCase))).Select(it=>it.DbColumnName).ToArray();
                    var indexName = string.Format("Index_{0}_{1}"+this.Context.CurrentConnectionConfig.IndexSuffix,entityInfo.DbTableName, string.Join("_", columnNames));
                    if (!IsAnyIndex(indexName))
                    {
                        CreateIndex(entityInfo.DbTableName, columnNames);
                    }
                }
            }


            var uIndexColumns = columns.Where(it => it.UIndexGroupNameList.HasValue()).ToList();
            if (uIndexColumns.HasValue())
            {
                var groups = uIndexColumns.SelectMany(it => it.UIndexGroupNameList).GroupBy(it => it).Select(it => it.Key).ToList();
                foreach (var item in groups)
                {
                    var columnNames = uIndexColumns.Where(it => it.UIndexGroupNameList.Any(i => i.Equals(item, StringComparison.CurrentCultureIgnoreCase))).Select(it => it.DbColumnName).ToArray();
                    var indexName = string.Format("Index_{0}_{1}_Unique" + this.Context.CurrentConnectionConfig.IndexSuffix, entityInfo.DbTableName, string.Join("_", columnNames));
                    if (!IsAnyIndex(indexName))
                    {
                        CreateUniqueIndex(entityInfo.DbTableName, columnNames);
                    }
                }
            }
        }

        protected virtual bool IsAnyDefaultValue(string tableName, string columnName,List<DbColumnInfo> columns)
        {
            var defaultValue = columns.Where(it => it.DbColumnName.Equals(columnName, StringComparison.CurrentCultureIgnoreCase)).First().DefaultValue;
            return defaultValue.HasValue();
        }

        public virtual bool IsAnyDefaultValue(string tableName, string columnName)
        {
            return IsAnyDefaultValue(tableName, columnName, this.GetColumnInfosByTableName(tableName, false));
        }

        public virtual void AddDefaultValue(EntityInfo entityInfo)
        {
            var dbColumns=this.GetColumnInfosByTableName(entityInfo.DbTableName, false);
            var db = this.Context;
            var columns = entityInfo.Columns.Where(it => it.IsIgnore == false).ToList();
            foreach (var item in columns)
            {
                if (item.DefaultValue.HasValue())
                {
                    if (!IsAnyDefaultValue(entityInfo.DbTableName,item.DbColumnName,dbColumns))
                    {
                        this.AddDefaultValue(entityInfo.DbTableName, item.DbColumnName, item.DefaultValue);
                    }
                }
            }
        }

        public virtual bool RenameTable(string oldTableName, string newTableName)
        {
            string sql = string.Format(this.RenameTableSql, oldTableName,newTableName);
            this.Context.Ado.ExecuteCommand(sql);
            return true;
        }
        #endregion

        #region Private
        private List<T> GetListOrCache<T>(string cacheKey, string sql)
        {
            return this.Context.Utilities.GetReflectionInoCacheInstance().GetOrCreate(cacheKey,
             () =>
             {
                 var isEnableLogEvent = this.Context.Ado.IsEnableLogEvent;
                 this.Context.Ado.IsEnableLogEvent = false;
                 var result = this.Context.Ado.SqlQuery<T>(sql);
                 this.Context.Ado.IsEnableLogEvent = isEnableLogEvent;
                 return result;
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
                string dataSize = GetSize(item);
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
            string dataSize = GetSize(columnInfo);
            string nullType = columnInfo.IsNullable ? this.CreateTableNull : CreateTableNotNull;
            string primaryKey = null;
            string identity = null;
            string result = string.Format(this.AddColumnToTableSql, tableName, columnName, dataType, dataSize, nullType, primaryKey, identity);
            return result;
        }
        protected virtual string GetUpdateColumnSql(string tableName, DbColumnInfo columnInfo)
        {
            string columnName = this.SqlBuilder.GetTranslationColumnName(columnInfo.DbColumnName);
            tableName = this.SqlBuilder.GetTranslationTableName(tableName);
            string dataSize = GetSize(columnInfo);
            string dataType = columnInfo.DataType;
            string nullType = columnInfo.IsNullable ? this.CreateTableNull : CreateTableNotNull;
            string primaryKey = null;
            string identity = null;
            string result = string.Format(this.AlterColumnToTableSql, tableName, columnName, dataType, dataSize, nullType, primaryKey, identity);
            return result;
        }
        protected virtual string GetCacheKey(string cacheKey)
        {
            return this.Context.CurrentConnectionConfig.DbType + "." + this.Context.Ado.Connection.Database + "." + cacheKey;
        }
        protected virtual string GetSize(DbColumnInfo item)
        {
            string dataSize = null;
            var isMax = item.Length > 4000 || item.Length == -1;
            if (isMax)
            {
                dataSize = item.Length > 0 ? string.Format("({0})", "max") : null;
            }
            else if (item.Length == 0 && item.DecimalDigits > 0)
            {
                item.Length = 10;
                dataSize = string.Format("({0},{1})", item.Length, item.DecimalDigits);
            }
            else if (item.Length > 0 && item.DecimalDigits == 0)
            {
                dataSize = item.Length > 0 ? string.Format("({0})", item.Length) : null;
            }
            else if (item.Length > 0 && item.DecimalDigits > 0)
            {
                dataSize = item.Length > 0 ? string.Format("({0},{1})", item.Length, item.DecimalDigits) : null;
            }
            return dataSize;
        }
        #endregion
    }
}
