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
