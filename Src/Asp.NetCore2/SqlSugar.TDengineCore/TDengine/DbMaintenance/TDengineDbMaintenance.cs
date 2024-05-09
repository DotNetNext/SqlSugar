using SqlSugar.TDengineAdo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SqlSugar.TDengine
{
    public class TDengineDbMaintenance : DbMaintenanceProvider
    {
        #region DML

        protected override string GetViewInfoListSql => throw new NotImplementedException();
        protected override string GetDataBaseSql
        {
            get
            {
                return "show databases";
            }
        }
        protected override string GetColumnInfosByTableNameSql
        {
            get
            {
                throw new NotSupportedException("TDengineCode暂时不支持DbFirst等方法,还在开发");
            }
        }

        protected override string GetTableInfoListSql
        {
            get
            {
                var dt = GetSTables();
                List<string> sb = new List<string>();
                foreach (DataRow item in dt.Rows)
                {
                    sb.Add($" SELECT '{item["stable_name"].ObjToString().ToSqlFilter()}' AS NAME ");
                }
                var dt2 = GetTables();
                foreach (DataRow item in dt2.Rows)
                {
                    sb.Add($" SELECT '{item["table_name"].ObjToString().ToSqlFilter()}' AS NAME ");
                }
                var result= string.Join(" UNION ALL ", sb);
                if (string.IsNullOrEmpty(result)) 
                {
                    result = " SELECT 'NoTables' AS Name ";
                }
                return result;
            }
        }

        #endregion

        #region DDL
        protected override string CreateDataBaseSql
        {
            get
            {
                return "CREATE DATABASE IF NOT EXISTS {0} WAL_RETENTION_PERIOD 3600";
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
                return "alter table {0} MODIFY COLUMN {1} {2}{3} {4} {5} {6}";
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
                return "CREATE STABLE IF NOT EXISTS  {0}(\r\n{1} ) TAGS("+SqlBuilder.GetTranslationColumnName("TagsTypeId") +" VARCHAR(20))";
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

        protected override string RenameTableSql => "alter table  {0} to {1}";

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
                return "  SELECT count(1) WHERE upper('{0}') IN ( SELECT upper(indexname) FROM pg_indexes )";
            }
        }
        protected override string IsAnyProcedureSql => throw new NotImplementedException();
        #endregion

        #region Check
        protected override string CheckSystemTablePermissionsSql
        {
            get
            {
                return "SHOW DATABASES";
            }
        }
        #endregion

        #region Scattered
        protected override string CreateTableNull
        {
            get
            {
                return " ";
            }
        }
        protected override string CreateTableNotNull
        {
            get
            {
                return " ";
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
        public override bool AddColumn(string tableName, DbColumnInfo columnInfo)
        {
            if (columnInfo.DbColumnName == "TagsTypeId") 
            {
                return true;
            }
            tableName = this.SqlBuilder.GetTranslationTableName(tableName);
            var isAddNotNUll = columnInfo.IsNullable == false && columnInfo.DefaultValue.HasValue();
            if (isAddNotNUll)
            {
                columnInfo = this.Context.Utilities.TranslateCopy(columnInfo);
                columnInfo.IsNullable = true;
            }
            string sql = GetAddColumnSql(tableName, columnInfo);
            this.Context.Ado.ExecuteCommand(sql); 
            return true;
        }
        public override List<DbTableInfo> GetViewInfoList(bool isCache = true)
        {
            return new List<DbTableInfo>();
        }
        public override bool CreateDatabase(string databaseName,string databaseDirectory = null)
        {
            var db=this.Context.CopyNew(); 
            db.Ado.Connection.ChangeDatabase("");
            var sql = CreateDataBaseSql;
            if (this.Context.CurrentConnectionConfig.ConnectionString.ToLower().Contains("config_us")) 
            {
                sql += " PRECISION 'us'";
            }
            else if (this.Context.CurrentConnectionConfig.ConnectionString.ToLower().Contains("config_ns"))
            {
                sql += " PRECISION 'ns'";
            }
            db.Ado.ExecuteCommand(string.Format(sql, databaseName));
            return true;
        }
        public override List<string> GetIndexList(string tableName)
        {
            var sql = $"SELECT indexname, indexdef FROM pg_indexes WHERE upper(tablename) = upper('{tableName}')";
            return this.Context.Ado.SqlQuery<string>(sql);
        }
        public override List<string> GetProcList(string dbName)
        {
            var sql = $"SELECT proname FROM pg_proc p JOIN pg_namespace n ON p.pronamespace = n.oid WHERE n.nspname = '{dbName}'";
            return this.Context.Ado.SqlQuery<string>(sql);
        }
        public override bool AddDefaultValue(string tableName, string columnName, string defaultValue)
        {
            return base.AddDefaultValue(this.SqlBuilder.GetTranslationTableName(tableName), this.SqlBuilder.GetTranslationTableName(columnName), defaultValue);
        }
        public override bool AddColumnRemark(string columnName, string tableName, string description)
        { 
            tableName = this.SqlBuilder.GetTranslationTableName(tableName);
            string sql = string.Format(this.AddColumnRemarkSql, this.SqlBuilder.GetTranslationColumnName(columnName.ToLower(isAutoToLowerCodeFirst)), tableName, description);
            this.Context.Ado.ExecuteCommand(sql);
            return true;
        }
        public override bool AddTableRemark(string tableName, string description)
        {
            tableName = this.SqlBuilder.GetTranslationTableName(tableName);
            return base.AddTableRemark(tableName, description);
        }
        public override bool UpdateColumn(string tableName, DbColumnInfo columnInfo)
        {
            tableName = this.SqlBuilder.GetTranslationTableName(tableName);
            var columnName= this.SqlBuilder.GetTranslationColumnName(columnInfo.DbColumnName);
            string sql = GetUpdateColumnSql(tableName, columnInfo);
            this.Context.Ado.ExecuteCommand(sql);
            var isnull = columnInfo.IsNullable?" DROP NOT NULL ": " SET NOT NULL ";
            this.Context.Ado.ExecuteCommand(string.Format("alter table {0} alter {1} {2}",tableName,columnName, isnull));
            return true;
        }

        protected override string GetUpdateColumnSql(string tableName, DbColumnInfo columnInfo)
        {
            string columnName = this.SqlBuilder.GetTranslationColumnName(columnInfo.DbColumnName);
            tableName = this.SqlBuilder.GetTranslationTableName(tableName);
            string dataSize = GetSize(columnInfo);
            string dataType = columnInfo.DataType;
            //if (!string.IsNullOrEmpty(dataType))
            //{
            //    dataType = " type " + dataType;
            //}
            string nullType = "";
            string primaryKey = null;
            string identity = null;
            string result = string.Format(this.AlterColumnToTableSql, tableName, columnName, dataType, dataSize, nullType, primaryKey, identity);
            return result;
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
                primaryKeyInfo = string.Format(", Primary key({0})", string.Join(",", columns.Where(it => it.IsPrimarykey).Select(it => this.SqlBuilder.GetTranslationColumnName(it.DbColumnName.ToLower(isAutoToLowerCodeFirst)))));

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
                //if (dataType == "uuid")
                //{
                //    item.Length = 50;
                //    dataType = "varchar";
                //}
                string dataSize = item.Length > 0 ? string.Format("({0})", item.Length) : null;
                //if (item.DecimalDigits > 0&&item.Length>0 && dataType?.ToLower()== "float") 
                //{
                //    item.Length = 0;
                //    dataSize = $"({item.Length},{item.DecimalDigits})";
                //}
                //if (item.DecimalDigits > 0 && item.Length > 0 && dataType?.ToLower() ==  "double")
                //{
               
                //    dataSize = $"({item.Length},{item.DecimalDigits})";
                //}
                //if (item.DecimalDigits > 0 && item.Length > 0 && dataType?.ToLower() == "decimal")
                //{ 
                //    dataSize = $"({item.Length},{item.DecimalDigits})";
                //}
                //if (item.DecimalDigits == 0 && item.Length == 0 && dataType?.ToLower() == "float") 
                //{
                //   dataType = $"FLOAT(18,4)";
                //}
                //if (item.DecimalDigits == 0 && item.Length == 0 && dataType?.ToLower() == "double")
                //{
                //    dataType = $"DOUBLE(18,4)";
                //}
                if (item.Length==0&&dataType?.ToLower()?.IsIn("nchar", "varchar") ==true)
                {
                    dataType = "VARCHAR(200)";
                }
                if (dataType?.ToLower()?.IsIn("float", "double") == true)
                {
                    dataSize = null;
                }
                string primaryKey = null;
                string addItem = string.Format(this.CreateTableColumn, this.SqlBuilder.GetTranslationColumnName(columnName.ToLower(isAutoToLowerCodeFirst)), dataType, dataSize, null, primaryKey, "");
                columnArray.Add(addItem);
            }
            string tableString = string.Format(this.CreateTableSql, this.SqlBuilder.GetTranslationTableName("STable_"+tableName.ToLower(isAutoToLowerCodeFirst)), string.Join(",\r\n", columnArray));
            var childTableName = this.SqlBuilder.GetTranslationTableName(tableName.ToLower(isAutoToLowerCodeFirst));
            var stableName =  this.SqlBuilder.GetTranslationTableName("STable_"+tableName.ToLower(isAutoToLowerCodeFirst));
            var isAttr = tableName.Contains("{stable}");
            if (isAttr) 
            {
                var attr = this.Context.Utilities.DeserializeObject<STableAttribute>(tableName.Split("{stable}").Last());
                stableName= this.SqlBuilder.GetTranslationTableName(attr.STableName.ToLower(isAutoToLowerCodeFirst));
                tableString = string.Format(this.CreateTableSql, stableName, string.Join(",\r\n", columnArray));
                tableName=childTableName = this.SqlBuilder.GetTranslationTableName(tableName.Split("{stable}").First().ToLower(isAutoToLowerCodeFirst));
                STable.Tags =this.Context.Utilities.DeserializeObject<List<ColumnTagInfo>>( attr.Tags);
            }
            if (STable.Tags?.Any() == true) 
            {
                var colums = STable.Tags.Select(it => this.SqlBuilder.GetTranslationTableName(it.Name)+ "  VARCHAR(20) ");
                tableString=tableString.Replace(SqlBuilder.GetTranslationColumnName("TagsTypeId"), string.Join(",", colums));
                tableString = tableString.Replace(" VARCHAR(20)  VARCHAR(20)", " VARCHAR(20)");
            }
            this.Context.Ado.ExecuteCommand(tableString);
            var createChildSql = $"CREATE TABLE IF NOT EXISTS     {childTableName} USING {stableName} TAGS('default')";
            if (STable.Tags?.Any() == true)
            {
                var colums = STable.Tags.Select(it => it.Value.ToSqlValue());
                createChildSql = createChildSql.Replace("TAGS('default')", $"TAGS({string.Join(",", colums)})"); 
            }
            this.Context.Ado.ExecuteCommand(createChildSql);
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

            var sql = $"select * from {this.SqlBuilder.GetTranslationColumnName( tableName)} where 1=2 ";
            List<DbColumnInfo> result = new List<DbColumnInfo>();
            DataTable dt = null;
            try
            {
                dt=this.Context.Ado.GetDataTable(sql);
            }
            catch (Exception)
            {
                sql = $"select * from `{tableName}` where 1=2 ";
                dt = this.Context.Ado.GetDataTable(sql);
            }
            foreach (DataColumn item in dt.Columns)
            {
                var addItem = new DbColumnInfo()
                {
                     DbColumnName=item.ColumnName,
                     DataType=item.DataType.Name
                };
                result.Add(addItem);
            }
            if (result.Count(it => it.DataType == "DateTime") == 1) 
            {
                result.First(it => it.DataType == "DateTime").IsPrimarykey = true;
            }
            return result;
        }
        #endregion

        #region Helper
        private bool isAutoToLowerCodeFirst
        {
            get
            {
                if (this.Context.CurrentConnectionConfig.MoreSettings == null) return true;
                else if (
                    this.Context.CurrentConnectionConfig.MoreSettings.PgSqlIsAutoToLower == false &&
                    this.Context.CurrentConnectionConfig.MoreSettings?.PgSqlIsAutoToLowerCodeFirst == false)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
         
        private string GetSchema()
        {
            var schema = "public";
            if (System.Text.RegularExpressions.Regex.IsMatch(this.Context.CurrentConnectionConfig.ConnectionString.ToLower(), "searchpath="))
            {
                var regValue = System.Text.RegularExpressions.Regex.Match(this.Context.CurrentConnectionConfig.ConnectionString.ToLower(), @"searchpath\=(\w+)").Groups[1].Value;
                if (regValue.HasValue())
                {
                    schema = regValue;
                }
            }
            else if (System.Text.RegularExpressions.Regex.IsMatch(this.Context.CurrentConnectionConfig.ConnectionString.ToLower(), "search path="))
            {
                var regValue = System.Text.RegularExpressions.Regex.Match(this.Context.CurrentConnectionConfig.ConnectionString.ToLower(), @"search path\=(\w+)").Groups[1].Value;
                if (regValue.HasValue())
                {
                    schema = regValue;
                }
            }

            return schema;
        }

        private DataTable GetTables()
        {
            return this.Context.Ado.GetDataTable("SHOW TABLES");
        }

        private DataTable GetSTables()
        {
            return this.Context.Ado.GetDataTable("SHOW STABLES");
        }

        #endregion
    }
}
