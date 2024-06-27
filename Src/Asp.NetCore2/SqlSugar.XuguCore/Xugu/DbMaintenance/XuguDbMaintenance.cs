using System;
using System.Collections.Generic;
using System.Xml.Linq;
using static Npgsql.Replication.PgOutput.Messages.RelationMessage;

namespace SqlSugar.Xugu
{
    public class XuguDbMaintenance : DbMaintenanceProvider
    {
        #region DML
        /// <summary>
        /// 列出所有数据库名
        /// </summary>
        protected override string GetDataBaseSql => "SELECT DB_NAME FROM USER_DATABASES";
        /// <summary>
        /// 列出指定表所有的列属性
        /// </summary>
        protected override string GetColumnInfosByTableNameSql
        {
            get
            {
                //重命名的列必须经过处理，否则会找不到（官方DLL的BUG）
                string sql = @"SELECT C.TABLE_ID+0 `TABLEID`,
	TRIM(T.TABLE_NAME) `TABLENAME`,
	TRIM(C.COL_NAME) `DBCOLUMNNAME`,
	TRIM(C.TYPE_NAME) `DATATYPE`,
	CASE WHEN C.SCALE>=65536 THEN -1 ELSE C.SCALE END `LENGTH`,
	TRIM(C.DEF_VAL) `DEFAULTVALUE`,
	TRIM(C.COMMENTS) `COLUMNDESCRIPTION`,
	CASE WHEN C.SCALE>=65536 THEN FLOOR(C.SCALE / 65536) ELSE -1 END `SCALE`,
	CASE WHEN C.SCALE>=65536 THEN C.SCALE - FLOOR(C.SCALE / 65536)*65536 ELSE -1 END `DECIMALDIGITS`,
	CASE WHEN C.NOT_NULL THEN FALSE ELSE TRUE END `ISNULLABLE`,
	CASE WHEN C.IS_SERIAL THEN TRUE ELSE FALSE END `ISIDENTITY`,
	CASE WHEN I.IS_PRIMARY THEN TRUE ELSE FALSE END `ISPRIMARYKEY`
FROM USER_COLUMNS C
LEFT JOIN USER_TABLES T ON T.TABLE_ID=C.TABLE_ID
LEFT JOIN USER_INDEXES I ON T.TABLE_ID=I.TABLE_ID AND I.KEYS LIKE '%""'+C.COL_NAME+'""%'
WHERE T.TABLE_NAME='{0}' AND T.DB_ID=CURRENT_DB_ID
	AND T.USER_ID=CURRENT_USERID AND T.SCHEMA_ID=CURRENT_SCHEMAID
ORDER BY C.COL_NO";//FIND_IN_SET('""'+C.COL_NAME+'""',I.KEYS)>0
                return sql;
            }
        }
        /// <summary>
        /// 获取用户所有表名、描述
        /// </summary>
        protected override string GetTableInfoListSql=> "SELECT TRIM(TABLE_NAME) name,TRIM(COMMENTS) Description FROM USER_TABLES";
        /// <summary>
        /// 获取用户所有视图名、描述
        /// </summary>
        protected override string GetViewInfoListSql=> "SELECT TRIM(VIEW_NAME) name,TRIM(COMMENTS) Description FROM USER_VIEWS";
        #endregion

