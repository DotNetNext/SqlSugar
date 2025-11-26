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
