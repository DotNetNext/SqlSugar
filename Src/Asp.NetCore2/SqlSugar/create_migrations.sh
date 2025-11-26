#!/bin/bash

echo "Creating SqlSugar Migration System..."

# ==================== 1. Migration.cs ====================
cat > Migrations/Models/Migration.cs << 'EOF'
using System;

namespace SqlSugar.Migrations
{
    public abstract class Migration
    {
        public abstract void Up(SchemaBuilder schema);
        public abstract void Down(SchemaBuilder schema);

        public long Version
        {
            get
            {
                var attr = (MigrationAttribute)Attribute.GetCustomAttribute(GetType(), typeof(MigrationAttribute));
                return attr?.Version ?? 0;
            }
        }

        public string Description
        {
            get
            {
                var attr = (MigrationAttribute)Attribute.GetCustomAttribute(GetType(), typeof(MigrationAttribute));
                return attr?.Description ?? GetType().Name;
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class MigrationAttribute : Attribute
    {
        public long Version { get; }
        public string Description { get; }

        public MigrationAttribute(long version, string description = null)
        {
            Version = version;
            Description = description;
        }
    }
}
EOF

# ==================== 2. MigrationHistoryEntry.cs ====================
cat > Migrations/Models/MigrationHistoryEntry.cs << 'EOF'
using System;

namespace SqlSugar.Migrations
{
    [SugarTable("__MigrationHistory")]
    public class MigrationHistoryEntry
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        [SugarColumn(IsNullable = false)]
        public long Version { get; set; }

        [SugarColumn(Length = 255)]
        public string Description { get; set; }

        [SugarColumn(Length = 500)]
        public string MigrationClass { get; set; }

        [SugarColumn(IsNullable = false)]
        public DateTime AppliedAt { get; set; }

        [SugarColumn(Length = 100)]
        public string AppliedBy { get; set; }

        [SugarColumn(IsNullable = false)]
        public int ExecutionTimeMs { get; set; }

        [SugarColumn(Length = 50)]
        public string Status { get; set; }

        [SugarColumn(ColumnDataType = "text")]
        public string ErrorMessage { get; set; }
    }
}
EOF

# ==================== 3. MigrationInfo.cs ====================
cat > Migrations/Models/MigrationInfo.cs << 'EOF'
using System;

namespace SqlSugar.Migrations
{
    public class MigrationInfo
    {
        public long Version { get; set; }
        public string Description { get; set; }
        public Type MigrationType { get; set; }
        public string ClassName { get; set; }
        public bool IsApplied { get; set; }
        public DateTime? AppliedAt { get; set; }
        public int? ExecutionTimeMs { get; set; }

        public override string ToString()
        {
            var status = IsApplied ? "✓ Applied" : "⧗ Pending";
            var time = AppliedAt.HasValue ? $" at {AppliedAt:yyyy-MM-dd HH:mm:ss}" : "";
            return $"{Version} - {Description} [{status}{time}]";
        }
    }
}
EOF

# ==================== 4. MigrationConfig.cs ====================
cat > Migrations/Models/MigrationConfig.cs << 'EOF'
using System;
using System.Reflection;

namespace SqlSugar.Migrations
{
    public class MigrationConfig
    {
        public string HistoryTableName { get; set; } = "__MigrationHistory";
        public Assembly MigrationAssembly { get; set; }
        public string MigrationNamespace { get; set; }
        public bool UseTransaction { get; set; } = true;
        public Func<string> CurrentUserProvider { get; set; }
        public bool AutoCreateHistoryTable { get; set; } = true;
        public int CommandTimeout { get; set; } = 300;
    }
}
EOF

# ==================== 5. MigrationException.cs ====================
cat > Migrations/Models/MigrationException.cs << 'EOF'
using System;

namespace SqlSugar.Migrations
{
    public class MigrationException : Exception
    {
        public long? Version { get; set; }
        public string MigrationName { get; set; }

        public MigrationException(string message) : base(message) { }

        public MigrationException(string message, Exception innerException) 
            : base(message, innerException) { }

        public MigrationException(long version, string migrationName, string message) 
            : base($"Migration {version} ({migrationName}): {message}")
        {
            Version = version;
            MigrationName = migrationName;
        }