        #region DDL
        /// <summary>
        /// 创建数据库
        /// </summary>
        protected override string CreateDataBaseSql => "CREATE DATABASE {0}  ";
        protected override string AddPrimaryKeySql=> "ALTER TABLE {0} ADD CONSTRAINT {1} PRIMARY KEY({2})";
        protected override string AddColumnToTableSql=> "ALTER TABLE {0} ADD COLUMN {1} {2}{3} @IDENT@{6} @PK@{5} {4}";
        protected override string AlterColumnToTableSql=> "ALTER TABLE {0} ALTER COLUMN {1} {2}{3} @IDENT@{6} @PK@{5} {4}";
        /// <summary>
        /// 备份数据库，文件路径为服务器上相对于XHOME的路径
        /// </summary>
        protected override string BackupDataBaseSql => "BACKUP DATABASE TO '{1}'";
        protected override string CreateTableSql=> "CREATE TABLE {0}(\r\n{1})";
        protected override string CreateTableColumn=> "{0} {1}{2} {3} {4} {5}";
        /// <summary>
        /// 清空表
        /// </summary>
        protected override string TruncateTableSql=> "TRUNCATE TABLE {0}";
        protected override string BackupTableSql=> "SELECT TOP {0} * INTO {1} FROM  {2}";
        protected override string DropTableSql=> "DROP TABLE {0}";
        protected override string DropColumnToTableSql=> "ALTER TABLE {0} DROP COLUMN {1}";
        protected override string DropConstraintSql=> "ALTER TABLE {0} DROP CONSTRAINT  {1}";
        protected override string RenameColumnSql => "ALTER TABLE {0} RENAME COLUMN {1} TO {2}";
        protected override string AddColumnRemarkSql => "COMMENT ON COLUMN {1}.{0} IS '{2}'";

        protected override string DeleteColumnRemarkSql => "COMMENT ON COLUMN {1}.{0} IS ''";

        protected override string IsAnyColumnRemarkSql
        {
            get
            {
                return @"SELECT T.TABLE_NAME AS table_name,C.COL_NAME AS column_name,C.COMMENTS AS column_description
FROM USER_COLUMNS C
LEFT JOIN USER_TABLES T ON T.TABLE_ID=C.TABLE_ID
WHERE T.TABLE_NAME='{1}' AND C.COL_NAME='{0}' AND T.DB_ID=CURRENT_DB_ID
	AND T.USER_ID=CURRENT_USERID AND T.SCHEMA_ID=CURRENT_SCHEMAID";

            }
        }

        protected override string AddTableRemarkSql=> "COMMENT ON TABLE {0} IS '{1}'";

        protected override string DeleteTableRemarkSql=> "COMMENT ON TABLE {0} IS ''";

        protected override string IsAnyTableRemarkSql
        {
            get
            {
                return @"SELECT TRIM(COMMENTS) class_desc 
FROM USER_TABLES T
WHERE T.TABLE_NAME='{0}' AND T.DB_ID=CURRENT_DB_ID
	AND T.USER_ID=CURRENT_USERID AND T.SCHEMA_ID=CURRENT_SCHEMAID";
            }

        }

        protected override string RenameTableSql => "ALTER TABLE {0} RENAME TO {1}";

        protected override string CreateIndexSql=> "CREATE {3} INDEX IX_{0}_{2} ON {0}({1})";//NONCLUSTERED
        protected override string AddDefaultValueSql=> "ALTER TABLE {0} ALTER COLUMN {1} SET DEFAULT '{2}'";
        protected override string IsAnyIndexSql => "SELECT COUNT(*) FROM USER_INDEXES WHERE INDEX_NAME='{0}'";
        #endregion

        #region Check
        /// <summary>
        /// 判断是否能够读取系统表user_tables
        /// </summary>
        protected override string CheckSystemTablePermissionsSql=> "SELECT TOP 1  * FROM USER_TABLES";
        #endregion

        #region Scattered
        protected override string CreateTableNull=> string.Empty;// "NULL";
        protected override string CreateTableNotNull=> "NOT NULL";
        protected override string CreateTablePirmaryKey=> " PRIMARY KEY ";
        protected override string CreateTableIdentity=> " IDENTITY ";//(1,1)
        #endregion

