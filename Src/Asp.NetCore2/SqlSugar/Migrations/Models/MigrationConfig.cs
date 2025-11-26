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
