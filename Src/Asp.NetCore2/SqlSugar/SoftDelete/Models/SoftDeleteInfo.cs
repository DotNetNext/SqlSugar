using System;

namespace SqlSugar.SoftDelete
{
    /// <summary>
    /// Soft delete metadata for an entity
    /// </summary>
    public class SoftDeleteInfo
    {
        public string EntityName { get; set; }
        public object EntityId { get; set; }
        public DateTime DeletedAt { get; set; }
        public string DeletedBy { get; set; }
        public string DeletedReason { get; set; }
        public bool CanRestore { get; set; } = true;
        public DateTime? PermanentDeleteAt { get; set; }
    }
}
