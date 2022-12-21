using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Data.Odbc;
namespace SqlSugar.Odbc
{
    public class OdbcDbMaintenance : DbMaintenanceProvider
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
                string sql = @"SELECT sysobjects.name AS TableName,
                           syscolumns.Id AS TableId,
                           syscolumns.name AS DbColumnName,
                           systypes.name AS DataType,
                           COLUMNPROPERTY(syscolumns.id,syscolumns.name,'PRECISION') as [length],
                           isnull(COLUMNPROPERTY(syscolumns.id,syscolumns.name,'Scale'),0) as Scale, 
						   isnull(COLUMNPROPERTY(syscolumns.id,syscolumns.name,'Scale'),0) as DecimalDigits,
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
                         WHERE upper(xtype) IN('U',
                                        'V') )
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
                return @"select
trim(a.tabname) as name,
trim(b.comments) as Description 
from systables a
left join syscomments b on b.tabname = a.tabname
where a.tabtype in ('T')  and not (a.tabname like 'sys%') AND a.tabname <>'dual' ";
            }
        }
        protected override string GetViewInfoListSql
        {
            get
            {
                return @"select
trim(a.tabname) as name,
trim(b.comments) as Description 
from systables a
left join syscomments b on b.tabname = a.tabname
where a.tabtype in ('V')  and not (a.tabname like 'sys%') AND a.tabname <>'dual'  ";
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
                return "SELECT TOP 1  * FROM Systables";
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
            string sql = "select *  /* " + Guid.NewGuid() + " */ from " + SqlBuilder.GetTranslationTableName(tableName) + " WHERE 1=2 ";
            this.Context.Utilities.RemoveCache<List<DbColumnInfo>>("DbMaintenanceProvider.GetFieldComment." + tableName);
            this.Context.Utilities.RemoveCache<List<string>>("DbMaintenanceProvider.GetPrimaryKeyByTableNames." + this.SqlBuilder.GetNoTranslationColumnName(tableName).ToLower());
            var oldIsEnableLog = this.Context.Ado.IsEnableLogEvent;
            this.Context.Ado.IsEnableLogEvent = false;
            using (var reader =  this.Context.Ado.GetDataReader(sql))
            {
                this.Context.Ado.IsEnableLogEvent = oldIsEnableLog;
                List<DbColumnInfo> result = new List<DbColumnInfo>();
                var schemaTable = reader.GetSchemaTable();
                int i = 0;
                foreach (System.Data.DataRow row in schemaTable.Rows)
                {
                    DbColumnInfo column = new DbColumnInfo()
                    {
                        TableName = tableName,
                        DataType = row["DataType"].ToString().Replace("System.", "").Trim(),
                        IsNullable = (bool)row["AllowDBNull"],
                        IsIdentity = (bool)row["IsAutoIncrement"],
                       // ColumnDescription = GetFieldComment(tableName, row["ColumnName"].ToString()),
                        DbColumnName = row["ColumnName"].ToString(),
                        //DefaultValue = row["defaultValue"].ToString(),
                        IsPrimarykey = i==0,//no support get pk
                        Length = row["ColumnSize"].ObjToInt(),
                        Scale = row["numericscale"].ObjToInt()
                    };
                    ++i;
                    result.Add(column);
                }
                return result;
            }
        }
        protected override string GetCreateTableSql(string tableName, List<DbColumnInfo> columns)
        {
            List<string> columnArray = new List<string>();
            Check.Exception(columns.IsNullOrEmpty(), "No columns found ");
            foreach (var item in columns)
            {
                string columnName = this.SqlBuilder.GetTranslationTableName(item.DbColumnName);
                string dataType = item.DataType;
                string dataSize = GetSize(item);
                string nullType = item.IsNullable ? this.CreateTableNull : CreateTableNotNull;
                if (item.IsIdentity&&item.IsPrimarykey) 
                {
                    dataSize = "";
                    dataType = "serial primary key";
                }
                else if (item.IsIdentity)
                {
                    dataSize = "";
                    dataType = "serial";
                }
                else if (item.IsPrimarykey)
                {
                    dataType = (dataType + dataSize + " primary key");
                    dataSize = "";
                }
                //string identity = item.IsIdentity ? this.CreateTableIdentity : null;
                string addItem = string.Format(this.CreateTableColumn, columnName, dataType, dataSize, nullType, "", "");
                columnArray.Add(addItem);
            }
            string tableString = string.Format(this.CreateTableSql, this.SqlBuilder.GetTranslationTableName(tableName), string.Join(",\r\n", columnArray));
            return tableString;
        }


        public override bool AddDefaultValue(string tableName, string columnName, string defaultValue)
        {
            if (defaultValue == "''")
            {
                defaultValue = "";
            }
            var template = AddDefaultValueSql;
            if (defaultValue != null && defaultValue.ToLower() == "getdate()") 
            {
                template = template.Replace("'{2}'", "{2}");
            }
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
            throw new NotSupportedException();
        }
        public override void AddDefaultValue(EntityInfo entityInfo)
        {
            
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
            }
            string sql = GetCreateTableSql(tableName, columns);
            this.Context.Ado.ExecuteCommand(sql);
            //if (isCreatePrimaryKey)
            //{
            //    var pkColumns = columns.Where(it => it.IsPrimarykey).ToList();
            //    if (pkColumns.Count > 1)
            //    {
            //        this.Context.DbMaintenance.AddPrimaryKeys(tableName, pkColumns.Select(it => it.DbColumnName).ToArray());
            //    }
            //    else
            //    {
            //        foreach (var item in pkColumns)
            //        {
            //            this.Context.DbMaintenance.AddPrimaryKey(tableName, item.DbColumnName);
            //        }
            //    }
            //}
            return true;
        }
        //public override List<DbColumnInfo> GetColumnInfosByTableName(string tableName, bool isCache = true)
        //{
        //    tableName = SqlBuilder.GetNoTranslationColumnName(tableName);
        //    var result= base.GetColumnInfosByTableName(tableName, isCache);
        //    return result;
        //}
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