        public MigrationException(long version, string migrationName, string message, Exception innerException)
            : base($"Migration {version} ({migrationName}): {message}", innerException)
        {
            Version = version;
            MigrationName = migrationName;
        }
    }
}
EOF

echo "✓ Models (5/5)"

# ==================== 6. IMigrationRunner.cs ====================
cat > Migrations/Core/IMigrationRunner.cs << 'EOF'
using System.Collections.Generic;

namespace SqlSugar.Migrations
{
    public interface IMigrationRunner
    {
        List<MigrationInfo> GetAllMigrations();
        List<MigrationInfo> GetPendingMigrations();
        List<MigrationInfo> GetAppliedMigrations();
        int RunPending();
        void MigrateTo(long version);
        void Rollback(int steps = 1);
        void Reset();
        long GetCurrentVersion();
    }
}
EOF

# ==================== 7. MigrationRunner.cs ====================
cat > Migrations/Core/MigrationRunner.cs << 'EOF'
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SqlSugar.Migrations
{
    public class MigrationRunner : IMigrationRunner
    {
        private readonly SqlSugarClient _client;
        private readonly MigrationConfig _config;
        private readonly MigrationDiscovery _discovery;
        private readonly MigrationHistoryService _history;

        public MigrationRunner(SqlSugarClient client, MigrationConfig config = null)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _config = config ?? new MigrationConfig();
            _discovery = new MigrationDiscovery(_config);
            _history = new MigrationHistoryService(_client, _config);

            if (_config.AutoCreateHistoryTable)
            {
                _history.EnsureHistoryTableExists();
            }
        }

        public List<MigrationInfo> GetAllMigrations()
        {
            var discovered = _discovery.DiscoverMigrations();
            var applied = _history.GetAppliedVersions();

            return discovered.Select(m => new MigrationInfo
            {
                Version = m.Version,
                Description = m.Description,
                MigrationType = m.MigrationType,
                ClassName = m.ClassName,
                IsApplied = applied.ContainsKey(m.Version),
                AppliedAt = applied.ContainsKey(m.Version) ? applied[m.Version].AppliedAt : (DateTime?)null,
                ExecutionTimeMs = applied.ContainsKey(m.Version) ? applied[m.Version].ExecutionTimeMs : (int?)null
            }).OrderBy(m => m.Version).ToList();
        }

        public List<MigrationInfo> GetPendingMigrations()
        {
            return GetAllMigrations().Where(m => !m.IsApplied).ToList();
        }

        public List<MigrationInfo> GetAppliedMigrations()
        {
            return GetAllMigrations().Where(m => m.IsApplied).ToList();
        }

        public int RunPending()
        {
            var pending = GetPendingMigrations();
            if (!pending.Any()) return 0;

            foreach (var migration in pending)
            {
                ApplyMigration(migration.MigrationType, migration.Version, migration.Description);
            }

            return pending.Count;
        }

        public void MigrateTo(long targetVersion)
        {
            var currentVersion = GetCurrentVersion();
            
            if (currentVersion == targetVersion) return;

            if (currentVersion < targetVersion)
            {
                var toApply = GetPendingMigrations()
                    .Where(m => m.Version <= targetVersion)
                    .OrderBy(m => m.Version)
                    .ToList();

                foreach (var migration in toApply)
                {
                    ApplyMigration(migration.MigrationType, migration.Version, migration.Description);
                }
            }
            else
            {
                var toRollback = GetAppliedMigrations()
                    .Where(m => m.Version > targetVersion)
                    .OrderByDescending(m => m.Version)
                    .ToList();

                foreach (var migration in toRollback)
                {
                    RollbackMigration(migration.MigrationType, migration.Version, migration.Description);
                }
            }
        }

        public void Rollback(int steps = 1)
        {
            var applied = GetAppliedMigrations().OrderByDescending(m => m.Version).Take(steps).ToList();
            if (!applied.Any()) return;

            foreach (var migration in applied)
            {
                RollbackMigration(migration.MigrationType, migration.Version, migration.Description);
            }
        }

        public void Reset()
        {
            var applied = GetAppliedMigrations().OrderByDescending(m => m.Version).ToList();
            if (!applied.Any()) return;

            foreach (var migration in applied)
            {
                RollbackMigration(migration.MigrationType, migration.Version, migration.Description);
            }
        }

        public long GetCurrentVersion()
        {
            var applied = GetAppliedMigrations();
            return applied.Any() ? applied.Max(m => m.Version) : 0;
        }

        private void ApplyMigration(Type migrationType, long version, string description)
        {
            var migration = (Migration)Activator.CreateInstance(migrationType);
            var schema = new SchemaBuilder(_client);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                if (_config.UseTransaction) _client.Ado.BeginTran();

                migration.Up(schema);
                
                stopwatch.Stop();
                _history.RecordMigration(version, description, migrationType.FullName, 
                    (int)stopwatch.ElapsedMilliseconds, "Applied");

                if (_config.UseTransaction) _client.Ado.CommitTran();
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                if (_config.UseTransaction) _client.Ado.RollbackTran();
                _history.RecordMigrationError(version, description, migrationType.FullName, ex.Message);
                throw new MigrationException(version, description, "Failed to apply migration", ex);
            }
        }

