using System;
using System.Collections.Generic;

namespace SqlSugar.SoftDelete
{
    /// <summary>
    /// Statistics for soft delete operations
    /// </summary>
    public class SoftDeleteStatistics
    {
        public int TotalSoftDeleted { get; set; }
        public int TotalRestored { get; set; }
        public int TotalPermanentDeleted { get; set; }
        public DateTime? OldestSoftDeletedDate { get; set; }
        public Dictionary<string, int> DeletedByTable { get; set; } = new Dictionary<string, int>();
        public List<SoftDeleteInfo> PendingPermanentDeletes { get; set; } = new List<SoftDeleteInfo>();
    }
}
