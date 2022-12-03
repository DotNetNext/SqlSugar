using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlSugar.MySqlConnector
{
    public class MySqlDbMaintenance : DbMaintenanceProvider
    {
        #region DML
        protected override string GetDataBaseSql
        {
            get
            {
                return "SHOW DATABASES";
            }
        }
        protected override string GetColumnInfosByTableNameSql
        {
            get
            {
                string sql = @"SELECT
                                    0 as TableId,
                                    TABLE_NAME as TableName, 
                                    column_name AS DbColumnName,
                                    CASE WHEN  left(COLUMN_TYPE,LOCATE('(',COLUMN_TYPE)-1)='' THEN COLUMN_TYPE ELSE  left(COLUMN_TYPE,LOCATE('(',COLUMN_TYPE)-1) END   AS DataType,
                                    CAST(SUBSTRING(COLUMN_TYPE,LOCATE('(',COLUMN_TYPE)+1,LOCATE(')',COLUMN_TYPE)-LOCATE('(',COLUMN_TYPE)-1) AS signed) AS Length,
                                    column_default  AS  `DefaultValue`,
                                    column_comment  AS  `ColumnDescription`,
                                    CASE WHEN COLUMN_KEY = 'PRI'
                                    THEN true ELSE false END AS `IsPrimaryKey`,
                                    CASE WHEN EXTRA='auto_increment' THEN true ELSE false END as IsIdentity,
                                    CASE WHEN is_nullable = 'YES'
                                    THEN true ELSE false END AS `IsNullable`,
                                    numeric_scale as Scale,
                                    numeric_scale as DecimalDigits,
                                    LOCATE(  'unsigned',COLUMN_type   ) >0  as IsUnsigned
                                    FROM
                                    Information_schema.columns where TABLE_NAME='{0}' and  TABLE_SCHEMA=(select database()) ORDER BY ordinal_position";
                return sql;
            }
        }
        protected override string GetTableInfoListSql
        {
            get
            {
                return @"select TABLE_NAME as Name,TABLE_COMMENT as Description from information_schema.tables
                         where  TABLE_SCHEMA=(select database())  AND TABLE_TYPE='BASE TABLE'";
            }
        }
        protected override string GetViewInfoListSql
        {
            get
            {
                return @"select TABLE_NAME as Name,TABLE_COMMENT as Description from information_schema.tables
                         where  TABLE_SCHEMA=(select database()) AND TABLE_TYPE='VIEW'
                         ";
            }
        }
        #endregion

        #region DDL
        protected override string CreateDataBaseSql
        {
            get
            {
                return "CREATE DATABASE `{0}` CHARACTER SET utf8 COLLATE utf8_general_ci ";
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
                return "ALTER TABLE {0} ADD {1} {2}{3} {4} {5} {6}";
            }
        }
        protected override string AlterColumnToTableSql
        {
            get
            {
                // return "ALTER TABLE {0} ALTER COLUMN {1} {2}{3} {4} {5} {6}";
                return "alter table {0} change  column {1} {1} {2}{3} {4} {5} {6}";
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
                return "Create table {1} (Select * from {2} LIMIT 0,{0})";
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
                return "ALTER TABLE {0} drop primary key;";
            }
        }
        protected override string RenameColumnSql
        {
            get
            {
                return "alter table {0} change  column {1} {2}";
            }
        }
        protected override string IsAnyProcedureSql
        {
            get 
            {
                return "select count(*) from information_schema.Routines where ROUTINE_NAME='{0}' and ROUTINE_TYPE='PROCEDURE'";
            }
        }
        #endregion

        #region Check
        protected override string CheckSystemTablePermissionsSql
        {
            get
            {
                return "select 1 from Information_schema.columns limit 0,1";
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
                return "AUTO_INCREMENT";
            }
        }

        protected override string AddColumnRemarkSql
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        protected override string DeleteColumnRemarkSql
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        protected override string IsAnyColumnRemarkSql
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        protected override string AddTableRemarkSql
        {
            get
            {
                 return "ALTER TABLE {0} COMMENT='{1}';";
            }
        }

        protected override string DeleteTableRemarkSql
        {
            get
            {
                return "ALTER TABLE {0} COMMENT='';";
            }
        }

        protected override string IsAnyTableRemarkSql
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        protected override string RenameTableSql {
            get
            {
                return "alter table {0} rename {1}";
            }
        }

        protected override string CreateIndexSql
        {
            get
            {
                return "CREATE {3} INDEX `Index_{0}_{2}` ON `{0}` ({1})";
            }
        }

        protected override string AddDefaultValueSql
        {
            get
            {
                return "ALTER TABLE `{0}` ALTER COLUMN `{1}` SET DEFAULT '{2}'";
            }
        }
        protected override string IsAnyIndexSql
        {
            get
            {
                return "SELECT count(*) FROM information_schema.statistics WHERE index_name = '{0}' and index_schema = '{1}'";
            }
        }
        #endregion

        #region Methods
        public override bool IsAnyTable(string tableName, bool isCache = true)
        {
            try
            {
                return base.IsAnyTable(tableName, isCache);
            }
            catch (Exception ex)
            {
                if (SugarCompatible.IsFramework && ex.Message == "Invalid attempt to Read when reader is closed.")
                {
                    Check.ExceptionEasy($"To upgrade the MySql.Data. Error:{ex.Message}", $" 请先升级MySql.Data 。 详细错误:{ex.Message}");
                    return true;
                }
                else
                {
                    throw;
                }
            }
        }
        public override bool IsAnyColumnRemark(string columnName, string tableName)
        {
            var isAny=this.Context.DbMaintenance.GetColumnInfosByTableName(tableName, false)
                .Any(it => it.ColumnDescription.HasValue() && it.DbColumnName.EqualCase(columnName));
            return isAny;
        }
        public override bool AddColumnRemark(string columnName, string tableName, string description)
        {
            //base.AddColumnRemark(columnName, tableName, description);
            var message= @"db.DbMaintenance.UpdateColumn(""tablename"", new DbColumnInfo()
            {{
                DataType = ""VARCHAR(30) NOT NULL COMMENT 'xxxxx'"",
                DbColumnName = ""columnname""
            }})" ;
            Check.Exception(true,"MySql no support AddColumnRemark , use " + message);
            return true;
        }
        /// <summary>
        ///by current connection string
        /// </summary>
        /// <param name="databaseDirectory"></param>
        /// <returns></returns>
        public override bool CreateDatabase(string databaseName, string databaseDirectory = null)
        {

            if (this.Context.Ado.IsValidConnection()) 
            {
                return true;
            }

            if (databaseDirectory != null)
            {
                if (!FileHelper.IsExistDirectory(databaseDirectory))
                {
                    FileHelper.CreateDirectory(databaseDirectory);
                }
            }
            var oldDatabaseName = this.Context.Ado.Connection.Database;
            var connection = this.Context.CurrentConnectionConfig.ConnectionString;
            Check.ExceptionEasy(Regex.Split(connection,oldDatabaseName).Length > 2
                , "The user name and password cannot be the same as the database name ",
                " 创建数据库失败, 请换一个库名，库名不能 password 或者 username 有重叠 ");
            connection = connection.Replace(oldDatabaseName, "mysql");
            var newDb = new SqlSugarClient(new ConnectionConfig()
            {
                DbType = this.Context.CurrentConnectionConfig.DbType,
                IsAutoCloseConnection = true,
                ConnectionString = connection
            });
            if (!GetDataBaseList(newDb).Any(it => it.Equals(databaseName, StringComparison.CurrentCultureIgnoreCase)))
            {
                newDb.Ado.ExecuteCommand(string.Format(CreateDataBaseSql, databaseName, databaseDirectory));
            }
            return true;
        }
        public override bool AddTableRemark(string tableName, string description)
        {
            string sql = string.Format(this.AddTableRemarkSql, this.SqlBuilder.GetTranslationTableName(tableName), description);
            this.Context.Ado.ExecuteCommand(sql);
            return true;
        }
        public override bool CreateTable(string tableName, List<DbColumnInfo> columns, bool isCreatePrimaryKey = true)
        {
            if (columns.HasValue())
            {
                foreach (var item in columns)
                {
                    if (item.DbColumnName.Equals("GUID",StringComparison.CurrentCultureIgnoreCase)&&item.Length==0)
                    {
                        item.Length = 10;
                    }
                }
            }
            string sql = GetCreateTableSql(tableName, columns);
            string primaryKeyInfo = null;
            if (columns.Any(it => it.IsPrimarykey)&&isCreatePrimaryKey) {
                primaryKeyInfo =string.Format( ", Primary key({0})",string.Join(",",columns.Where(it=>it.IsPrimarykey).Select(it=>this.SqlBuilder.GetTranslationColumnName(it.DbColumnName))));

            }
            sql = sql.Replace("$PrimaryKey", primaryKeyInfo);
            this.Context.Ado.ExecuteCommand(sql);
            return true;
        }
        public override bool AddRemark(EntityInfo entity)
        {
            var db = this.Context;
            db.DbMaintenance.AddTableRemark(entity.DbTableName, entity.TableDescription);
            List<EntityColumnInfo> columns = entity.Columns.Where(it => it.IsIgnore == false).ToList();
            foreach (var item in columns)
            {
                if (item.ColumnDescription != null)
                {
                    var mySqlCodeFirst = this.Context.CodeFirst as MySqlCodeFirst;
                    if (item.UnderType == UtilConstants.GuidType&&item.Length==0) 
                    {
                        item.Length = 36;
                    }
                    string sql = GetUpdateColumnSql(entity.DbTableName, mySqlCodeFirst.GetEntityColumnToDbColumn(entity, entity.DbTableName, item))+" "+(item.IsIdentity? "AUTO_INCREMENT" : "")+" " + " COMMENT '" + item.ColumnDescription + "'";
                    db.Ado.ExecuteCommand(sql);
                }
            }
            return true;
        }
        protected override string GetCreateTableSql(string tableName, List<DbColumnInfo> columns)
        {
            List<string> columnArray = new List<string>();
            Check.Exception(columns.IsNullOrEmpty(), "No columns found ");
            foreach (var item in columns)
            {
                string columnName = item.DbColumnName;
                string dataSize = "";
                dataSize = GetSize(item);
                string dataType = item.DataType;
                string nullType = item.IsNullable ? this.CreateTableNull : CreateTableNotNull;
                string primaryKey = null;
                string identity = item.IsIdentity ? this.CreateTableIdentity : null;
                string addItem = string.Format(this.CreateTableColumn, this.SqlBuilder.GetTranslationColumnName(columnName), dataType, dataSize, nullType, primaryKey, identity);
                if (!string.IsNullOrEmpty(item.ColumnDescription))
                {
                    addItem += " COMMENT '"+item.ColumnDescription.ToSqlFilter()+"' ";
                }
                columnArray.Add(addItem);
            }
            string tableString = string.Format(this.CreateTableSql, this.SqlBuilder.GetTranslationTableName(tableName), string.Join(",\r\n", columnArray));
            return tableString;
        }

        protected override string GetSize(DbColumnInfo item)
        {
            string dataSize = null;
            var isMax = item.Length > 4000 || item.Length == -1;
            if (isMax)
            {
                dataSize="";
                item.DataType = "longtext";
            }
            else if (item.Length > 0 && item.DecimalDigits == 0)
            {
                dataSize = item.Length > 0 ? string.Format("({0})", item.Length) : null;
            }
            else if (item.Length == 0 && item.DecimalDigits > 0)
            {
                item.Length = 10;
                dataSize = string.Format("({0},{1})", item.Length, item.DecimalDigits);
            }
            else if (item.Length > 0 && item.DecimalDigits > 0)
            {
                dataSize = item.Length > 0 ? string.Format("({0},{1})", item.Length, item.DecimalDigits) : null;
            }
            return dataSize;
        }

        public override bool RenameColumn(string tableName, string oldColumnName, string newColumnName)
        {
            var columns=GetColumnInfosByTableName(tableName).Where(it=>it.DbColumnName.Equals(oldColumnName,StringComparison.CurrentCultureIgnoreCase));
            if (columns != null && columns.Any())
            {
                var column = columns.First();
                var appendSql = " " + column.DataType;
                if (column.Length > 0 && column.Scale == 0)
                {
                    appendSql += string.Format("({0}) ", column.Length);
                }
                else if (column.Scale > 0 && column.Length > 0)
                {
                    appendSql += string.Format("({0},{1}) ", column.Length, column.Scale);
                }
                else
                {
                    appendSql += column.IsNullable ? " NULL " : " NOT NULL ";
                }
                tableName = this.SqlBuilder.GetTranslationTableName(tableName);
                oldColumnName = this.SqlBuilder.GetTranslationColumnName(oldColumnName);
                newColumnName = this.SqlBuilder.GetTranslationColumnName(newColumnName);
                string sql = string.Format(this.RenameColumnSql, tableName, oldColumnName, newColumnName+appendSql);
                this.Context.Ado.ExecuteCommand(sql);
                return true;
            }
            else
            {
                return false;
            }
        }
        public override bool AddDefaultValue(string tableName, string columnName, string defaultValue)
        {
            if (defaultValue == "''")
            {
                defaultValue = "";
            }
            if (defaultValue.ToLower().IsIn("now()", "current_timestamp") || defaultValue.ToLower().Contains("current_timestamp"))
            {
                string template = "ALTER table {0} CHANGE COLUMN {1} {1} {3} {4} default {2} COMMENT '{5}'";
                var dbColumnInfo = this.Context.DbMaintenance.GetColumnInfosByTableName(tableName).First(it => it.DbColumnName.Equals(columnName, StringComparison.CurrentCultureIgnoreCase));
                var value = Regex.Match(defaultValue, @"\(\d\)$").Value;
                string sql = string.Format(template, tableName, columnName, defaultValue, dbColumnInfo.DataType + value, dbColumnInfo.IsNullable ? " NULL " : " NOT NULL ", dbColumnInfo.ColumnDescription);
                this.Context.Ado.ExecuteCommand(sql);
                return true;
            }
            else if (defaultValue == "0" || defaultValue == "1")
            {
                string sql = string.Format(AddDefaultValueSql.Replace("'", ""), tableName, columnName, defaultValue);
                this.Context.Ado.ExecuteCommand(sql);
                return true;
            }
            else
            {
                return base.AddDefaultValue(tableName, columnName, defaultValue);
            }
        }
        public override bool IsAnyConstraint(string constraintName)
        {
            throw new NotSupportedException("MySql IsAnyConstraint NotSupportedException");
        }
        public override bool BackupDataBase(string databaseName, string fullFileName)
        {
            Check.ThrowNotSupportedException("MySql BackupDataBase NotSupported");
            return false;
        }

        #endregion
    }
}