        private void RollbackMigration(Type migrationType, long version, string description)
        {
            var migration = (Migration)Activator.CreateInstance(migrationType);
            var schema = new SchemaBuilder(_client);

            try
            {
                if (_config.UseTransaction) _client.Ado.BeginTran();

                migration.Down(schema);
                _history.RemoveMigration(version);

                if (_config.UseTransaction) _client.Ado.CommitTran();
            }
            catch (Exception ex)
            {
                if (_config.UseTransaction) _client.Ado.RollbackTran();
                throw new MigrationException(version, description, "Failed to rollback migration", ex);
            }
        }
    }
}
EOF

# ==================== 8. MigrationDiscovery.cs ====================
cat > Migrations/Core/MigrationDiscovery.cs << 'EOF'
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SqlSugar.Migrations
{
    public class MigrationDiscovery
    {
        private readonly MigrationConfig _config;

        public MigrationDiscovery(MigrationConfig config)
        {
            _config = config;
        }

        public List<MigrationInfo> DiscoverMigrations()
        {
            var assembly = _config.MigrationAssembly ?? Assembly.GetCallingAssembly();
            var migrationTypes = assembly.GetTypes()
                .Where(t => typeof(Migration).IsAssignableFrom(t) && !t.IsAbstract)
                .Where(t => string.IsNullOrEmpty(_config.MigrationNamespace) || 
                           t.Namespace?.StartsWith(_config.MigrationNamespace) == true)
                .ToList();

            var migrations = new List<MigrationInfo>();

            foreach (var type in migrationTypes)
            {
                var attr = (MigrationAttribute)Attribute.GetCustomAttribute(type, typeof(MigrationAttribute));
                if (attr != null)
                {
                    migrations.Add(new MigrationInfo
                    {
                        Version = attr.Version,
                        Description = attr.Description ?? type.Name,
                        MigrationType = type,
                        ClassName = type.FullName,
                        IsApplied = false
                    });
                }
            }

            return migrations.OrderBy(m => m.Version).ToList();
        }
    }
}
EOF

# ==================== 9. MigrationHistoryService.cs ====================
cat > Migrations/Core/MigrationHistoryService.cs << 'EOF'
using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlSugar.Migrations
{
    public class MigrationHistoryService
    {
        private readonly SqlSugarClient _client;
        private readonly MigrationConfig _config;

        public MigrationHistoryService(SqlSugarClient client, MigrationConfig config)
        {
            _client = client;
            _config = config;
        }

        public void EnsureHistoryTableExists()
        {
            if (!_client.DbMaintenance.IsAnyTable(_config.HistoryTableName, false))
            {
                _client.CodeFirst.InitTables<MigrationHistoryEntry>();
            }
        }

        public Dictionary<long, MigrationHistoryEntry> GetAppliedVersions()
        {
            EnsureHistoryTableExists();
            
            var entries = _client.Queryable<MigrationHistoryEntry>()
                .Where(e => e.Status == "Applied")
                .ToList();

            return entries.ToDictionary(e => e.Version);
        }

        public void RecordMigration(long version, string description, string className, 
            int executionTimeMs, string status)
        {
            var entry = new MigrationHistoryEntry
            {
                Version = version,
                Description = description,
                MigrationClass = className,
                AppliedAt = DateTime.UtcNow,
                AppliedBy = _config.CurrentUserProvider?.Invoke() ?? Environment.UserName,
                ExecutionTimeMs = executionTimeMs,
                Status = status
            };

            _client.Insertable(entry).ExecuteCommand();
        }

        public void RecordMigrationError(long version, string description, string className, string error)
        {
            var entry = new MigrationHistoryEntry
            {
                Version = version,
                Description = description,
                MigrationClass = className,
                AppliedAt = DateTime.UtcNow,
                AppliedBy = _config.CurrentUserProvider?.Invoke() ?? Environment.UserName,
                ExecutionTimeMs = 0,
                Status = "Failed",
                ErrorMessage = error
            };

            _client.Insertable(entry).ExecuteCommand();
        }

        public void RemoveMigration(long version)
        {
            _client.Deleteable<MigrationHistoryEntry>()
                .Where(e => e.Version == version)
                .ExecuteCommand();
        }
    }
}
EOF

