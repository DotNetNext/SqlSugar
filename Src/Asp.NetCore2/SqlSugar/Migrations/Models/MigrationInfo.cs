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
