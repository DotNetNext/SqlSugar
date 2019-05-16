using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class DB2DbMaintenance : DbMaintenanceProvider
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
                return @"SELECT name FROM SYSIBM.SYSTABLES WHERE TYPE = 'T' AND CREATOR = (SELECT CURRENT SCHEMA FROM SYSIBM.SYSDUMMY1)";

            }
        }
        protected override string GetViewInfoListSql
        {
            get
            {
                return @"SELECT Name FROM SYSIBM.SYSTABLES WHERE TYPE = 'V' AND CREATOR = (SELECT CURRENT SCHEMA FROM SYSIBM.SYSDUMMY1) ";
            }
        }
        #endregion

        #region DDL
        protected override string AddPrimaryKeySql
        {
            get
            {
                return @"ALTER TABLE {0} ADD CONSTRAINT {1} PRIMARY KEY({2})";
            }
        }
        protected override string AddColumnToTableSql
        {
            get
            {
                //return "ALTER TABLE {0} ADD ({1} {2}{3} {4} {5} {6})";
                return "ALTER TABLE {0} ADD COLUMN {1} {2}{3}";
            }
        }
        protected override string AlterColumnToTableSql
        {
            get
            {
                //return "ALTER TABLE {0} modify ({1} {2}{3} {4} {5} {6}) ";
                return "alter table {0} alter column {1} set data type {2} {3}";
            }
        }
        protected override string BackupDataBaseSql
        {
            get
            {
                //return @"USE master;BACKUP DATABASE {0} TO disk = '{1}'";
                return @"db2 backup db {0} TO {1}";
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
                return "create table {1} as select * from {2} fetch first  {0} rows only";
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
                //return "ALTER TABLE {0} DROP CONSTRAINT  {1}";
                return "ALTER TABLE {0} DROP COLUMN {1} CASCADE ";
            }
        }
        protected override string RenameColumnSql
        {
            //get
            //{
            //    return "ALTER TABLE {0} rename   column  {1} to {2}";
            //}
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
                return "select Name from SYSIBM.SYSTABLES  fetch first  1 rows only";
            }
        }
        #endregion

        #region Scattered
        protected override string CreateTableNull
        {
            get
            {
                return "DEFAULT NULL";
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
                return "GENERATED ALWAYS AS IDENTITY (START WITH 1, INCREMENT BY 1 )";
            }
        }

        protected override string AddColumnRemarkSql
        {
            get
            {
                //return "comment on column {1}.{0} is '{2}'";
                return "COMMENT ON {1} ({0} IS '{2}' )";
            }
        }

        protected override string DeleteColumnRemarkSql
        {
            get
            {
                return "COMMENT ON {1} ({0} IS '' )";
            }
        }

        protected override string IsAnyColumnRemarkSql
        {
            get
            {
                //return "select * from user_col_comments where Table_Name='{1}' AND COLUMN_NAME='{0}' order by column_name";
                return "select t.Remarks as name from syscat.COLUMNS t  where TABNAME='{1}' and COLNAME='{0}' and t.REMARKS is not null";
            }
        }

        protected override string AddTableRemarkSql
        {
            get
            {
                return "";
            }
        }

        protected override string DeleteTableRemarkSql
        {
            get
            {
                return "";
            }
        }

        protected override string IsAnyTableRemarkSql
        {
            get
            {
                return "";
            }
        }

        protected override string RenameTableSql {
            get
            {
                return "alter table {0} rename to {1}";
            }
        }
        #endregion

        #region Methods
        public override bool AddRemark(EntityInfo entity)
        {
            var db = this.Context;
            var columns = entity.Columns.Where(it => it.IsIgnore == false).ToList();

            foreach (var item in columns)
            {
                if (item.ColumnDescription != null)
                {
                    //column remak
                    if (db.DbMaintenance.IsAnyColumnRemark(item.DbColumnName.ToUpper(), item.DbTableName.ToUpper()))
                    {
                        db.DbMaintenance.DeleteColumnRemark(item.DbColumnName.ToUpper(), item.DbTableName.ToUpper());
                        db.DbMaintenance.AddColumnRemark(item.DbColumnName.ToUpper(), item.DbTableName.ToUpper(), item.ColumnDescription);
                    }
                    else
                    {
                        db.DbMaintenance.AddColumnRemark(item.DbColumnName.ToUpper(), item.DbTableName.ToUpper(), item.ColumnDescription);
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
        public override List<DbColumnInfo> GetColumnInfosByTableName(string tableName, bool isCache = true)
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
                        //string sql = @" select distinct cu.COLUMN_name KEYNAME  from user_cons_columns cu, user_constraints au 
                        //    where cu.constraint_name = au.constraint_name
                        //    and au.constraint_type = 'P' and au.table_name = '" + tableName.ToUpper() + @"'";
                        string sql = @"select * from 	(select constname, TRIM(tabschema)||'.'||TRIM(tabname) tbname, type from syscat.tabconst) t where t.tbname='" + tableName.ToUpper() + @"' AND t.TYPE='P' ";
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
                              //string sql = "SELECT COMMENTS FROM USER_TAB_COMMENTS WHERE TABLE_NAME =@tableName ORDER BY TABLE_NAME";
                              string sql = @"SELECT  T.REMARKS FROM (SELECT
    trim(VARCHAR(TABSCHEMA, 10)) || '.' || trim(VARCHAR(TABNAME, 50))   AS TABNAME, --表名
    REMARKS--表的注释
FROM
    SYSCAT.TABLES WHERE TYPE = 'T') T
  WHERE
    T.TABNAME = 'ERP.VIRTUALEMPLOYEETOMES'";
                              var oldIsEnableLog = this.Context.Ado.IsEnableLogEvent;
                              this.Context.Ado.IsEnableLogEvent = false;
                              var pks = this.Context.Ado.SqlQuery<string>(sql, new { tableName = tableName.ToUpper() });
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
            return comments.HasValue() ? comments.First(it => it.DbColumnName.Equals(filedName, StringComparison.CurrentCultureIgnoreCase)).ColumnDescription : "";

        }

        public override bool CreateTable(string tableName, List<DbColumnInfo> columns, bool isCreatePrimaryKey = true)
        {
            if (columns.HasValue())
            {
                foreach (var item in columns)
                {
                    if (item.DbColumnName.Equals("GUID", StringComparison.CurrentCultureIgnoreCase) && item.Length == 0)
                    {
                        item.Length = 50;
                    }
                    if (item.DataType == "varchar" && item.Length == 0)
                    {
                        item.Length = 50;
                    }
                }
            }
            string sql = GetCreateTableSql(tableName, columns);
            this.Context.Ado.ExecuteCommand(sql);
            if (isCreatePrimaryKey)
            {
                var pkColumns = columns.Where(it => it.IsPrimarykey).ToList();
                foreach (var item in pkColumns)
                {
                    this.Context.DbMaintenance.AddPrimaryKey(tableName, item.DbColumnName);
                }
            }
            return true;
        }
        #endregion
    }
}