        #region Methods
        //public override List<DbColumnInfo> GetColumnInfosByTableName(string tableName, bool isCache = true)
        //{
        //    string cacheKey = "DbMaintenanceProvider.GetColumnInfosByTableName." + this.SqlBuilder.GetNoTranslationColumnName(tableName).ToLower();
        //    cacheKey = GetCacheKey(cacheKey);
        //    if (!isCache)
        //        return GetColumnInfosByTableName(tableName);
        //    else
        //        return this.Context.Utilities.GetReflectionInoCacheInstance().GetOrCreate(cacheKey,
        //                () =>
        //                {
        //                    return GetColumnInfosByTableName(tableName);

        //                });
        //}

        private List<DbColumnInfo> GetColumnInfosByTableName(string tableName)
        {
            string sql = "select *  /* " + Guid.NewGuid() + " */ from " + SqlBuilder.GetTranslationTableName(tableName) + " WHERE 1=2 ";
            this.Context.Utilities.RemoveCache<List<DbColumnInfo>>("DbMaintenanceProvider.GetFieldComment." + tableName);
            this.Context.Utilities.RemoveCache<List<string>>("DbMaintenanceProvider.GetPrimaryKeyByTableNames." + this.SqlBuilder.GetNoTranslationColumnName(tableName).ToLower());
            var oldIsEnableLog = this.Context.Ado.IsEnableLogEvent;
            this.Context.Ado.IsEnableLogEvent = false;
            using (var reader = this.Context.Ado.GetDataReader(sql))
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
                        IsPrimarykey = i == 0,//no support get pk
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
                if (item.IsIdentity && item.IsPrimarykey)
                {
                    dataSize = "";
                    dataType += this.CreateTableIdentity + this.CreateTablePirmaryKey;
                }
                else if (item.IsIdentity)
                {
                    dataSize = "";
                    dataType += this.CreateTableIdentity;
                }
                else if (item.IsPrimarykey)
                {
                    dataType = (dataType + dataSize + this.CreateTablePirmaryKey);
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
            if (defaultValue != null && defaultValue.ToLower() == "SYSDATE")
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
        protected override string GetAddColumnSql(string tableName, DbColumnInfo columnInfo)
        {
            return base.GetAddColumnSql(tableName, columnInfo)
                .Replace("@IDENT@", columnInfo.IsIdentity?this.CreateTableIdentity:string.Empty)
                .Replace("@PK@", columnInfo.IsPrimarykey ? this.CreateTablePirmaryKey : string.Empty);
        }
        protected override string GetUpdateColumnSql(string tableName, DbColumnInfo columnInfo)
        {
            return base.GetUpdateColumnSql(tableName, columnInfo)
                .Replace("@IDENT@", columnInfo.IsIdentity ? this.CreateTableIdentity : string.Empty)
                .Replace("@PK@", columnInfo.IsPrimarykey ? this.CreateTablePirmaryKey : string.Empty);
        }
        public override bool UpdateColumn(string tableName, DbColumnInfo column)
        {
            try
            {
                tableName = this.SqlBuilder.GetTranslationTableName(tableName);
                string sql = GetUpdateColumnSql(tableName, column);
                Console.WriteLine(sql);
                this.Context.Ado.ExecuteCommand(sql);
                return true;
            }
            catch { 
                //Console.WriteLine($"修改数据库列{tableName}.{column.DbColumnName}发生异常");
                return false;
            }
        }
        public override bool AddPrimaryKey(string tableName, string columnName)
        {
            try
            {
                tableName = this.SqlBuilder.GetTranslationTableName(tableName);
                columnName = this.SqlBuilder.GetTranslationTableName(columnName);
                string sql = string.Format(this.AddPrimaryKeySql, tableName, string.Format("PK_{0}_{1}", this.SqlBuilder.GetNoTranslationColumnName(tableName), this.SqlBuilder.GetNoTranslationColumnName(columnName)), columnName);
                Console.WriteLine(sql);
                this.Context.Ado.ExecuteCommand(sql);
                return true;
            }
            catch {
                //Console.WriteLine($"添加主键 PK_{tableName}_{columnName}发生异常");
                return false;
            }
        }
        #endregion
    }
}