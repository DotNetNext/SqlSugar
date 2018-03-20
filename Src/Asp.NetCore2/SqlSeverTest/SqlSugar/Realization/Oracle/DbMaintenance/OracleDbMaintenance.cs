using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class OracleDbMaintenance : DbMaintenanceProvider
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
                return @"SELECT  table_name name from user_tables where
                        table_name!='HELP' 
                        AND table_name NOT LIKE '%$%'
                        AND table_name NOT LIKE 'LOGMNRC_%'
                        AND table_name!='LOGMNRP_CTAS_PART_MAP'
                        AND table_name!='LOGMNR_LOGMNR_BUILDLOG'
                        AND table_name!='SQLPLUS_PRODUCT_PROFILE'  
                         ";
            }
        }
        protected override string GetViewInfoListSql
        {
            get
            {
                return @"select view_name name  from user_views 
                                                WHERE VIEW_name NOT LIKE '%$%'
                                                AND VIEW_NAME !='PRODUCT_PRIVS'
                        AND VIEW_NAME NOT LIKE 'MVIEW_%' ";
            }
        }
        #endregion

        #region DDL
        protected override string AddPrimaryKeySql
        {
            get
            {
                return "ALTER TABLE {0} ADD CONSTRAINT {1} PRIMARY KEY({2})";
            }
        }
        protected override string AddColumnToTableSql
        {
            get
            {
                return "ALTER TABLE {0} ADD {1} {2}{3} {4} {5} {6}";
            }
        }
        protected override string AlterColumnToTableSql
        {
            get
            {
                return "ALTER TABLE {0} ALTER COLUMN {1} {2}{3} {4} {5} {6}";
            }
        }
        protected override string BackupDataBaseSql
        {
            get
            {
                return @"USE master;BACKUP DATABASE {0} TO disk = '{1}'";
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
                return "SELECT TOP {0} *　INTO {1} FROM  {2}";
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
                return "ALTER TABLE {0} DROP CONSTRAINT  {1}";
            }
        }
        protected override string RenameColumnSql
        {
            get
            {
                return "exec sp_rename '{0}.{1}','{2}','column';";
            }
        }
        #endregion

        #region Check
        protected override string CheckSystemTablePermissionsSql
        {
            get
            {
                return "select  t.table_name from user_tables t  where rownum=1";
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
                return "IDENTITY(1,1)";
            }
        }
        #endregion

        #region Methods
        public override List<DbColumnInfo> GetColumnInfosByTableName(string tableName,bool isCache=true)
        {
            string cacheKey = "DbMaintenanceProvider.GetColumnInfosByTableName." + this.SqlBuilder.GetNoTranslationColumnName(tableName).ToLower();
            cacheKey = GetCacheKey(cacheKey);
            if (!isCache)
                return GetColumnInfosByTableName(tableName);
            else
            return this.Context.Utilities.GetReflectionInoCacheInstance().GetOrCreate(cacheKey,
                    () =>
                    {
                        return GetColumnInfosByTableName(tableName);

                    });
        }

        private List<DbColumnInfo> GetColumnInfosByTableName(string tableName)
        {
            string sql = "select * from " + tableName + " WHERE 1=2 ";
            var oldIsEnableLog = this.Context.Ado.IsEnableLogEvent;
            this.Context.Ado.IsEnableLogEvent = false;
            using (DbDataReader reader = (DbDataReader)this.Context.Ado.GetDataReader(sql))
            {
                this.Context.Ado.IsEnableLogEvent = oldIsEnableLog;
                List<DbColumnInfo> result = new List<DbColumnInfo>();
                var schemaTable = reader.GetSchemaTable();
                foreach (System.Data.DataRow row in schemaTable.Rows)
                {
                    DbColumnInfo column = new DbColumnInfo()
                    {
                        TableName = tableName,
                        DataType = row["DataType"].ToString().Replace("System.", "").Trim(),
                        IsNullable = (bool)row["AllowDBNull"],
                        //IsIdentity = (bool)row["IsAutoIncrement"],
                        ColumnDescription = GetFieldComment(tableName, row["ColumnName"].ToString()),
                        DbColumnName = row["ColumnName"].ToString(),
                        //DefaultValue = row["defaultValue"].ToString(),
                        IsPrimarykey = GetPrimaryKeyByTableNames(tableName).Any(it => it.Equals(row["ColumnName"].ToString(), StringComparison.CurrentCultureIgnoreCase)),
                        Length = row["ColumnSize"].ObjToInt(),
                        Scale = row["numericscale"].ObjToInt()
                    };
                    result.Add(column);
                }
                return result;
            }
        }

        private List<string> GetPrimaryKeyByTableNames(string tableName)
        {
            string cacheKey = "DbMaintenanceProvider.GetPrimaryKeyByTableNames." + this.SqlBuilder.GetNoTranslationColumnName(tableName).ToLower();
            cacheKey = GetCacheKey(cacheKey);
            return this.Context.Utilities.GetReflectionInoCacheInstance().GetOrCreate(cacheKey,
                    () =>
                    {
                        var oldIsEnableLog = this.Context.Ado.IsEnableLogEvent;
                        this.Context.Ado.IsEnableLogEvent = false;
                        string sql = @" select distinct cu.COLUMN_name KEYNAME  from user_cons_columns cu, user_constraints au 
                            where cu.constraint_name = au.constraint_name
                            and au.constraint_type = 'P' and au.table_name = '" + tableName.ToUpper() + @"'";
                        var pks = this.Context.Ado.SqlQuery<string>(sql);
                        this.Context.Ado.IsEnableLogEvent = oldIsEnableLog;
                        return pks;
                    });
        }

        public string GetTableComment(string tableName)
        {
            string cacheKey = "DbMaintenanceProvider.GetTableComment." + tableName;
            var comments = this.Context.Utilities.GetReflectionInoCacheInstance().GetOrCreate(cacheKey,
                          () =>
                          {
                              string sql = "SELECT COMMENTS FROM USER_TAB_COMMENTS WHERE TABLE_NAME =@tableName ORDER BY TABLE_NAME";
                              var oldIsEnableLog = this.Context.Ado.IsEnableLogEvent;
                              this.Context.Ado.IsEnableLogEvent = false;
                              var pks = this.Context.Ado.SqlQuery<string>(sql,new { tableName=tableName.ToUpper() });
                              this.Context.Ado.IsEnableLogEvent = oldIsEnableLog;
                              return pks;
                          });
            return comments.HasValue() ? comments.First() : "";
        }

        public string GetFieldComment(string tableName, string filedName)
        {
            string cacheKey = "DbMaintenanceProvider.GetFieldComment." + tableName;
            var comments = this.Context.Utilities.GetReflectionInoCacheInstance().GetOrCreate(cacheKey,
                           () =>
                           {
                               string sql = "SELECT TABLE_NAME AS TableName, COLUMN_NAME AS DbColumnName,COMMENTS AS ColumnDescription  FROM user_col_comments   WHERE TABLE_NAME =@tableName ORDER BY TABLE_NAME";
                               var oldIsEnableLog = this.Context.Ado.IsEnableLogEvent;
                               this.Context.Ado.IsEnableLogEvent = false;
                               var pks = this.Context.Ado.SqlQuery<DbColumnInfo>(sql, new { tableName = tableName.ToUpper() });
                               this.Context.Ado.IsEnableLogEvent = oldIsEnableLog;
                               return pks;
                           });
            return comments.HasValue() ? comments.First(it=>it.DbColumnName.Equals(filedName,StringComparison.CurrentCultureIgnoreCase)).ColumnDescription : "";

        }

        public override bool CreateTable(string tableName, List<DbColumnInfo> columns, bool isCreatePrimaryKey = true)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
