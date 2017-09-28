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
                return null;
            }
        }
        protected override string GetTableInfoListSql
        {
            get
            {
                return @"SELECT s.Name,Convert(varchar(max),tbp.value) as Description
                            FROM sysobjects s
					     	LEFT JOIN sys.extended_properties as tbp ON s.id=tbp.major_id and tbp.minor_id=0  WHERE s.xtype IN('U') AND (tbp.Name='MS_Description' OR tbp.Name is null)";
            }
        }
        protected override string GetViewInfoListSql
        {
            get
            {
                return @"SELECT s.Name,Convert(varchar(max),tbp.value) as Description
                            FROM sysobjects s
					     	LEFT JOIN sys.extended_properties as tbp ON s.id=tbp.major_id and tbp.minor_id=0  WHERE s.xtype IN('V')  AND (tbp.Name='MS_Description' OR tbp.Name is null)";
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
                return "select top 1 id from sysobjects";
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
        public override List<DbColumnInfo> GetColumnInfosByTableName(string tableName)
        {
            string cacheKey = "DbMaintenanceProvider.GetColumnInfosByTableName." + this.SqlBuilder.GetNoTranslationColumnName(tableName).ToLower();
            cacheKey = GetCacheKey(cacheKey);
            return this.Context.Utilities.GetReflectionInoCacheInstance().GetOrCreate(cacheKey,
                    () =>
                    {
                        string sql = "select * from " + tableName + " WHERE 1=2 ";
                        var oldIsEnableLog = this.Context.Ado.IsEnableLogEvent;
                        this.Context.Ado.IsEnableLogEvent = false;
                        using (DbDataReader reader = (DbDataReader)this.Context.Ado.GetDataReader(sql))
                        {
                            this.Context.Ado.IsEnableLogEvent = oldIsEnableLog;
                            List<DbColumnInfo> result = new List<DbColumnInfo>();
                            var schemaTable = reader.GetSchemaTable();
                            foreach (DataRow row in schemaTable.Rows)
                            {
                                DbColumnInfo column = new DbColumnInfo()
                                {
                                    TableName = tableName,
                                    DataType = row["DataType"].ToString().Replace("System.", "").Trim(),
                                    IsNullable = (bool)row["AllowDBNull"],
                                    //IsIdentity = (bool)row["IsAutoIncrement"],
                                    ColumnDescription = null,
                                    DbColumnName = row["ColumnName"].ToString(),
                                    //DefaultValue = row["defaultValue"].ToString(),
                                    IsPrimarykey = GetPrimaryKeyByTableNames(tableName).Any(it=>it.Equals(row["ColumnName"].ToString(), StringComparison.CurrentCultureIgnoreCase)),
                                    Length = row["ColumnSize"].ObjToInt(),
                                    Scale = row["numericscale"].ObjToInt()
                                };
                                result.Add(column);
                            }
                            return result;
                        }

                    });
        }
        private List<string> GetPrimaryKeyByTableNames(string tableName)
        {
            string cacheKey = "DbMaintenanceProvider.GetPrimaryKeyByTableNames." + this.SqlBuilder.GetNoTranslationColumnName(tableName).ToLower();
            cacheKey = GetCacheKey(cacheKey);
            return this.Context.Utilities.GetReflectionInoCacheInstance().GetOrCreate(cacheKey,
                    () =>
                    {
                        var oldIsEnableLog = this.Context.Ado.IsEnableLogEvent;
                        string sql = @" select cu.COLUMN_name KEYNAME  from user_cons_columns cu, user_constraints au 
                            where cu.constraint_name = au.constraint_name
                            and au.constraint_type = 'P' and au.table_name = '" +tableName.ToUpper()+ @"'";
                        var pks = this.Context.Ado.SqlQuery<string>(sql);
                        this.Context.Ado.IsEnableLogEvent = oldIsEnableLog;
                        return pks;
                    });
        }
        #endregion
    }
}
