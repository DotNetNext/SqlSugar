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
