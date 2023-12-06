using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
 
namespace SqlSugar
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
                                    CAST(SUBSTRING(COLUMN_TYPE,LOCATE('(',COLUMN_TYPE)+1,LOCATE(')',COLUMN_TYPE)-LOCATE('(',COLUMN_TYPE)-1) AS  decimal(18,0) ) AS Length,
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
        public override List<string> GetDbTypes()
        {
            return this.Context.Ado.SqlQuery<string>(@"SELECT DISTINCT DATA_TYPE
FROM information_schema.COLUMNS");
        }
        public override List<string> GetTriggerNames(string tableName)
        {
            return this.Context.Ado.SqlQuery<string>(@"SELECT TRIGGER_NAME
FROM INFORMATION_SCHEMA.TRIGGERS
WHERE EVENT_OBJECT_TABLE = '" + tableName + "'");
        }
        public override List<string> GetFuncList()
        {
            return this.Context.Ado.SqlQuery<string>(" SELECT routine_name\r\nFROM information_schema.ROUTINES\r\nWHERE routine_schema = (SELECT DATABASE()) AND routine_type = 'FUNCTION'; ");
        }
        public override List<string> GetIndexList(string tableName)
        {
            var sql = $"SHOW INDEX FROM {this.SqlBuilder.GetTranslationColumnName(tableName)}";
            return this.Context.Ado.GetDataTable(sql).AsEnumerable().Cast<DataRow>().Select(it => it["key_name"]).Cast<string>().ToList();
        }
        public override List<string> GetProcList(string dbName)
        {
            var sql = $"SELECT ROUTINE_NAME FROM information_schema.ROUTINES WHERE ROUTINE_TYPE = 'PROCEDURE' AND ROUTINE_SCHEMA = '{dbName}'";
            return this.Context.Ado.SqlQuery<string>(sql);
        }
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

            tableName = this.SqlBuilder.GetTranslationColumnName(tableName);
            columnName=this.SqlBuilder.GetTranslationColumnName(columnName);
            var sql = this.Context.Ado.GetDataTable($"SHOW CREATE TABLE {tableName};").Rows[0][1]+"";
            var columns=sql.Split('\n');
            var columnList = columns.Where(it => it.Last()==',').ToList();
            
            foreach ( var column in columnList ) 
            {
                if (column.Contains(columnName)) 
                {
                    Regex regex = new Regex(" COMMENT .+$");
                    var newcolumn = regex.Replace(column.TrimEnd(','), "");
                    newcolumn += $" COMMENT '{description.ToSqlFilter()}'  ";
                    var updateSql = $"ALTER TABLE {tableName} MODIFY COLUMN " + newcolumn.TrimEnd(',');
                    this.Context.Ado.ExecuteCommand(updateSql);
                    break;
                }
            }
            ////base.AddColumnRemark(columnName, tableName, description);
            //var message= @"db.DbMaintenance.UpdateColumn(""tablename"", new DbColumnInfo()
            //{{
            //    DataType = ""VARCHAR(30) NOT NULL COMMENT 'xxxxx'"",
            //    DbColumnName = ""columnname""
            //}})" ;
            //Check.Exception(true,"MySql no support AddColumnRemark , use " + message);
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
            if (Regex.Split(connection, oldDatabaseName).Length > 2)
            {
                var name = Regex.Match(connection, @"database\=\w+|datasource\=\w+", RegexOptions.IgnoreCase).Value;
                if (!string.IsNullOrEmpty(name))
                {
                    connection = connection.Replace(name, "database=mysql");
                }
                else
                {
                    Check.ExceptionEasy("Failed to create the database. The database name has a keyword. Please change the name", "建库失败，库名存在关键字，请换一个名字");
                }
            }
            else
            {
                connection = connection.Replace(oldDatabaseName, "mysql");
            }
            var newDb = new SqlSugarClient(new ConnectionConfig()
            {
                DbType = this.Context.CurrentConnectionConfig.DbType,
                IsAutoCloseConnection = true,
                ConnectionString = connection
            });
            if (!GetDataBaseList(newDb).Any(it => it.Equals(databaseName, StringComparison.CurrentCultureIgnoreCase)))
            {
                var createSql = CreateDataBaseSql;
                if (ContainsCharSet("utf8mb4")) 
                {
                    createSql = createSql.Replace("utf8 COLLATE utf8_general_ci", "utf8mb4");
                }
                if (!string.IsNullOrEmpty(StaticConfig.CodeFirst_MySqlCollate))
                {
                    if (createSql.Contains(" COLLATE "))
                    {
                        createSql = $" {Regex.Split(createSql, " COLLATE ").First()} COLLATE  {StaticConfig.CodeFirst_MySqlCollate} ";
                    }
                    else
                    {
                        createSql += $" COLLATE  {StaticConfig.CodeFirst_MySqlCollate} ";
                    }
                }
                newDb.Ado.ExecuteCommand(string.Format(createSql, databaseName, databaseDirectory));
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
            var oldColumns = this.Context.DbMaintenance.GetColumnInfosByTableName(entity.DbTableName, false);
            var db = this.Context;
            db.DbMaintenance.AddTableRemark(entity.DbTableName, entity.TableDescription);
            List<EntityColumnInfo> columns = entity.Columns.Where(it => it.IsIgnore == false).ToList();
            foreach (var item in columns)
            {
                if (item.ColumnDescription != null)
                {
                    var mySqlCodeFirst = this.Context.CodeFirst as MySqlCodeFirst;
                    if (item.UnderType == UtilConstants.GuidType && item.Length == 0)
                    {
                        item.Length = 36;
                    }
                    var columnInfo = oldColumns.FirstOrDefault(it => it.DbColumnName.EqualCase(item.DbColumnName));
                    if (columnInfo?.ColumnDescription.ObjToString() != item.ColumnDescription.ObjToString())
                    {
                        string sql = GetUpdateColumnSql(entity.DbTableName, mySqlCodeFirst.GetEntityColumnToDbColumn(entity, entity.DbTableName, item)) + " " + (item.IsIdentity ? "AUTO_INCREMENT" : "") + " " + " COMMENT '" + item.ColumnDescription + "'";
                        db.Ado.ExecuteCommand(sql);
                    }
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


        public override bool AddColumn(string tableName, DbColumnInfo columnInfo)
        {
            tableName = this.SqlBuilder.GetTranslationTableName(tableName);
            var isAddNotNUll = columnInfo.IsNullable == false && columnInfo.DefaultValue.HasValue();
            if (isAddNotNUll)
            {
                columnInfo = this.Context.Utilities.TranslateCopy(columnInfo);
                columnInfo.IsNullable = true;
            }
            string sql = GetAddColumnSql(tableName, columnInfo);
            if (sql != null && columnInfo.ColumnDescription.HasValue() &&!sql.ToLower().Contains("comment")) 
            {
                sql = $"{sql}{" COMMENT '"+ columnInfo.ColumnDescription.ToSqlFilter()+ "'  "}";
            }
            this.Context.Ado.ExecuteCommand(sql);
            if (isAddNotNUll)
            {
                var dtColums = this.Context.Queryable<object>().AS(columnInfo.TableName).Where("1=2")
                    .Select(this.SqlBuilder.GetTranslationColumnName(columnInfo.DbColumnName)).ToDataTable().Columns.Cast<System.Data.DataColumn>();
                var dtColumInfo = dtColums.First(it => it.ColumnName.EqualCase(columnInfo.DbColumnName));
                var type = UtilMethods.GetUnderType(dtColumInfo.DataType);
                var value = type == UtilConstants.StringType ? (object)"" : Activator.CreateInstance(type);
                if (this.Context.CurrentConnectionConfig.DbType == DbType.Oracle)
                {
                    value = columnInfo.DefaultValue;
                    if (value.Equals(""))
                    {
                        value = "empty";
                    }
                }
                var dt = new Dictionary<string, object>();
                dt.Add(columnInfo.DbColumnName, value);
                this.Context.Updateable(dt)
                             .AS(tableName)
                             .Where($"{columnInfo.DbColumnName} is null ").ExecuteCommand();
                columnInfo.IsNullable = false;
                UpdateColumn(tableName, columnInfo);
            }
            return true;
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
            var columns=GetColumnInfosByTableName(tableName,false).Where(it=>it.DbColumnName.Equals(oldColumnName,StringComparison.CurrentCultureIgnoreCase));
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
            if (defaultValue.ToLower().IsIn("null","now()", "current_timestamp") || defaultValue.ToLower().Contains("current_timestamp")|| defaultValue.Contains("()"))
            {
                defaultValue = "("+defaultValue + ")";
                string sql = string.Format(AddDefaultValueSql.Replace("'", ""), tableName, columnName, defaultValue);
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
            if (fullFileName == null)
            {
                fullFileName = $"c:\\{databaseName}.sql";
            }
            var db = this.Context.CopyNew();
            using (db.Ado.OpenAlways())
            {
                if (databaseName != null) 
                {
                    db.Ado.Connection.ChangeDatabase(databaseName);
                }
                // Load the MySqlBackup assembly
                Assembly assembly = null;

                try
                {
                    Assembly currentAssembly = Assembly.GetExecutingAssembly();
                    string exePath = currentAssembly.Location.Replace("SqlSugar.dll", "MySqlBackupNet.MySqlConnector.dll");
                    assembly = Assembly.LoadFrom(exePath);
                }
                catch (Exception)
                {
                    Check.ExceptionEasy("Need MySqlBackup.NET.MySqlConnector", "需要安装:MySqlBackup.NET.MySqlConnector");
                    throw;
                }

                // Get the MySqlBackup type
                Type mbType = assembly.GetType("MySqlConnector.MySqlBackup", false);
              
                // Create an instance of the MySqlBackup class
                object mb = Activator.CreateInstance(mbType, db.Ado.Connection.CreateCommand());

                // Get the ExportToFile method
                MethodInfo exportMethod = mbType.GetMethod("ExportToFile");

                // Invoke the ExportToFile method
                exportMethod.Invoke(mb, new object[] { fullFileName });
            }
            return true;
        }

        #endregion

        #region Helper
        private bool ContainsCharSet(string charset)
        {
            if (this.Context.CurrentConnectionConfig.ConnectionString.ObjToString().ToLower().Contains(charset))
            {
                return true;
            }
            else 
            {
                return false;
            }
        }
        #endregion
    }
}
