using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class SqlServerDbMaintenance : DbMaintenanceProvider
    {
        #region DML
        protected override string GetDataBaseSql
        {
            get
            {
                return "SELECT NAME FROM MASTER.DBO.SYSDATABASES ORDER BY NAME";
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
                           syscolumns.length AS [Length],
                           sys.extended_properties.[value] AS [ColumnDescription],
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
                         WHERE xtype IN('u',
                                        'v') )
                      AND (systypes.name <> 'sysname')
                      AND sysobjects.name='{0}'
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
                return @"SELECT s.Name,Convert(varchar(max),tbp.value) as Description
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
                return "EXECUTE sp_addextendedproperty N'MS_Description', '{2}', N'user', N'dbo', N'table', N'{1}', N'column', N'{0}'"; ;
            }
        }

        protected override string DeleteColumnRemarkSql
        {
            get
            {
                return "EXEC sp_dropextendedproperty 'MS_Description','user',dbo,'table','{1}','column',{0}";
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
                                " WHERE A.name = '{1}' and b.name = '{0}'";

            }
        }

        protected override string AddTableRemarkSql
        {
            get
            {
                return "EXECUTE sp_addextendedproperty N'MS_Description', '{1}', N'user', N'dbo', N'table', N'{0}', NULL, NULL";
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
            var oldDatabaseName = this.Context.Ado.Connection.Database;
            var connection = this.Context.CurrentConnectionConfig.ConnectionString;
            connection = connection.Replace(oldDatabaseName, "master");
            var newDb = new SqlSugarClient(new ConnectionConfig()
            {
                DbType = this.Context.CurrentConnectionConfig.DbType,
                IsAutoCloseConnection = true,
                ConnectionString = connection
            });
            if (!GetDataBaseList(newDb).Any(it => it.Equals(databaseName, StringComparison.CurrentCultureIgnoreCase)))
            {
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
                }
                newDb.Ado.ExecuteCommand(string.Format(sql, databaseName, databaseDirectory));
            }
            return true;
        }
        public override bool CreateTable(string tableName, List<DbColumnInfo> columns, bool isCreatePrimaryKey = true)
        {
            tableName = this.SqlBuilder.GetTranslationTableName(tableName);
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
