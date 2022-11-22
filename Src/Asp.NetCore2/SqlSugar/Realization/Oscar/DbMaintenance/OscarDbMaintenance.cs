using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class OscarDbMaintenance : DbMaintenanceProvider
    {
        #region DML
        protected override string GetDataBaseSql
        {
            get
            {
                return "SELECT datname FROM sys_database ";
            }
        }
        protected override string GetColumnInfosByTableNameSql
        {
            get
            {
                string sql = @" SELECT 
                                A.COLUMN_NAME AS DbColumnName,
                                A.TABLE_NAME AS TableName,
                                A.DATA_TYPE AS DataType,
                                 case when  DATA_DEFAULT like 'NEXTVAL%'  then true else false end as IsIdentity,
                                 case when A.NULLABLE = 'Y'  then true else false end as IsNullable   ,    
                                 A.DATA_LENGTH AS LENGTH,
                                 B.COMMENTS AS ColumnDescription,                             
                                 CASE WHEN K.COLUMN_NAME IS NULL THEN FALSE ELSE TRUE END AS IsPrimarykey,
                                 DATA_SCALE AS DecimalDigits,
                                 A.DATA_PRECISION AS  SCALE,
                                 A.DATA_DEFAULT as DefaultValue
                                FROM
                                INFO_SCHEM.ALL_TAB_COLUMNS A
                                LEFT JOIN INFO_SCHEM.SYS_CLASS T ON T.RELNAME=A.TABLE_NAME
                                LEFT JOIN INFO_SCHEM.ALL_COL_COMMENTS B  ON A.TABLE_NAME=B.TABLE_NAME AND A.COLUMN_NAME=B.COLUMN_NAME 
                                LEFT JOIN INFO_SCHEM.SYS_ATTRIBUTE C ON C.ATTNAME=A.COLUMN_NAME AND C.ATTRELID=T.OID
                                LEFT JOIN INFO_SCHEM.V_SYS_PRIMARY_KEYS K ON A.TABLE_NAME=K.TABLE_NAME AND  K.COLUMN_NAME=A.COLUMN_NAME
                                WHERE upper(A.TABLE_NAME)=upper('{0}')  
                                 ORDER BY c.ATTNUM 
                                ";
                return sql;
            }
        }
        protected override string GetTableInfoListSql
        {
            get
            {
                //AND t.relnamespace=1 表空间限制。
                return @" select cast(relname as varchar(500)) as Name , DESCRIPTION AS Description FROM sys_class  t 
                        LEFT JOIN sys_description  d ON t.OID=d.OBJOID AND d.OBJSUBID=0 
                        WHERE  t.relvbase>0 AND t.relkind = 'r' AND t.relnamespace=(SELECT OID FROM  sys_namespace WHERE nspname =USER)
                        order by relname";
            }
        }
        protected override string GetViewInfoListSql
        {
            get
            {
                return @" select cast(relname as varchar(500)) as Name , DESCRIPTION AS Description FROM sys_class  t 
                        LEFT JOIN sys_description  d ON t.OID=d.OBJOID AND d.OBJSUBID=0 
                        WHERE  t.relvbase>0 AND t.relkind = 'v'  AND t.relnamespace=(SELECT OID FROM  sys_namespace WHERE nspname =USER)
                        order by relname";
            }
        }
        #endregion

        #region DDL
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
                return "ALTER TABLE {0} ADD COLUMN {1} {2}{3} {4} {5} {6}";
            }
        }
        protected override string AlterColumnToTableSql
        {
            get
            {
                return "alter table {0} ALTER COLUMN {1} {2}{3} {4} {5} {6}";
            }
        }
        protected override string BackupDataBaseSql
        {
            get
            {
                return "mysqldump.exe  {0} -uroot -p > {1}  ";
            }
        }
        protected override string CreateTableSql
        {
            get
            {
                return "CREATE TABLE {0}(\r\n{1} $PrimaryKey)";
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
                return "create table {0} as (select * from {1} limit {2} offset 0)";
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
                return "ALTER TABLE {0} DROP CONSTRAINT {1}";
            }
        }
        protected override string RenameColumnSql
        {
            get
            {
                return "ALTER TABLE {0} RENAME {1} TO {2}";
            }
        }
        protected override string AddColumnRemarkSql => "comment on column {1}.{0} is '{2}'";

        protected override string DeleteColumnRemarkSql => "comment on column {1}.{0} is ''";

        protected override string IsAnyColumnRemarkSql { get { throw new NotSupportedException(); } }

        protected override string AddTableRemarkSql => "comment on table {0} is '{1}'";

        protected override string DeleteTableRemarkSql => "comment on table {0} is ''";

        protected override string IsAnyTableRemarkSql { get { throw new NotSupportedException(); } }

        protected override string RenameTableSql => "alter table 表名 {0} to {1}";

        protected override string CreateIndexSql
        {
            get
            {
                return "CREATE {3} INDEX Index_{0}_{2} ON {0} ({1})";
            }
        }
        protected override string AddDefaultValueSql
        {
            get
            {
                return "ALTER TABLE {0} ALTER COLUMN {1} SET DEFAULT {2}";
            }
        }
        protected override string IsAnyIndexSql
        {
            get
            {
                return "  Select count(1) from (SELECT to_regclass('{0}') as c ) t where t.c is not null";
            }
        }
        protected override string IsAnyProcedureSql => throw new NotImplementedException();

        #endregion

        #region Check
        protected override string CheckSystemTablePermissionsSql
        {
            get
            {
                return "select 1 from INFO_SCHEM.ALL_TAB_COLUMNS limit 1 offset 0";
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
                return "serial";
            }
        }
        #endregion

        #region Methods
        public override bool UpdateColumn(string tableName, DbColumnInfo columnInfo)
        {

            tableName = this.SqlBuilder.GetTranslationTableName(tableName);
            var columnName= this.SqlBuilder.GetTranslationColumnName(columnInfo.DbColumnName);
            string type = GetType(tableName, columnInfo);
            //this.Context.Ado.ExecuteCommand(sql);

            string sql= @"ALTER TABLE {table} ALTER TYPE {column} {type};ALTER TABLE {table} ALTER COLUMN {column} {null}";

            var isnull = columnInfo.IsNullable?" DROP NOT NULL ": " SET NOT NULL ";

            sql = sql.Replace("{table}", tableName)
                .Replace("{type}", type)
                .Replace("{column}", columnName)
                .Replace("{null}", isnull); 
            this.Context.Ado.ExecuteCommand(sql);
            return true;
        }

        protected string GetType(string tableName, DbColumnInfo columnInfo)
        {
            string columnName = this.SqlBuilder.GetTranslationColumnName(columnInfo.DbColumnName);
            tableName = this.SqlBuilder.GetTranslationTableName(tableName);
            string dataSize = GetSize(columnInfo);
            string dataType = columnInfo.DataType;
            //if (!string.IsNullOrEmpty(dataType))
            //{
            //    dataType =  dataType;
            //}
            return dataType +""+ dataSize;
        }
        protected override string GetUpdateColumnSql(string tableName, DbColumnInfo columnInfo)
        {
            string columnName = this.SqlBuilder.GetTranslationColumnName(columnInfo.DbColumnName);
            tableName = this.SqlBuilder.GetTranslationTableName(tableName);
            string dataSize = GetSize(columnInfo);
            string dataType = columnInfo.DataType;
            if (!string.IsNullOrEmpty(dataType))
            {
                dataType = " type " + dataType;
            }
            string nullType = "";
            string primaryKey = null;
            string identity = null;
            string result = string.Format(this.AlterColumnToTableSql, tableName, columnName, dataType, dataSize, nullType, primaryKey, identity);
            return result;
        }

        /// <summary>
        ///by current connection string
        /// </summary>
        /// <param name="databaseDirectory"></param>
        /// <returns></returns>
        public override bool CreateDatabase(string databaseName, string databaseDirectory = null)
        {
            throw new NotSupportedException("Not Supported CreateDatabase");
        }
        public override bool AddRemark(EntityInfo entity)
        {
            var db = this.Context;
            var columns = entity.Columns.Where(it => it.IsIgnore == false).ToList();

            foreach (var item in columns)
            {
                if (item.ColumnDescription != null)
                {
                    db.DbMaintenance.AddColumnRemark(item.DbColumnName, item.DbTableName, item.ColumnDescription);

                }
            }
            //table remak
            if (entity.TableDescription != null)
            {
                db.DbMaintenance.AddTableRemark(entity.DbTableName, entity.TableDescription);
            }
            return true;
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
        protected override string GetCreateTableSql(string tableName, List<DbColumnInfo> columns)
        {
            List<string> columnArray = new List<string>();
            Check.Exception(columns.IsNullOrEmpty(), "No columns found ");
            foreach (var item in columns)
            {
                string columnName = item.DbColumnName;
                string dataType = item.DataType;
                if (dataType == "varchar" && item.Length == 0)
                {
                    item.Length = 1;
                }
                if (dataType == "uuid")
                {
                    item.Length = 50;
                    dataType = "varchar";
                }
                string dataSize = item.Length > 0 ? string.Format("({0})", item.Length) : null;
                if (item.DecimalDigits > 0&&item.Length>0 && dataType == "numeric") 
                {
                    dataSize = $"({item.Length},{item.DecimalDigits})";
                }
                string nullType = item.IsNullable ? this.CreateTableNull : CreateTableNotNull;
                string primaryKey = null;
                string addItem = string.Format(this.CreateTableColumn, this.SqlBuilder.GetTranslationColumnName(columnName.ToLower()), dataType, dataSize, nullType, primaryKey, "");
                if (item.IsIdentity)
                {
                    string length = dataType.Substring(dataType.Length - 1);
                    string identityDataType = "serial" + length;
                    addItem = addItem.Replace(dataType, identityDataType);
                }
                columnArray.Add(addItem);
            }
            string tableString = string.Format(this.CreateTableSql, this.SqlBuilder.GetTranslationTableName(tableName.ToLower()), string.Join(",\r\n", columnArray));
            return tableString;
        }
        public override bool IsAnyConstraint(string constraintName)
        {
            throw new NotSupportedException("PgSql IsAnyConstraint NotSupportedException");
        }
        public override bool BackupDataBase(string databaseName, string fullFileName)
        {
            Check.ThrowNotSupportedException("PgSql BackupDataBase NotSupported");
            return false;
        }

        public override List<DbColumnInfo> GetColumnInfosByTableName(string tableName, bool isCache = true)
        {
            var result= base.GetColumnInfosByTableName(tableName.TrimStart('"').TrimEnd('"'), isCache);
            foreach (var columnInfo in result) 
            {
                if (columnInfo.IsIdentity && !columnInfo.DataType.ObjToString().ToLower().Contains("int")) 
                {
                    columnInfo.IsIdentity = false;
                }
            }
            string sql = "select * from " + SqlBuilder.GetTranslationTableName(tableName) + " WHERE 1=2 ";
            var oldIsEnableLog = this.Context.Ado.IsEnableLogEvent;
            this.Context.Ado.IsEnableLogEvent = false;
            using (DbDataReader reader = (DbDataReader)this.Context.Ado.GetDataReader(sql))
            {
                this.Context.Ado.IsEnableLogEvent = oldIsEnableLog;
                var schemaTable = reader.GetSchemaTable();
                foreach (System.Data.DataRow row in schemaTable.Rows)
                {
                    var name = row["columnname"] + "";
                    var data = result.First(it => it.DbColumnName.Equals(name, StringComparison.OrdinalIgnoreCase));
                    data.IsPrimarykey= row["iskey"].ToString() =="True"? true : false;
                }
            }
            return result;
        }
        #endregion
    }
}