echo "✓ Core (4/4)"

# ==================== 10-12. Builders ====================
cat > Migrations/Builders/SchemaBuilder.cs << 'EOF'
using System;

namespace SqlSugar.Migrations
{
    public class SchemaBuilder
    {
        private readonly SqlSugarClient _client;

        public SchemaBuilder(SqlSugarClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public void CreateTable(string tableName, Action<TableBuilder> buildAction)
        {
            var builder = new TableBuilder(_client, tableName);
            buildAction(builder);
            builder.Execute();
        }

        public void DropTable(string tableName)
        {
            _client.DbMaintenance.DropTable(tableName);
        }

        public void RenameTable(string oldName, string newName)
        {
            _client.DbMaintenance.RenameTable(oldName, newName);
        }

        public void AlterTable(string tableName, Action<TableBuilder> alterAction)
        {
            var builder = new TableBuilder(_client, tableName, isAlter: true);
            alterAction(builder);
            builder.Execute();
        }

        public void AddColumn(string tableName, string columnName, Action<ColumnBuilder> columnAction)
        {
            var column = new ColumnBuilder(columnName);
            columnAction(column);
            _client.DbMaintenance.AddColumn(tableName, column.Build());
        }

        public void DropColumn(string tableName, string columnName)
        {
            _client.DbMaintenance.DropColumn(tableName, columnName);
        }

        public void RenameColumn(string tableName, string oldName, string newName)
        {
            _client.DbMaintenance.RenameColumn(tableName, oldName, newName);
        }

        public void CreateIndex(string tableName, string indexName, params string[] columns)
        {
            _client.DbMaintenance.CreateIndex(tableName, columns, indexName, false);
        }

        public void CreateUniqueIndex(string tableName, string indexName, params string[] columns)
        {
            _client.DbMaintenance.CreateIndex(tableName, columns, indexName, true);
        }

        public void DropIndex(string tableName, string indexName)
        {
            _client.DbMaintenance.DropIndex(indexName, tableName);
        }

        public void Sql(string sql)
        {
            _client.Ado.ExecuteCommand(sql);
        }

        public void Sql(string sql, object parameters)
        {
            _client.Ado.ExecuteCommand(sql, parameters);
        }
    }
}
EOF

cat > Migrations/Builders/TableBuilder.cs << 'EOF'
using System.Collections.Generic;

namespace SqlSugar.Migrations
{
    public class TableBuilder
    {
        private readonly SqlSugarClient _client;
        private readonly string _tableName;
        private readonly bool _isAlter;
        private readonly List<DbColumnInfo> _columns = new List<DbColumnInfo>();

        public TableBuilder(SqlSugarClient client, string tableName, bool isAlter = false)
        {
            _client = client;
            _tableName = tableName;
            _isAlter = isAlter;
        }

        private ColumnBuilder AddColumnBuilder(string name)
        {
            var builder = new ColumnBuilder(name);
            _columns.Add(builder.Build());
            return builder;
        }

        public ColumnBuilder Int(string name) => AddColumnBuilder(name).AsInt();
        public ColumnBuilder Long(string name) => AddColumnBuilder(name).AsLong();
        public ColumnBuilder String(string name, int length = 255) => AddColumnBuilder(name).AsString(length);
        public ColumnBuilder Text(string name) => AddColumnBuilder(name).AsText();
        public ColumnBuilder Decimal(string name, int precision = 18, int scale = 2) => AddColumnBuilder(name).AsDecimal(precision, scale);
        public ColumnBuilder Bool(string name) => AddColumnBuilder(name).AsBool();
        public ColumnBuilder DateTime(string name) => AddColumnBuilder(name).AsDateTime();
        public ColumnBuilder Date(string name) => AddColumnBuilder(name).AsDate();
        public ColumnBuilder Guid(string name) => AddColumnBuilder(name).AsGuid();

        public void Execute()
        {
            if (_isAlter)
            {
                foreach (var column in _columns)
                {
                    if (!_client.DbMaintenance.IsAnyColumn(_tableName, column.DbColumnName, false))
                    {
                        _client.DbMaintenance.AddColumn(_tableName, column);
                    }
                }
            }
            else
            {
                _client.DbMaintenance.CreateTable(_tableName, _columns, true);
            }
        }
    }
}
EOF

cat > Migrations/Builders/ColumnBuilder.cs << 'EOF'
namespace SqlSugar.Migrations
{
    public class ColumnBuilder
    {
        private readonly DbColumnInfo _column;

