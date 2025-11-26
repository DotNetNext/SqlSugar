using System;

namespace SqlSugar.SoftDelete
{
    /// <summary>
    /// Audit entry for soft delete operations
    /// </summary>
    public class SoftDeleteAuditEntry
    {
        public long Id { get; set; }
        public string TableName { get; set; }
        public string EntityId { get; set; }
        public SoftDeleteOperation Operation { get; set; }
        public string PerformedBy { get; set; }
        public DateTime PerformedAt { get; set; }
        public string Reason { get; set; }
        public string EntitySnapshot { get; set; }
        public bool IsCascaded { get; set; }
    }

    public enum SoftDeleteOperation
    {
        Delete = 1,
        Restore = 2,
        PermanentDelete = 3
    }
}
