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
