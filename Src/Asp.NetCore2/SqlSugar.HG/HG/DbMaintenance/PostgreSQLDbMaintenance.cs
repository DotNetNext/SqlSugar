using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar.HG
{
    public class HGDbMaintenance : DbMaintenanceProvider
    {
        #region DML
        protected override string GetDataBaseSql
        {
            get
            {
                return "SELECT datname FROM pg_database";
            }
        }
        protected override string GetColumnInfosByTableNameSql
        {
            get
            {
                string schema = GetSchema();
                string sql = @"select cast (pclass.oid as int4) as TableId,cast(ptables.tablename as varchar) as TableName,
                                pcolumn.column_name as DbColumnName,pcolumn.udt_name as DataType,
                                CASE WHEN pcolumn.numeric_scale >0 THEN pcolumn.numeric_precision ELSE pcolumn.character_maximum_length END   as Length,
                                pcolumn.column_default as DefaultValue,
                                pcolumn.numeric_scale as DecimalDigits,
                                pcolumn.numeric_scale as Scale,
                                col_description(pclass.oid, pcolumn.ordinal_position) as ColumnDescription,
                                case when pkey.colname = pcolumn.column_name
                                then true else false end as IsPrimaryKey,
                                case when pcolumn.column_default like 'nextval%'
                                then true else false end as IsIdentity,
                                case when pcolumn.is_nullable = 'YES'
                                then true else false end as IsNullable
                                 from (select * from pg_tables where upper(tablename) = upper('{0}') and schemaname='" + schema + @"') ptables inner join pg_class pclass
                                on ptables.tablename = pclass.relname inner join (SELECT *
                                FROM information_schema.columns
                                ) pcolumn on pcolumn.table_name = ptables.tablename
                                left join (
	                                select  pg_class.relname,pg_attribute.attname as colname from 
	                                pg_constraint  inner join pg_class 
	                                on pg_constraint.conrelid = pg_class.oid 
	                                inner join pg_attribute on pg_attribute.attrelid = pg_class.oid 
	                                and  pg_attribute.attnum = pg_constraint.conkey[1]
	                                inner join pg_type on pg_type.oid = pg_attribute.atttypid
	                                where pg_constraint.contype='p'
                                ) pkey on pcolumn.table_name = pkey.relname
                                order by table_catalog, table_schema, ordinal_position";
                return sql;
            }
        }

        protected override string GetTableInfoListSql
        {
            get
            {
                var schema = GetSchema();
                return @"select cast(relname as varchar) as Name,
                        cast(obj_description(c.oid,'pg_class') as varchar) as Description from pg_class c 
                         inner join 
						 pg_namespace n on n.oid = c.relnamespace and nspname='" + schema + @"'
                         inner join 
                         pg_tables z on z.tablename=c.relname
                        where  relkind in('p', 'r') and relname not like 'hg_%' and relname not like 'sql_%' and schemaname='" + schema + "' order by relname";
            }
        }
        protected override string GetViewInfoListSql
        {
            get
            {
                return @"select cast(relname as varchar) as Name,cast(Description as varchar) from pg_description
                         join pg_class on pg_description.objoid = pg_class.oid
                         where objsubid = 0 and relname in (SELECT viewname from pg_views  
                         WHERE schemaname ='"+GetSchema()+"')";
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
                return "select 1 from information_schema.columns limit 1 offset 0";
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
            if (databaseDirectory != null)
            {
                if (!FileHelper.IsExistDirectory(databaseDirectory))
                {
                    FileHelper.CreateDirectory(databaseDirectory);
                }
            }
            // var oldDatabaseName = this.Context.Ado.Connection.Database;
            //var connection = this.Context.CurrentConnectionConfig.ConnectionString;
            //connection = connection.Replace(oldDatabaseName, "");
            if (this.Context.Ado.IsValidConnection()) 
            {
                return true;
            }
            var newDb = this.Context.CopyNew();
            newDb.Ado.Connection.ChangeDatabase("highgo");
            newDb.Open();
            if (!GetDataBaseList(newDb).Any(it => it.Equals(databaseName, StringComparison.CurrentCultureIgnoreCase)))
            {
                newDb.Ado.ExecuteCommand(string.Format(CreateDataBaseSql, this.SqlBuilder.SqlTranslationLeft+databaseName+this.SqlBuilder.SqlTranslationRight, databaseDirectory));
            }
            newDb.Close();
            return true;
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
                if (item.DecimalDigits > 0&&item.Length>0 && dataType == "numeric") 
                {
                    dataSize = $"({item.Length},{item.DecimalDigits})";
                }
                string nullType = item.IsNullable ? this.CreateTableNull : CreateTableNotNull;
                string primaryKey = null;
                string addItem = string.Format(this.CreateTableColumn, this.SqlBuilder.GetTranslationColumnName(columnName.ToLower(isAutoToLowerCodeFirst)), dataType, dataSize, nullType, primaryKey, "");
                if (item.IsIdentity)
                {
                    string length = dataType.Substring(dataType.Length - 1);
                    string identityDataType = "serial" + length;
                    addItem = addItem.Replace(dataType, identityDataType);
                }
                columnArray.Add(addItem);
            }
            string tableString = string.Format(this.CreateTableSql, this.SqlBuilder.GetTranslationTableName(tableName.ToLower(isAutoToLowerCodeFirst)), string.Join(",\r\n", columnArray));
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
            var result= base.GetColumnInfosByTableName(tableName.TrimEnd('"').TrimStart('"').ToLower(), isCache);
            if (result == null || result.Count() == 0)
            {
                result = base.GetColumnInfosByTableName(tableName, isCache);
            }
            try
            {
                string sql = $@"select  
                               kcu.column_name as key_column
                               from information_schema.table_constraints tco
                               join information_schema.key_column_usage kcu 
                               on kcu.constraint_name = tco.constraint_name
                               and kcu.constraint_schema = tco.constraint_schema
                               and kcu.constraint_name = tco.constraint_name
                               where tco.constraint_type = 'PRIMARY KEY'
                               and kcu.table_schema='{GetSchema()}' and 
                               upper(kcu.table_name)=upper('{tableName.TrimEnd('"').TrimStart('"')}')";
                List<string> pkList = new List<string>();
                if (isCache)
                {
                    pkList=GetListOrCache<string>("GetColumnInfosByTableName_N_Pk"+tableName, sql);
                }
                else
                {
                    pkList = this.Context.Ado.SqlQuery<string>(sql);
                }
                if (pkList.Count >1) 
                {
                    foreach (var item in result)
                    {
                        if (pkList.Select(it=>it.ToUpper()).Contains(item.DbColumnName.ToUpper())) 
                        {
                            item.IsPrimarykey = true;
                        }
                    }
                }
            }
            catch  
            {

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

        #endregion
    }
}
