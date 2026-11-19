using Apache.IoTDB.Data;
using Apache.IoTDB.DataStructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using SqlSugar;

namespace SqlSugar.IoTDB
{
    public sealed class IoTDBDbMaintenance : DbMaintenanceProvider
    {
        protected override string GetViewInfoListSql => throw new NotSupportedException("IoTDB tree model has no relational views.");
        protected override string GetDataBaseSql => "SHOW DATABASES";
        protected override string GetTableInfoListSql => throw new NotSupportedException("IoTDB devices are discovered through tree metadata.");
        protected override string GetColumnInfosByTableNameSql => throw new NotSupportedException("IoTDB measurements are discovered through tree metadata.");
        protected override string CreateIndexSql => throw new NotSupportedException("IoTDB tree model does not support relational indexes.");
        protected override string IsAnyIndexSql => throw new NotSupportedException("IoTDB tree model does not support relational indexes.");
        protected override string AddDefaultValueSql => throw new NotSupportedException("IoTDB measurements do not support relational defaults.");
        protected override string CreateDataBaseSql => "CREATE DATABASE {0}";
        protected override string AddColumnToTableSql => throw new NotSupportedException();
        protected override string AlterColumnToTableSql => throw new NotSupportedException();
        protected override string BackupDataBaseSql => throw new NotSupportedException();
        protected override string CreateTableSql => throw new NotSupportedException();
        protected override string CreateTableColumn => throw new NotSupportedException();
        protected override string BackupTableSql => throw new NotSupportedException();
        protected override string TruncateTableSql => throw new NotSupportedException();
        protected override string DropTableSql => throw new NotSupportedException();
        protected override string DropColumnToTableSql => throw new NotSupportedException();
        protected override string DropConstraintSql => throw new NotSupportedException();
        protected override string AddPrimaryKeySql => throw new NotSupportedException("IoTDB tree model does not support relational primary keys.");
        protected override string RenameColumnSql => throw new NotSupportedException();
        protected override string AddColumnRemarkSql => throw new NotSupportedException();
        protected override string DeleteColumnRemarkSql => throw new NotSupportedException();
        protected override string IsAnyColumnRemarkSql => throw new NotSupportedException();
        protected override string AddTableRemarkSql => throw new NotSupportedException();
        protected override string DeleteTableRemarkSql => throw new NotSupportedException();
        protected override string IsAnyTableRemarkSql => throw new NotSupportedException();
        protected override string RenameTableSql => throw new NotSupportedException();
        protected override string CheckSystemTablePermissionsSql => "SHOW DATABASES";
        protected override string CreateTableNull => string.Empty;
        protected override string CreateTableNotNull => string.Empty;
        protected override string CreateTablePirmaryKey => throw new NotSupportedException("IoTDB tree model does not support relational primary keys.");
        protected override string CreateTableIdentity => throw new NotSupportedException("IoTDB tree model has no auto-increment identity columns.");