        public ColumnBuilder(string name)
        {
            _column = new DbColumnInfo
            {
                DbColumnName = name,
                IsNullable = true,
                IsPrimarykey = false,
                IsIdentity = false
            };
        }

        public ColumnBuilder AsInt() { _column.DataType = "int"; return this; }
        public ColumnBuilder AsLong() { _column.DataType = "bigint"; return this; }
        public ColumnBuilder AsString(int length = 255) { _column.DataType = "varchar"; _column.Length = length; return this; }
        public ColumnBuilder AsText() { _column.DataType = "text"; return this; }
        public ColumnBuilder AsDecimal(int precision = 18, int scale = 2) { _column.DataType = "decimal"; _column.DecimalDigits = precision; _column.Scale = scale; return this; }
        public ColumnBuilder AsBool() { _column.DataType = "bit"; return this; }
        public ColumnBuilder AsDateTime() { _column.DataType = "datetime"; return this; }
        public ColumnBuilder AsDate() { _column.DataType = "date"; return this; }
        public ColumnBuilder AsGuid() { _column.DataType = "uniqueidentifier"; return this; }
        public ColumnBuilder PrimaryKey() { _column.IsPrimarykey = true; _column.IsNullable = false; return this; }
        public ColumnBuilder AutoIncrement() { _column.IsIdentity = true; return this; }
        public ColumnBuilder NotNull() { _column.IsNullable = false; return this; }
        public ColumnBuilder Nullable() { _column.IsNullable = true; return this; }
        public ColumnBuilder Default(string value) { _column.DefaultValue = value; return this; }

        public DbColumnInfo Build() => _column;
    }
}
EOF

echo "✓ Builders (3/3)"

# ==================== 13-14. Extensions ====================
cat > Migrations/Extensions/MigrationExtensions.cs << 'EOF'
using System;

namespace SqlSugar.Migrations
{
    public static class MigrationExtensions
    {
        private const string MIGRATION_RUNNER_KEY = "__MigrationRunner__";

        public static IMigrationRunner Migrations(this ISqlSugarClient client, MigrationConfig config = null)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));

            var sqlSugarClient = client as SqlSugarClient ?? 
                throw new InvalidOperationException("Client must be SqlSugarClient");

            if (!client.TempItems.ContainsKey(MIGRATION_RUNNER_KEY))
            {
                client.TempItems[MIGRATION_RUNNER_KEY] = new MigrationRunner(sqlSugarClient, config);
            }

            return client.TempItems[MIGRATION_RUNNER_KEY] as IMigrationRunner;
        }

        public static IMigrationRunner Migrations(this ISqlSugarClient client, Action<MigrationConfig> configAction)
        {
            var config = new MigrationConfig();
            configAction?.Invoke(config);
            return client.Migrations(config);
        }
    }
}
EOF

cat > Migrations/Extensions/SchemaBuilderExtensions.cs << 'EOF'
using System;

namespace SqlSugar.Migrations
{
    public static class SchemaBuilderExtensions
    {
        public static void CreateTableIfNotExists(this SchemaBuilder schema, SqlSugarClient client, 
            string tableName, Action<TableBuilder> buildAction)
        {
            if (!client.DbMaintenance.IsAnyTable(tableName, false))
            {
                schema.CreateTable(tableName, buildAction);
            }
        }

        public static void DropTableIfExists(this SchemaBuilder schema, SqlSugarClient client, string tableName)
        {
            if (client.DbMaintenance.IsAnyTable(tableName, false))
            {
                schema.DropTable(tableName);
            }
        }

        public static void AddColumnIfNotExists(this SchemaBuilder schema, SqlSugarClient client,
            string tableName, string columnName, Action<ColumnBuilder> columnAction)
        {
            if (!client.DbMaintenance.IsAnyColumn(tableName, columnName, false))
            {
                schema.AddColumn(tableName, columnName, columnAction);
            }
        }

        public static void DropColumnIfExists(this SchemaBuilder schema, SqlSugarClient client,
            string tableName, string columnName)
        {
            if (client.DbMaintenance.IsAnyColumn(tableName, columnName, false))
            {
                schema.DropColumn(tableName, columnName);
            }
        }
    }
}
EOF

echo "✓ Extensions (2/2)"
echo ""
echo "=== Migration System Complete ==="
echo "Files created: $(find Migrations -name "*.cs" | wc -l)"
find Migrations -name "*.cs" | sort

