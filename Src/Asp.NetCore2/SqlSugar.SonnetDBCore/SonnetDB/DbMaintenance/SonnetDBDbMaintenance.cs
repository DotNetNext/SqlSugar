using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace SqlSugar.SonnetDB
{
    public class SonnetDBDbMaintenance : DbMaintenanceProvider
    {
        protected override string GetDataBaseSql => "SHOW DATABASES";
        protected override string GetColumnInfosByTableNameSql => "SELECT * FROM information_schema.columns WHERE table_name = '{0}'";
        protected override string GetTableInfoListSql => "SELECT table_name AS Name FROM information_schema.tables WHERE table_type = 'BASE TABLE'";
        protected override string GetViewInfoListSql => "SELECT table_name AS Name FROM information_schema.views";
        protected override string CreateDataBaseSql => "CREATE DATABASE {0}";
        protected override string AddPrimaryKeySql => "ALTER TABLE {0} ADD CONSTRAINT {1} PRIMARY KEY ({2})";
        protected override string AddColumnToTableSql => "ALTER TABLE {0} ADD COLUMN {1} {2}{3} {4}";
        protected override string AlterColumnToTableSql => "ALTER TABLE {0} ALTER COLUMN {1} TYPE {2}{3} {4}";
        protected override string BackupDataBaseSql => throw new NotSupportedException("SonnetDB has no SQL database backup statement.");
        protected override string CreateTableSql => "CREATE TABLE {0} (\r\n{1}\r\n)";
        protected override string CreateTableColumn => "{0} {1}{2} {5} {3}";
        protected override string BackupTableSql => "CREATE TABLE {1} AS SELECT * FROM {2} LIMIT {0}";
        protected override string TruncateTableSql => "TRUNCATE TABLE {0}";
        protected override string DropTableSql => "DROP TABLE {0}";
        protected override string DropColumnToTableSql => "ALTER TABLE {0} DROP COLUMN {1}";
        protected override string DropConstraintSql => "ALTER TABLE {0} DROP CONSTRAINT {1}";
        protected override string RenameColumnSql => "ALTER TABLE {0} RENAME COLUMN {1} TO {2}";
        protected override string RenameTableSql => "ALTER TABLE {0} RENAME TO {1}";
        protected override string CreateIndexSql => "CREATE {3} INDEX {2} ON {0} ({1})";
        protected override string IsAnyIndexSql => "SELECT COUNT(*) FROM information_schema.indexes WHERE index_name = '{0}'";
        protected override string AddDefaultValueSql => "ALTER TABLE {0} ALTER COLUMN {1} SET DEFAULT {2}";
        protected override string AddColumnRemarkSql => "COMMENT ON COLUMN {1}.{0} IS '{2}'";
        protected override string DeleteColumnRemarkSql => "COMMENT ON COLUMN {1}.{0} IS NULL";
        protected override string IsAnyColumnRemarkSql => "SELECT column_name FROM information_schema.columns WHERE table_name = '{1}' AND column_name = '{0}' AND column_comment IS NOT NULL";
        protected override string AddTableRemarkSql => "COMMENT ON TABLE {0} IS '{1}'";
        protected override string DeleteTableRemarkSql => "COMMENT ON TABLE {0} IS NULL";
        protected override string IsAnyTableRemarkSql => "SELECT table_name FROM information_schema.tables WHERE table_name = '{0}' AND table_comment IS NOT NULL";
        protected override string CheckSystemTablePermissionsSql => "SELECT 1";
        protected override string CreateTableNull => "NULL";
        protected override string CreateTableNotNull => "NOT NULL";
        protected override string CreateTablePirmaryKey => "PRIMARY KEY";
        protected override string CreateTableIdentity => "AUTO_INCREMENT";

        public override List<string> GetDataBaseList()
        {
            return new List<string> { Context.Ado.Connection.Database };
        }

        public override List<string> GetDataBaseList(SqlSugarClient db)
        {
            return new List<string> { db.Ado.Connection.Database };
        }

        public override List<DbTableInfo> GetTableInfoList(bool isCache = true)
        {
            return GetSchema("Tables").Rows.Cast<DataRow>()
                .Where(row => string.Equals(GetString(row, "TABLE_TYPE"), "BASE TABLE", StringComparison.OrdinalIgnoreCase))
                .Select(row => new DbTableInfo
                {
                    Name = GetString(row, "TABLE_NAME"),
                    Description = GetString(row, "TABLE_COMMENT"),
                    DbObjectType = DbObjectType.Table
                })
                .OrderBy(table => table.Name, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public override List<DbColumnInfo> GetColumnInfosByTableName(string tableName, bool isCache = true)
        {
            tableName = ResolveTableName(tableName);
            return GetSchema("Columns", tableName).Rows.Cast<DataRow>()
                .Select(row =>
                {
                    var dataType = GetString(row, "DATA_TYPE");
                    var isDecimal = string.Equals(dataType, "DECIMAL", StringComparison.OrdinalIgnoreCase) ||
                                    string.Equals(dataType, "NUMERIC", StringComparison.OrdinalIgnoreCase);
                    return new DbColumnInfo
                    {
                        TableId = GetInt(row, "ORDINAL_POSITION") - 1,
                        TableName = GetString(row, "TABLE_NAME"),
                        DbColumnName = GetString(row, "COLUMN_NAME"),
                        ColumnDescription = GetString(row, "COLUMN_COMMENT"),
                        DataType = dataType,
                        DefaultValue = GetString(row, "COLUMN_DEFAULT"),
                        IsNullable = GetBool(row, "IS_NULLABLE"),
                        IsPrimarykey = GetBool(row, "IS_PRIMARY_KEY"),
                        IsIdentity = GetBool(row, "IS_AUTO_INCREMENT"),
                        Length = isDecimal ? GetInt(row, "NUMERIC_PRECISION") : 0,
                        DecimalDigits = isDecimal ? GetInt(row, "NUMERIC_SCALE") : 0,
                        Scale = isDecimal ? GetInt(row, "NUMERIC_SCALE") : 0
                    };
                })
                .OrderBy(column => column.TableId)
                .ToList();
        }

        public override List<string> GetIndexList(string tableName)
        {
            return GetSchema("Indexes", ResolveTableName(tableName)).Rows.Cast<DataRow>()
                .Select(row => GetString(row, "INDEX_NAME")!)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public override List<string> GetDbTypes()
        {
            return GetSchema("DataTypes").Rows.Cast<DataRow>()
                .Select(row => GetString(row, "TypeName")!)
                .ToList();
        }

        public override List<string> GetProcList(string dbName)
        {
            return ReadObjectNames("SHOW PROCEDURES");
        }

        public override List<string> GetFuncList()
        {
            return ReadObjectNames("SHOW FUNCTIONS");
        }

        public override List<string> GetTriggerNames(string tableName)
        {
            return ReadObjectNames("SHOW TRIGGERS ON " + SqlBuilder.GetTranslationTableName(tableName));
        }

        public override bool IsAnySystemTablePermissions()
        {
            GetSchema("Tables");
            return true;
        }

        public override bool IsAnyIndex(string indexName)
        {
            indexName = SqlBuilder.GetNoTranslationColumnName(indexName);
            return GetSchema("Indexes").Rows.Cast<DataRow>().Any(row =>
                string.Equals(GetString(row, "INDEX_NAME"), indexName, StringComparison.OrdinalIgnoreCase));
        }

        public override bool IsIdentity(string tableName, string columnName)
        {
            columnName = SqlBuilder.GetNoTranslationColumnName(columnName);
            return GetColumnInfosByTableName(tableName, false).Any(column => column.IsIdentity &&
                string.Equals(column.DbColumnName, columnName, StringComparison.OrdinalIgnoreCase));
        }

        public override bool IsAnyConstraint(string constraintName)
        {
            return Context.Ado.GetInt(
                "SELECT COUNT(*) FROM information_schema.table_constraints WHERE constraint_name = @name",
                new SugarParameter("@name", constraintName)) > 0;
        }

        public override bool IsAnyProcedure(string procName)
        {
            return GetProcList(Context.Ado.Connection.Database).Any(name =>
                string.Equals(name, SqlBuilder.GetNoTranslationColumnName(procName), StringComparison.OrdinalIgnoreCase));
        }

        public override bool CreateTable(string tableName, List<DbColumnInfo> columns, bool isCreatePrimaryKey = true)
        {
            var sql = GetCreateTableSql(tableName, columns);
            var primaryKeys = columns.Where(column => column.IsPrimarykey).ToList();
            if (isCreatePrimaryKey && primaryKeys.Count > 0)
            {
                var primaryKey = ",\r\nPRIMARY KEY (" + string.Join(", ", primaryKeys.Select(column =>
                    SqlBuilder.GetTranslationColumnName(column.DbColumnName))) + ")";
                sql = sql.Insert(sql.LastIndexOf("\r\n)", StringComparison.Ordinal), primaryKey);
            }

            Context.Ado.ExecuteCommand(sql);
            return true;
        }

        public override bool AddDefaultValue(string tableName, string columnName, string defaultValue)
        {
            var column = GetColumnInfosByTableName(tableName, false).First(column =>
                string.Equals(column.DbColumnName, SqlBuilder.GetNoTranslationColumnName(columnName), StringComparison.OrdinalIgnoreCase));
            // SqlSugar permits unquoted text defaults; expressions remain SQL expressions.
            if (column.DataType.ToUpperInvariant() is "STRING" or "JSON" or "DATETIME" or "TIME" &&
                !defaultValue.StartsWith("'", StringComparison.Ordinal) && !defaultValue.Contains("(") &&
                !defaultValue.Equals("NULL", StringComparison.OrdinalIgnoreCase) &&
                !defaultValue.Equals("CURRENT_TIMESTAMP", StringComparison.OrdinalIgnoreCase) &&
                !defaultValue.Equals("CURRENT_DATE", StringComparison.OrdinalIgnoreCase))
            {
                defaultValue = defaultValue.ToSqlValue();
            }

            Context.Ado.ExecuteCommand(string.Format(AddDefaultValueSql,
                SqlBuilder.GetTranslationTableName(tableName),
                SqlBuilder.GetTranslationColumnName(columnName), defaultValue));
            return true;
        }

        public override bool CreateIndex(string tableName, string[] columnNames, string indexName, bool isUnique = false)
        {
            // Omitting explicit ASC is equivalent to SonnetDB's default direction; DESC and INCLUDE are retained.
            var columns = columnNames.Select(column => column.EndsWith(" ASC", StringComparison.OrdinalIgnoreCase)
                ? column.Substring(0, column.Length - 4)
                : column).ToArray();
            return base.CreateIndex(tableName, columns, indexName, isUnique);
        }

        public override bool DropIndex(string indexName)
        {
            var name = SqlBuilder.GetNoTranslationColumnName(indexName);
            var index = GetSchema("Indexes").Rows.Cast<DataRow>().FirstOrDefault(row =>
                string.Equals(GetString(row, "INDEX_NAME"), name, StringComparison.OrdinalIgnoreCase));
            return index != null && DropIndex(indexName, GetString(index, "TABLE_NAME"));
        }

        public override bool DropIndex(string indexName, string tableName)
        {
            Context.Ado.ExecuteCommand("DROP INDEX " + SqlBuilder.GetTranslationColumnName(indexName) +
                                       " ON " + SqlBuilder.GetTranslationTableName(tableName));
            return true;
        }

        public override bool RenameTable(string oldTableName, string newTableName)
        {
            return base.RenameTable(SqlBuilder.GetTranslationTableName(oldTableName), SqlBuilder.GetTranslationTableName(newTableName));
        }

        public override bool SetAutoIncrementInitialValue(string tableName, int initialValue)
        {
            if (initialValue <= 0) throw new ArgumentOutOfRangeException(nameof(initialValue));
            Context.Ado.ExecuteCommand("ALTER TABLE " + SqlBuilder.GetTranslationTableName(ResolveTableName(tableName)) +
                " AUTO_INCREMENT = " + initialValue.ToString(System.Globalization.CultureInfo.InvariantCulture));
            return true;
        }

        public override bool AddPrimaryKey(string tableName, string columnName)
        {
            tableName = ResolveTableName(tableName);
            columnName = SqlBuilder.GetNoTranslationColumnName(columnName);
            string name = "PK_" + tableName + "_" + columnName;
            Context.Ado.ExecuteCommand(string.Format(AddPrimaryKeySql,
                SqlBuilder.GetTranslationTableName(tableName), SqlBuilder.GetTranslationColumnName(name),
                SqlBuilder.GetTranslationColumnName(columnName)));
            return true;
        }

        public override bool DropConstraint(string tableName, string constraintName)
        {
            Context.Ado.ExecuteCommand("ALTER TABLE " + SqlBuilder.GetTranslationTableName(ResolveTableName(tableName)) +
                " DROP CONSTRAINT " + SqlBuilder.GetTranslationColumnName(constraintName));
            return true;
        }

        public override bool SetAutoIncrementInitialValue(Type entityType, int initialValue)
        {
            return SetAutoIncrementInitialValue(Context.EntityMaintenance.GetEntityInfo(entityType).DbTableName, initialValue);
        }

        protected override string GetSize(DbColumnInfo column)
        {
            if (column.DataType.Contains("(") ||
                column.DataType.ToUpperInvariant() is "INT" or "FLOAT" or "BOOL" or "STRING" or "DATETIME" or "TIME" or "BLOB" or "JSON")
            {
                return string.Empty;
            }

            return base.GetSize(column);
        }

        private DataTable GetSchema(string collectionName, string? tableName = null)
        {
            Context.Ado.CheckConnection();
            var connection = (DbConnection)Context.Ado.Connection;
            return tableName == null
                ? connection.GetSchema(collectionName)
                : connection.GetSchema(collectionName, new[] { null, null, tableName, null });
        }

        private string ResolveTableName(string tableName)
        {
            tableName = SqlBuilder.GetNoTranslationColumnName(tableName);
            return GetSchema("Tables").Rows.Cast<DataRow>()
                .Select(row => GetString(row, "TABLE_NAME"))
                .FirstOrDefault(name => string.Equals(name, tableName, StringComparison.OrdinalIgnoreCase)) ?? tableName;
        }

        private List<string> ReadObjectNames(string sql)
        {
            return Context.Ado.GetDataTable(sql).Rows.Cast<DataRow>()
                .Select(row => Convert.ToString(row[0])!)
                .ToList();
        }

        private static string? GetString(DataRow row, string name)
        {
            return row.Table.Columns.Contains(name) && row[name] != DBNull.Value ? Convert.ToString(row[name]) : null;
        }

        private static bool GetBool(DataRow row, string name)
        {
            return row.Table.Columns.Contains(name) && row[name] != DBNull.Value && Convert.ToBoolean(row[name]);
        }

        private static int GetInt(DataRow row, string name)
        {
            return row.Table.Columns.Contains(name) && row[name] != DBNull.Value ? Convert.ToInt32(row[name]) : 0;
        }
    }
}
