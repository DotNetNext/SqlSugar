using System;

namespace SqlSugar.SoftDelete
{
    /// <summary>
    /// Configuration for soft delete behavior
    /// </summary>
    public class SoftDeleteConfig
    {
        /// <summary>
        /// Global enable/disable soft delete feature
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Field name for soft delete flag (default: IsDeleted)
        /// </summary>
        public string DeletedFieldName { get; set; } = "IsDeleted";

        /// <summary>
        /// Field name for deletion timestamp (default: DeletedAt)
        /// </summary>
        public string DeletedAtFieldName { get; set; } = "DeletedAt";

        /// <summary>
        /// Field name for deletion user (default: DeletedBy)
        /// </summary>
        public string DeletedByFieldName { get; set; } = "DeletedBy";

        /// <summary>
        /// Field name for deletion reason (optional)
        /// </summary>
        public string DeletedReasonFieldName { get; set; } = "DeletedReason";

        /// <summary>
        /// Enable cascade soft delete for related entities
        /// </summary>
        public bool EnableCascadeDelete { get; set; } = false;

        /// <summary>
        /// Auto-exclude soft deleted records in queries
        /// </summary>
        public bool AutoFilter { get; set; } = true;

        /// <summary>
        /// Days before permanent delete (0 = never, -1 = disabled)
        /// </summary>
        public int PermanentDeleteAfterDays { get; set; } = -1;

        /// <summary>
        /// Enable audit trail for soft delete operations
        /// </summary>
        public bool EnableAuditTrail { get; set; } = false;

        /// <summary>
        /// Current user provider for DeletedBy field
        /// </summary>
        public Func<string> CurrentUserProvider { get; set; }

        /// <summary>
        /// Custom deleted value (default: true)
        /// </summary>
        public object DeletedValue { get; set; } = true;

        /// <summary>
        /// Custom not-deleted value (default: false)
        /// </summary>
        public object NotDeletedValue { get; set; } = false;
    }
}
