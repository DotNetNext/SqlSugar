using SqlSugar;
using System;

namespace WWB.Park.Entity
{

    public interface IDeletedFilter
    {
        bool IsDelete { get; set; }
    }

    public interface IAgentFilter
    {
        long AgentId { get; set; }
    }

    public interface IMemberFilter
    {
        long MemberId { get; set; }
    }

    public abstract class EntityBase<TPrimary>
    {
        [SugarColumn(ColumnName = "id")] public TPrimary Id { get; set; }
    }

    public abstract class AuditDeleteEntityBase<TPrimary> : AuditEntityBase<TPrimary>, IDeletedFilter
    {
        [SugarColumn(ColumnName = "is_delete", ColumnDescription = "软删除")]
        public bool IsDelete { get; set; }
    }

    public abstract class AuditEntityBase<TPrimary> : EntityBase<TPrimary>
    {
        /// <summary>
        ///
        /// </summary>
        [SugarColumn(ColumnName = "add_time", IsOnlyIgnoreUpdate = true)]
        public DateTime AddTime { get; set; }

        /// <summary>
        ///
        /// </summary>
        [SugarColumn(ColumnName = "update_time", IsNullable = true, IsOnlyIgnoreInsert = true)]
        public DateTime UpdateTime { get; set; }
    }
}