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
