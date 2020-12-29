using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class DmDbMaintenance : DbMaintenanceProvider
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
        protected override string IsAnyIndexSql
        {
            get
            {
                return "select count(1) from user_ind_columns where index_name=('{0}')";
            }
        }
        protected override string CreateIndexSql
        {
            get
            {
                return "CREATE {3} INDEX Index_{0}_{2} ON {0}({1})";
            }
        }
        protected override string AddDefaultValueSql
        {
            get
            {
                return "ALTER TABLE {0} MODIFY({1} DEFAULT '{2}')";
            }
        }
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
                return "ALTER TABLE {0} ADD PRIMARY KEY({2}) /*{1}*/";
            }
        }
        protected override string AddColumnToTableSql
        {
            get
            {
                return "ALTER TABLE {0} ADD ({1} {2}{3} {4} {5} {6})";
            }
        }
        protected override string AlterColumnToTableSql
        {
            get
            {
                return "ALTER TABLE {0} modify ({1} {2}{3} {4} {5} {6}) ";
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
                return "create table {1} as select * from {2}  where ROWNUM<={0}";
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
                return "ALTER TABLE {0} rename   column  {1} to {2}";
            }
        }
        protected override string AddColumnRemarkSql
        {
            get
            {
                return "comment on column {1}.{0} is '{2}'";
            }
        }

        protected override string DeleteColumnRemarkSql
        {
            get
            {
                return "comment on column {1}.{0} is ''";
            }
        }

        protected override string IsAnyColumnRemarkSql
        {
            get
            {
                return "select * from user_col_comments where Table_Name='{1}' AND COLUMN_NAME='{0}' order by column_name";
            }
        }

        protected override string AddTableRemarkSql
        {
            get
            {
                return "comment on table {0}  is  '{1}'";
            }
        }

        protected override string DeleteTableRemarkSql
        {
            get
            {
                return "comment on table {0}  is  ''";
            }
        }

        protected override string IsAnyTableRemarkSql
        {
            get
            {
                return "select * from user_tab_comments where Table_Name='{0}'order by Table_Name";
            }
        }

        protected override string RenameTableSql
        {
            get
            {
                return "alter table {0} rename to {1}";
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
                return "";
            }
        }
        protected override string CreateTableNotNull
        {
            get
            {
                return "";
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
        public override bool AddColumn(string tableName, DbColumnInfo columnInfo)
        {
            if (columnInfo.DataType == "varchar" && columnInfo.Length == 0)
            {
                columnInfo.DataType = "varchar2";
                columnInfo.Length = 50;
            }
            return base.AddColumn(tableName, columnInfo);
        }
        public override bool CreateIndex(string tableName, string[] columnNames, bool isUnique = false)
        {
            string sql = string.Format(CreateIndexSql, tableName, string.Join(",", columnNames), string.Join("_", columnNames.Select(it => (it + "abc").Substring(0, 3))), isUnique ? "UNIQUE" : "");
            this.Context.Ado.ExecuteCommand(sql);
            return true;
        }
        public override bool AddDefaultValue(string tableName, string columnName, string defaultValue)
        {
            if (defaultValue == "''")
            {
                defaultValue = "";
            }
            if (defaultValue.ToLower().IsIn("sysdate"))
            {
                var template = AddDefaultValueSql.Replace("'", "");
                string sql = string.Format(template, tableName, columnName, defaultValue);
                this.Context.Ado.ExecuteCommand(sql);
                return true;
            }
            else
            {
                return base.AddDefaultValue(tableName, columnName, defaultValue);
            }
        }
        public override bool CreateDatabase(string databaseDirectory = null)
        {
            throw new NotSupportedException();
        }
        public override bool CreateDatabase(string databaseName, string databaseDirectory = null)
        {
            throw new NotSupportedException();
        }
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
            string sql = "select * from " + SqlBuilder.GetTranslationTableName(tableName) + " WHERE 1=2 ";
            var oldIsEnableLog = this.Context.Ado.IsEnableLogEvent;
            this.Context.Ado.IsEnableLogEvent = false;
            using(DbDataReader reader = (DbDataReader) this.Context.Ado.GetDataReader(sql))
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
                        IsNullable = (bool) row["AllowDBNull"],
                        IsIdentity = (bool) row["IsIdentity"],
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
                    string sql = "SELECT TVNAME AS TableName, COLNAME as DbColumnName ,COMMENT$ AS ColumnDescription   from SYSCOLUMNCOMMENTS   WHERE TVNAME='" + tableName.ToUpper() + "' ORDER BY TVNAME";
                    var oldIsEnableLog = this.Context.Ado.IsEnableLogEvent;
                    this.Context.Ado.IsEnableLogEvent = false;
                    var pks = this.Context.Ado.SqlQuery<DbColumnInfo>(sql);
                    this.Context.Ado.IsEnableLogEvent = oldIsEnableLog;
                    return pks;
                });
            if (comments.HasValue())
            {
                var comment = comments.FirstOrDefault(it => it.DbColumnName.Equals(filedName, StringComparison.CurrentCultureIgnoreCase));
                return comment?.ColumnDescription;
            }
            else
            {
                return "";
            }

        }

        public override bool CreateTable(string tableName, List<DbColumnInfo> columns, bool isCreatePrimaryKey = true)
        {
            if (columns.HasValue())
            {
                foreach (var item in columns)
                {
                    if (item.DbColumnName.Equals("GUID", StringComparison.CurrentCultureIgnoreCase) && item.Length == 0)
                    {
                        item.Length = 10;
                    }
                }
            }
            string sql = GetCreateTableSql(tableName, columns);
            string primaryKeyInfo = null;
            if (columns.Any(it => it.IsPrimarykey) && isCreatePrimaryKey)
            {
                primaryKeyInfo = string.Format(", Primary key({0})", string.Join(",", columns.Where(it => it.IsPrimarykey).Select(it => this.SqlBuilder.GetTranslationColumnName(it.DbColumnName.ToLower()))));

            }
            sql = sql.Replace("$PrimaryKey", primaryKeyInfo);
            this.Context.Ado.ExecuteCommand(sql);
            return true;
        }
        #endregion
    }
}