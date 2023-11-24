using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlSugar
{
    public class SqlServerDbMaintenance : DbMaintenanceProvider
    {
        #region DML
        protected override string GetDataBaseSql
        {
            get
            {
                return "SELECT NAME FROM master.dbo.sysdatabases ORDER BY NAME";
            }
        }
        protected override string GetColumnInfosByTableNameSql
        {
            get
            {
                string sql = @"SELECT sysobjects.name AS TableName,
                           syscolumns.Id AS TableId,
                           syscolumns.name AS DbColumnName,
                           systypes.name AS DataType,
                           COLUMNPROPERTY(syscolumns.id,syscolumns.name,'PRECISION') as [length],
                           isnull(COLUMNPROPERTY(syscolumns.id,syscolumns.name,'Scale'),0) as Scale, 
						   isnull(COLUMNPROPERTY(syscolumns.id,syscolumns.name,'Scale'),0) as DecimalDigits,
                           Cast( sys.extended_properties.[value] as nvarchar(2000)) AS [ColumnDescription],
                           syscomments.text AS DefaultValue,
                           syscolumns.isnullable AS IsNullable,
	                       columnproperty(syscolumns.id,syscolumns.name,'IsIdentity')as IsIdentity,
                           (CASE
                                WHEN EXISTS
                                       ( 
                                             	select 1
												from sysindexes i
												join sysindexkeys k on i.id = k.id and i.indid = k.indid
												join sysobjects o on i.id = o.id
												join syscolumns c on i.id=c.id and k.colid = c.colid
												where o.xtype = 'U' 
												and exists(select 1 from sysobjects where xtype = 'PK' and name = i.name) 
												and o.name=sysobjects.name and c.name=syscolumns.name
                                       ) THEN 1
                                ELSE 0
                            END) AS IsPrimaryKey
                    FROM syscolumns
                    INNER JOIN systypes ON syscolumns.xtype = systypes.xtype
                    LEFT JOIN sysobjects ON syscolumns.id = sysobjects.id
                    LEFT OUTER JOIN sys.extended_properties ON (sys.extended_properties.minor_id = syscolumns.colid
                                                                AND sys.extended_properties.major_id = syscolumns.id)
                    LEFT OUTER JOIN syscomments ON syscolumns.cdefault = syscomments.id
                    WHERE syscolumns.id IN
                        (SELECT id
                         FROM sysobjects
                         WHERE upper(xtype) IN('U',
                                        'V') )
                      AND (systypes.name <> 'sysname')
                      AND sysobjects.name=N'{0}'
                      AND systypes.name<>'geometry'
                      AND systypes.name<>'geography'
                    ORDER BY syscolumns.colid";
                return sql;
            }
        }
        protected override string GetTableInfoListSql
        {
            get
            {
                return @"SELECT s.Name,Convert(nvarchar(max),tbp.value) as Description
                            FROM sysobjects s
					     	LEFT JOIN sys.extended_properties as tbp ON s.id=tbp.major_id and tbp.minor_id=0 AND (tbp.Name='MS_Description' OR tbp.Name is null)  WHERE s.xtype IN('U') ";
            }
        }
        protected override string GetViewInfoListSql
        {
            get
            {
                return @"SELECT s.Name,Convert(varchar(max),tbp.value) as Description
                            FROM sysobjects s
					     	LEFT JOIN sys.extended_properties as tbp ON s.id=tbp.major_id and tbp.minor_id=0  AND (tbp.Name='MS_Description' OR tbp.Name is null) WHERE s.xtype IN('V')  ";
            }
        }
        #endregion

        #region DDL
        protected override string CreateDataBaseSql
        {
            get
            {
                return @"create database {0}  ";
            }
        }
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
                return "SELECT TOP {0} * INTO {1} FROM  {2}";
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
        protected override string AddColumnRemarkSql
        {
            get
            {
                return "EXECUTE sp_addextendedproperty N'MS_Description', N'{2}', N'user', N'dbo', N'table', N'{1}', N'column', N'{0}'"; ;
            }
        }

        protected override string DeleteColumnRemarkSql
        {
            get
            {
                return "EXEC sp_dropextendedproperty 'MS_Description','user',dbo,'table','{1}','column','{0}'";
            }

        }

        protected override string IsAnyColumnRemarkSql
        {
            get
            {
                return @"SELECT" +
                                " A.name AS table_name," +
                                " B.name AS column_name," +
                                " C.value AS column_description" +
                                " FROM sys.tables A" +
                                " LEFT JOIN sys.extended_properties C ON C.major_id = A.object_id" +
                                " LEFT JOIN sys.columns B ON B.object_id = A.object_id AND C.minor_id = B.column_id" +
                                " INNER JOIN sys.schemas SC ON SC.schema_id = A.schema_id AND SC.name = 'dbo'" +
                                " WHERE A.name = '{1}' and B.name = '{0}'";

            }
        }

        protected override string AddTableRemarkSql
        {
            get
            {
                return "EXECUTE sp_addextendedproperty N'MS_Description', N'{1}', N'user', N'dbo', N'table', N'{0}', NULL, NULL";
            }
        }

        protected override string DeleteTableRemarkSql
        {
            get
            {
                return "EXEC sp_dropextendedproperty 'MS_Description','user',dbo,'table','{0}' ";
            }

        }

        protected override string IsAnyTableRemarkSql
        {
            get
            {
                return @"SELECT C.class_desc
                                FROM sys.tables A 
                                LEFT JOIN sys.extended_properties C ON C.major_id = A.object_id 
								INNER JOIN sys.schemas SC ON  SC.schema_id=A.schema_id AND SC.name='dbo'
                                WHERE A.name = '{0}'  AND minor_id=0";
            }

        }

        protected override string RenameTableSql
        {
            get
            {
                return "EXEC sp_rename '{0}','{1}'";
            }
        }

        protected override string CreateIndexSql
        {
            get
            {
                return "CREATE {3} NONCLUSTERED INDEX Index_{0}_{2} ON {0}({1})";
            }
        }
        protected override string AddDefaultValueSql
        {
            get
            {
                return "alter table {0} ADD DEFAULT '{2}' FOR {1}";
            }
        }
        protected override string IsAnyIndexSql
        {
            get
            {
                return "select count(*) from sys.indexes where name='{0}'";
            }
        }
        protected override string IsAnyProcedureSql
        {
            get
            {
                return "select count(*) from sys.objects where [object_id] = OBJECT_ID(N'sp_GetSubLedgerJoinWithdrawalApplicationRecords') and [type] in (N'P')";
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
        public override List<DbTableInfo> GetSchemaTables(EntityInfo entityInfo)
        {
            if (entityInfo.DbTableName.Contains(".") && this.Context.CurrentConnectionConfig.DbType == DbType.SqlServer)
            {
                var schema = entityInfo.DbTableName.Split('.').First();
                var isAny = GetSchemas().Any(it => it.EqualCase(schema))||schema.EqualCase("dbo");
                if (isAny)
                {
                    var tableInfos = this.Context.Ado.SqlQuery<DbTableInfo>(@"SELECT schem.name+'.'+tb.name Name,tb.Description from 
                                ( SELECT obj.name,Convert(nvarchar(max),prop.value)as Description,obj.schema_id FROM sys.objects  obj
                                    LEFT JOIN sys.extended_properties  prop 
                                    ON obj.object_id=prop.major_id
                                        and prop.minor_id=0
                                        AND (prop.Name='MS_Description' OR prop.Name is null)
                                        WHERE obj.type IN('U')) tb
                                            inner join	sys.schemas as schem
                                            on tb.schema_id=schem.schema_id ");
                    return tableInfos;
                }
            }
            return null;
        }

        public override bool DropColumn(string tableName, string columnName)
        {
            if (Regex.IsMatch(tableName, @"^\w+$") && Regex.IsMatch(columnName, @"^\w+$"))
            {
                var sql = $"SELECT distinct dc.name AS ConstraintName \r\nFROM sys.default_constraints dc\r\nJOIN sys.columns c ON dc.parent_column_id = c.column_id\r\nWHERE dc.parent_object_id = OBJECT_ID('{tableName}')\r\nAND c.name = '{columnName}';";
                var checks = this.Context.Ado.SqlQuery<string>(sql);
                foreach (var checkName in checks)
                {
                    if (checkName?.ToUpper()?.StartsWith("DF__") == true)
                    {
                        this.Context.Ado.ExecuteCommand($"ALTER TABLE {SqlBuilder.GetTranslationColumnName(tableName)} DROP CONSTRAINT {checkName}");
                    }
                }
            }
            return base.DropColumn(tableName, columnName);
        }
        public override List<string> GetDbTypes() 
        {
            return this.Context.Ado.SqlQuery<string>(@"SELECT name
FROM sys.types
WHERE is_user_defined = 0;");
        }
        public override List<string> GetTriggerNames(string tableName)
        {
            return this.Context.Ado.SqlQuery<string>(@"SELECT DISTINCT sysobjects.name AS TriggerName
FROM sysobjects
JOIN syscomments ON sysobjects.id = syscomments.id
WHERE sysobjects.xtype = 'TR'
AND syscomments.text LIKE '%"+tableName+"%'");
        }
        public override List<string> GetFuncList()
        {
            return this.Context.Ado.SqlQuery<string>("SELECT name\r\nFROM sys.objects\r\nWHERE type_desc = 'SQL_SCALAR_FUNCTION' ");
        }
        private bool IsAnySchemaTable(string tableName)
        {
            if (tableName == null||!tableName.Contains(".") )
            {
                return false;
            }
            var list = GetSchemas() ?? new List<string>();
            list.Add("dbo");
            var isAnySchemas = list.Any(it => it.EqualCase(tableName?.Split('.').FirstOrDefault()));
            return isAnySchemas;
        }
        public override bool IsAnyColumnRemark(string columnName, string tableName)
        {
            if (tableName!=null&&tableName.Contains(".") && tableName.Contains(SqlBuilder.SqlTranslationLeft)) 
            {
                tableName =string.Join(".", tableName.Split('.').Select(it => SqlBuilder.GetNoTranslationColumnName(it)));
            }
            if (IsAnySchemaTable(tableName))
            {
                var schema = tableName.Split('.').First();
                tableName = tableName.Split('.').Last();
                var temp = this.IsAnyColumnRemarkSql.Replace("'dbo'", $"'{schema}'");
                string sql = string.Format(temp, columnName, tableName);
                var dt = this.Context.Ado.GetDataTable(sql);
                return dt.Rows != null && dt.Rows.Count > 0;
            }
            return base.IsAnyColumnRemark(columnName, tableName);
        }
        public override bool DeleteColumnRemark(string columnName, string tableName)
        {
            if (tableName != null&&tableName.Contains(".") && tableName.Contains(SqlBuilder.SqlTranslationLeft))
            {
                tableName = string.Join(".", tableName.Split('.').Select(it => SqlBuilder.GetNoTranslationColumnName(it)));
            }
            if (IsAnySchemaTable(tableName))
            {
                var schema = tableName.Split('.').First();
                tableName = tableName.Split('.').Last();
                var temp = this.DeleteColumnRemarkSql.Replace(",dbo,", $",{schema},");
                if (!schema.EqualCase("dbo"))
                {
                    temp = temp.Replace("N'user'", $"N'schema'");
                }
                string sql = string.Format(temp, columnName, tableName);
                this.Context.Ado.ExecuteCommand(sql);
                return true;
            }
            return base.DeleteColumnRemark(columnName, tableName);
        }
        public override bool AddColumnRemark(string columnName, string tableName, string description)
        {
            if (tableName != null&&tableName.Contains(".") && tableName.Contains(SqlBuilder.SqlTranslationLeft))
            {
                tableName = string.Join(".", tableName.Split('.').Select(it => SqlBuilder.GetNoTranslationColumnName(it)));
            }
            if (IsAnySchemaTable(tableName))
            {
                var schema = tableName.Split('.').First();
                tableName = tableName.Split('.').Last();
                var temp = this.AddColumnRemarkSql.Replace("N'dbo'", $"N'{schema}'");
                if (!schema.EqualCase("dbo")) 
                {
                    temp= temp.Replace("N'user'", $"N'schema'");
                }
                string sql = string.Format(temp, columnName, tableName, description);
                this.Context.Ado.ExecuteCommand(sql);
                return true;
            }
            return base.AddColumnRemark(columnName, tableName, description);
        }

        public override void AddDefaultValue(EntityInfo entityInfo)
        {
            var dbColumns = this.GetColumnInfosByTableName(entityInfo.DbTableName, false);
            var db = this.Context;
            var columns = entityInfo.Columns.Where(it => it.IsIgnore == false).ToList();
            foreach (var item in columns)
            {
                if (item.DefaultValue.HasValue() || (item.DefaultValue == "" && item.UnderType == UtilConstants.StringType))
                {
                    if (!IsAnyDefaultValue(entityInfo.DbTableName, item.DbColumnName, dbColumns))
                    {
                        this.AddDefaultValue(entityInfo.DbTableName, item.DbColumnName, item.DefaultValue);
                    }
                }
            }
        }

        public override List<string> GetIndexList(string tableName)
        {
           return this.Context.Ado.SqlQuery<string>($"SELECT indexname = i.name FROM sys.indexes i\r\nJOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id\r\nJOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id\r\nWHERE i.object_id = OBJECT_ID('{tableName}')");
        }
        public override List<string> GetProcList(string dbName)
        {
            var sql = $"SELECT name FROM {dbName}.sys.procedures";
            return this.Context.Ado.SqlQuery<string>(sql);
        }
        public override bool UpdateColumn(string tableName, DbColumnInfo column)
        {
            if (column.DataType != null && this.Context.CurrentConnectionConfig?.MoreSettings?.SqlServerCodeFirstNvarchar == true)
            {
                if (!column.DataType.ToLower().Contains("nvarchar"))
                {
                    column.DataType = column.DataType.ToLower().Replace("varchar", "nvarchar");
                }
            }
            return base.UpdateColumn(tableName, column);
        }
        public override bool IsAnyTable(string tableName, bool isCache = true)
        {
            if (tableName.Contains("."))
            {
                var schemas = GetSchemas();
                var first =this.SqlBuilder.GetNoTranslationColumnName(tableName.Split('.').First());
                var schemaInfo= schemas.FirstOrDefault(it => it.EqualCase(first));
                if (schemaInfo == null)
                {
                    return base.IsAnyTable(tableName, isCache);
                }
                else
                {
                    var result= this.Context.Ado.GetInt($"select object_id(N'{tableName}')");
                    return result > 0;
                }
            }
            else if (isCache)
            {
                return base.IsAnyTable(tableName, isCache);
            }
            else 
            {
                if (tableName.Contains(SqlBuilder.SqlTranslationLeft)) 
                {
                    tableName = SqlBuilder.GetNoTranslationColumnName(tableName);
                }
                var sql = @"IF EXISTS (SELECT * FROM sys.objects
                        WHERE type='u' AND name=N'"+tableName.ToSqlFilter()+@"')  
                        SELECT 1 AS res ELSE SELECT 0 AS res;";
                return this.Context.Ado.GetInt(sql) > 0;
            }
        }
        public List<string> GetSchemas()
        {
            return this.Context.Ado.SqlQuery<string>("SELECT name FROM  sys.schemas where name <> 'dbo'");
        }
        public override bool DeleteTableRemark(string tableName)
        {
            string sql = string.Format(this.DeleteTableRemarkSql, tableName);
            if (tableName.Contains("."))
            {
                var schemas = GetSchemas();
                var tableSchemas = this.SqlBuilder.GetNoTranslationColumnName(tableName.Split('.').First());
                if (schemas.Any(y => y.EqualCase(tableSchemas)))
                {
                    sql = string.Format(this.DeleteTableRemarkSql, this.SqlBuilder.GetNoTranslationColumnName(tableName.Split('.').Last()));
                    if (tableSchemas.EqualCase("user"))
                    {
                        sql = sql.Replace("'user'", "'SCHEMA'").Replace("dbo", $"'{tableSchemas}'");
                    }
                    else
                    {
                        sql = sql.Replace(",dbo,", $",{tableSchemas},").Replace("'user'", "'SCHEMA'");
                    }
                }
            }
            this.Context.Ado.ExecuteCommand(sql);
            return true;
        }
        public override bool IsAnyTableRemark(string tableName)
        {
            string sql = string.Format(this.IsAnyTableRemarkSql, tableName);
            if (tableName.Contains("."))
            {
                var schemas = GetSchemas();
                var tableSchemas = this.SqlBuilder.GetNoTranslationColumnName(tableName.Split('.').First());
                if (schemas.Any(y => y.EqualCase(tableSchemas)))
                {
                    sql = string.Format(this.IsAnyTableRemarkSql, this.SqlBuilder.GetNoTranslationColumnName(tableName.Split('.').Last()));
                    sql = sql.Replace("'dbo'", $"'{tableSchemas}'");
                }
            }
            var dt = this.Context.Ado.GetDataTable(sql);
            return dt.Rows != null && dt.Rows.Count > 0;
        }
        public override bool AddTableRemark(string tableName, string description)
        {
            string sql = string.Format(this.AddTableRemarkSql, tableName, description);
            if (tableName.Contains(".")) 
            {
                var schemas = GetSchemas();
                var tableSchemas =this.SqlBuilder.GetNoTranslationColumnName(tableName.Split('.').First());
                if (schemas.Any(y => y.EqualCase(tableSchemas))) 
                {
                    sql = string.Format(this.AddTableRemarkSql, this.SqlBuilder.GetNoTranslationColumnName(tableName.Split('.').Last()), description);
                    if (tableSchemas.EqualCase("user"))
                    {
                        sql = sql.Replace("N'user', N'dbo'", $"N'user', '{tableSchemas}'").Replace("N'user'", "N'SCHEMA'");
                    }
                    else
                    {
                        sql = sql.Replace("N'dbo'", $"N'{tableSchemas}'").Replace("N'user'", "N'SCHEMA'");
                    }
                }
            }
            this.Context.Ado.ExecuteCommand(sql);
            return true;
        }
        public override bool AddDefaultValue(string tableName, string columnName, string defaultValue)
        {
            if (defaultValue == "''")
            {
                defaultValue = "";
            }
            var template = AddDefaultValueSql;
            if (defaultValue != null && defaultValue.Replace(" ","").Contains("()")) 
            {
                template = template.Replace("'{2}'", "{2}");
            }
            tableName=SqlBuilder.GetTranslationTableName(tableName);
            columnName = SqlBuilder.GetTranslationTableName(columnName);
            string sql = string.Format(template, tableName, columnName, defaultValue);
            this.Context.Ado.ExecuteCommand(sql);
            return true;
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
                try
                {
                    if (!FileHelper.IsExistDirectory(databaseDirectory))
                    {
                        FileHelper.CreateDirectory(databaseDirectory);
                    }
                }
                catch  
                {
                    //Databases and sites are not in the same service
                }
            }
            var oldDatabaseName = this.Context.Ado.Connection.Database;
            var connection = this.Context.CurrentConnectionConfig.ConnectionString;
            if (Regex.Split(connection, oldDatabaseName).Length > 2)
            {
                var name=Regex.Match(connection, @"database\=\w+|datasource\=\w+",RegexOptions.IgnoreCase).Value;
                if (!string.IsNullOrEmpty(name))
                {
                    connection = connection.Replace(name, "database=master");
                }
                else 
                {
                    Check.ExceptionEasy("Failed to create the database. The database name has a keyword. Please change the name", "建库失败，库名存在关键字，请换一个名字");
                }
            }
            else
            {
                connection = connection.Replace(oldDatabaseName, "master");
            }
            var newDb = new SqlSugarClient(new ConnectionConfig()
            {
                DbType = this.Context.CurrentConnectionConfig.DbType,
                IsAutoCloseConnection = true,
                ConnectionString = connection
            });
            if (!GetDataBaseList(newDb).Any(it => it.Equals(databaseName, StringComparison.CurrentCultureIgnoreCase)))
            {
                var separatorChar = UtilMethods.GetSeparatorChar();
                var sql = CreateDataBaseSql;
                if (databaseDirectory.HasValue())
                {
                    sql += @"on primary 
                                        (
                                            name = N'{0}',
                                            filename = N'{1}\{0}.mdf',
                                            size = 10mb,
                                            maxsize = 100mb,
                                            filegrowth = 1mb
                                        ),
                                        (
                                            name = N'{0}_ndf',   
                                            filename = N'{1}\{0}.ndf',
                                            size = 10mb,
                                            maxsize = 100mb,
                                             filegrowth = 10 %
                                        )
                                        log on  
                                        (
                                            name = N'{0}_log',
                                            filename = N'{1}\{0}.ldf',
                                            size = 100mb,
                                            maxsize = 1gb,
                                            filegrowth = 10mb
                                        ); ";
                    databaseDirectory = databaseDirectory.Replace("\\", separatorChar);
                }
                if (databaseName.Contains(".")) 
                {
                    databaseName = $"[{databaseName}]";
                }
                else if (Regex.IsMatch(databaseName,@"^\d.*"))
                {
                    databaseName = $"[{databaseName}]";
                }
                newDb.Ado.ExecuteCommand(string.Format(sql, databaseName, databaseDirectory));
            }
            return true;
        }
        public override bool CreateTable(string tableName, List<DbColumnInfo> columns, bool isCreatePrimaryKey = true)
        {
            tableName = this.SqlBuilder.GetTranslationTableName(tableName);
            foreach (var item in columns)
            {
                if (item.DataType == "decimal" && item.DecimalDigits == 0 && item.Length == 0)
                {
                    item.DecimalDigits = 4;
                    item.Length = 18;
                }
                else if (item.DataType != null && this.Context.CurrentConnectionConfig?.MoreSettings?.SqlServerCodeFirstNvarchar == true)
                {
                    if (!item.DataType.ToLower().Contains("nvarchar"))
                    {
                        item.DataType = item.DataType.ToLower().Replace("varchar", "nvarchar");
                    }
                }

            }
            string sql = GetCreateTableSql(tableName, columns);
            this.Context.Ado.ExecuteCommand(sql);
            if (isCreatePrimaryKey)
            {
                var pkColumns = columns.Where(it => it.IsPrimarykey).ToList();
                if (pkColumns.Count > 1)
                {
                    this.Context.DbMaintenance.AddPrimaryKeys(tableName, pkColumns.Select(it => it.DbColumnName).ToArray());
                }
                else
                {
                    foreach (var item in pkColumns)
                    {
                        this.Context.DbMaintenance.AddPrimaryKey(tableName, item.DbColumnName);
                    }
                }
            }
            return true;
        }
        public override List<DbColumnInfo> GetColumnInfosByTableName(string tableName, bool isCache = true)
        {
            tableName = SqlBuilder.GetNoTranslationColumnName(tableName);
            var result= base.GetColumnInfosByTableName(tableName, isCache);
            return result;
        }
        public override bool RenameColumn(string tableName, string oldColumnName, string newColumnName)
        {
            tableName = this.SqlBuilder.GetTranslationTableName(tableName);
            oldColumnName = this.SqlBuilder.GetTranslationColumnName(oldColumnName);
            newColumnName = this.SqlBuilder.GetNoTranslationColumnName(newColumnName);
            string sql = string.Format(this.RenameColumnSql, tableName, oldColumnName, newColumnName);
            this.Context.Ado.ExecuteCommand(sql);
            return true;
        }
        #endregion
    }
}