        public override List<DbTableInfo> GetTableInfoList(bool isCache = true)
        {
            var database = Options.Database;
            var rows = ReadMetadata("SHOW DEVICES " + database + ".**");
            var result = new List<DbTableInfo>();
            foreach (var row in rows)
            {
                var name = Value(row, "Device", "Devices", "Timeseries", "TimeseriesName");
                if (string.IsNullOrWhiteSpace(name)) continue;
                var prefix = database + ".";
                if (name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) name = name.Substring(prefix.Length);
                if (name.Contains(".")) name = name.Substring(0, name.IndexOf('.'));
                if (!result.Any(it => it.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                    result.Add(new DbTableInfo { Name = name, DbObjectType = DbObjectType.Table });
            }
            return result;
        }

        public override bool IsAnyTable(string tableName, bool isCache = true)
        {
            var path = Options.GetDevicePath(SqlBuilder.GetNoTranslationColumnName(tableName));
            return ReadMetadata("SHOW TIMESERIES " + path + ".*").Count > 0;
        }

        public override List<DbColumnInfo> GetColumnInfosByTableName(string tableName, bool isCache = true)
        {
            var path = Options.GetDevicePath(SqlBuilder.GetNoTranslationColumnName(tableName));
            var rows = ReadMetadata("SHOW TIMESERIES " + path + ".*");
            var result = new List<DbColumnInfo>();
            foreach (var row in rows)
            {
                var fullPath = Value(row, "Timeseries", "TimeseriesName", "Path");
                var type = Value(row, "DataType", "Datatype", "Type");
                if (string.IsNullOrWhiteSpace(fullPath)) continue;
                var name = fullPath.Substring(fullPath.LastIndexOf('.') + 1);
                result.Add(new DbColumnInfo
                {
                    DbColumnName = name,
                    DataType = type ?? "TEXT",
                    IsNullable = true,
                    TableName = path
                });
            }
            return result;
        }

        public override List<DbTableInfo> GetViewInfoList(bool isCache = true) => new List<DbTableInfo>();
        public override List<string> GetDataBaseList() => ReadMetadata("SHOW DATABASES")
            .Select(row => Value(row, "Database", "DatabaseName", "Name", "Path") ?? Convert.ToString(row.Values.FirstOrDefault()))
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .ToList();

        public override bool IsAnySystemTablePermissions()
        {
            this.Context.Ado.CheckConnection();
            return true;
        }

        public override bool CreateDatabase(string databaseDirectory = null)
        {
            return CreateDatabase(string.IsNullOrWhiteSpace(databaseDirectory) ? Options.Database : databaseDirectory, null);
        }

        public override bool CreateDatabase(string databaseName, string databaseDirectory = null)
        {
            var path = databaseName.StartsWith("root.", StringComparison.OrdinalIgnoreCase) ? databaseName : "root." + databaseName;
            ValidatePath(path);
            var exists = ReadMetadata("SHOW DATABASES").Any(row => row.Values.Any(value =>
                string.Equals(Convert.ToString(value), path, StringComparison.OrdinalIgnoreCase)));
            if (exists) return true;
            try
            {
                this.Context.Ado.ExecuteCommand(BuildCreateDatabaseSql(path));
            }
            catch
            {
                var createdConcurrently = ReadMetadata("SHOW DATABASES").Any(row => row.Values.Any(value =>
                    string.Equals(Convert.ToString(value), path, StringComparison.OrdinalIgnoreCase)));
                if (!createdConcurrently) throw;
            }
            return true;
        }

        public override bool CreateTable(string tableName, List<DbColumnInfo> columns, bool isCreatePrimaryKey = true)
        {
            var path = Options.GetDevicePath(SqlBuilder.GetNoTranslationColumnName(tableName));
            foreach (var column in columns.Where(it => !IsTimeColumn(it.DbColumnName) && !it.IsIdentity))
            {
                CreateTimeseries(path, column);
            }
            return true;
        }

        public override bool AddColumn(string tableName, DbColumnInfo columnInfo)
        {
            if (!IsTimeColumn(columnInfo.DbColumnName)) CreateTimeseries(Options.GetDevicePath(SqlBuilder.GetNoTranslationColumnName(tableName)), columnInfo);
            return true;
        }

        public override bool AddPrimaryKey(string tableName, string columnName) => throw new NotSupportedException("IoTDB tree model has no relational primary keys.");
        public override bool DropColumn(string tableName, string columnName) => throw new NotSupportedException("Dropping a measurement requires an explicit DELETE TIMESERIES operation.");
        public override bool DropTable(string tableName)
        {
            var path = Options.GetDevicePath(SqlBuilder.GetNoTranslationColumnName(tableName));
            this.Context.Ado.ExecuteCommand("DELETE TIMESERIES " + path + ".*");
            return true;
        }
        public override bool TruncateTable(string tableName)
        {
            var path = Options.GetDevicePath(SqlBuilder.GetNoTranslationColumnName(tableName));
            this.Context.Ado.ExecuteCommand("DELETE FROM " + path + ".* WHERE time <= now()");
            return true;
        }
        public override bool UpdateColumn(string tableName, DbColumnInfo columnInfo) => throw new NotSupportedException("IoTDB measurement types cannot be altered in place.");
        public override bool AddDefaultValue(string tableName, string columnName, string defaultValue) => throw new NotSupportedException("IoTDB measurements do not support relational defaults.");
        public override bool AddRemark(EntityInfo entity) => true;
        public override void AddIndex(EntityInfo entityInfo) { }
        public override bool IsAnyIndex(string indexName) => false;
        public override bool CreateIndex(string tableName, string[] columnNames, bool isUnique = false) => throw new NotSupportedException("IoTDB tree model does not support relational indexes.");
        public override bool CreateIndex(string tableName, string[] columnNames, string indexName, bool isUnique = false) => throw new NotSupportedException("IoTDB tree model does not support relational indexes.");
        public override bool BackupDataBase(string databaseName, string fullFileName) => throw new NotSupportedException("Use IoTDB export/backup tooling for tree data.");
        public override bool BackupTable(string oldTableName, string newTableName, int maxBackupDataRows = int.MaxValue) => throw new NotSupportedException("Use IoTDB export/backup tooling for tree data.");

        private IoTDBConnectionOptions Options => IoTDBConnectionOptions.Parse(this.Context.CurrentConnectionConfig.ConnectionString);

        private void CreateTimeseries(string devicePath, DbColumnInfo column)
        {
            var measurement = Options.GetMeasurementName(column.DbColumnName);
            var path = devicePath + "." + measurement;
            if (ReadMetadata("SHOW TIMESERIES " + path).Count > 0) return;
            var type = TypeName(column.DataType, column);
            try
            {
                this.Context.Ado.ExecuteCommand(BuildCreateTimeseriesSql(path, type));
            }
            catch
            {
                if (ReadMetadata("SHOW TIMESERIES " + path).Count == 0) throw;
            }
        }

        internal static string BuildCreateDatabaseSql(string path) => "CREATE DATABASE " + path;

        internal static string BuildCreateTimeseriesSql(string path, string type) =>
            "CREATE TIMESERIES " + path + " WITH DATATYPE=" + type + ", ENCODING=" + Encoding(type);

        private List<Dictionary<string, object>> ReadMetadata(string sql)
        {
            var wasOpen = this.Context.Ado.Connection.State == ConnectionState.Open;
            this.Context.Ado.CheckConnection();
            try
            {
                var connection = (IoTDBConnection)this.Context.Ado.Connection;
                using (var dataSet = connection.SessionPool.ExecuteQueryStatementAsync(sql).GetAwaiter().GetResult())
                {
                    var names = dataSet.GetColumnNames();
                    var rows = new List<Dictionary<string, object>>();
                    while (dataSet.HasNextAsync().GetAwaiter().GetResult())
                    {
                        var record = dataSet.Next();
                        var row = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                        for (var index = 0; index < names.Count && index < record.Values.Count; index++)
                            row[names[index]] = record.Values[index];
                        rows.Add(row);
                    }
                    return rows;
                }
            }
            finally
            {
                if (!wasOpen && this.Context.CurrentConnectionConfig.IsAutoCloseConnection)
                    this.Context.Ado.Connection.Close();
            }
        }

        private static string? Value(Dictionary<string, object> row, params string[] names)
        {
            foreach (var name in names)
                if (row.TryGetValue(name, out var value) && value != null && value != DBNull.Value)
                    return Convert.ToString(value);
            return null;
        }

        private string TypeName(string declaredType, DbColumnInfo column)
        {
            var type = declaredType;
            if (string.IsNullOrWhiteSpace(type) || type.StartsWith("System.", StringComparison.OrdinalIgnoreCase))
                type = new IoTDBDbBind { Context = this.Context }.GetDbTypeName(column.DataType);
            type = type.ToUpperInvariant();
            if (!new[] { "BOOLEAN", "INT32", "INT64", "FLOAT", "DOUBLE", "TEXT", "STRING", "BLOB", "TIMESTAMP", "DATE" }.Contains(type))
                throw new NotSupportedException("Unsupported IoTDB measurement type: " + type);
            return type;
        }

        private static string Encoding(string type)
        {
            switch (type)
            {
                case "BOOLEAN": return "PLAIN";
                case "INT32":
                case "INT64": return "RLE";
                case "FLOAT":
                case "DOUBLE": return "GORILLA";
                default: return "PLAIN";
            }
        }

        private static bool IsTimeColumn(string columnName) => columnName.Equals("time", StringComparison.OrdinalIgnoreCase) || columnName.Equals("timestamp", StringComparison.OrdinalIgnoreCase);

        private static void ValidatePath(string path)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(path, @"^root(?:\.[A-Za-z0-9_]+)+$"))
                throw new ArgumentException("Invalid IoTDB database path.", nameof(path));
        }
    }
}
