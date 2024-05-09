using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations; 
using SqlSugar;

namespace tech.fuhong.mes.basis.entity
{

    /// <summary>
    /// 工序信息 - 数据实体
    /// 
    /// @author 复弘智能
    /// @version 1.0  2023-12-09
    /// </summary>
    [SugarTable("basis_process_unit")]
    [Description("工序信息")]
    public class ProcessUnit : BizEntity
    {
        /// <summary>
        /// 记录编号（关键字）
        /// </summary>
        [SugarColumn(ColumnName = "process_unit_id", ColumnDescription = "记录编号", Length = 10, IsPrimaryKey = true, IsIdentity = true)]
        public long? Id { get; set; }

        /// <summary>
        /// 工序编码
        /// </summary>
        [SugarColumn(ColumnName = "process_unit_code", ColumnDescription = "工序编码", Length = 50, IsNullable = false)]
        public String? ProcessUnitCode { get; set; }

        /// <summary>
        ///// 工序名称
        ///// </summary>
        //[SugarColumn(ColumnName = "process_unit_name", ColumnDescription = "工序名称", Length = 50, IsNullable = false)]
        //public String? ProcessUnitName { get; set; }

        ///// <summary>
        ///// 有效标识
        ///// </summary>
        //[SugarColumn(ColumnName = "active_flag", ColumnDescription = "有效标识", IsNullable = false)]
        //public bool? ActiveFlag { get; set; }

        ///// <summary>
        ///// 同步源记录ID
        ///// </summary>
        //[SugarColumn(ColumnName = "sync_record_id", ColumnDescription = "同步源记录ID", Length = 50, IsNullable = true)]
        //public String? SyncRecordId { get; set; }
        ///// <summary>
        ///// 最近同步时间
        ///// </summary>
        //[SugarColumn(ColumnName = "sync_time", ColumnDescription = "最近同步时间", IsNullable = true)]
        //public DateTime? SyncTime { get; set; }
        ///// <summary>
        ///// 最近同步时间 （查询上限）
        ///// </summary>
        //[SugarColumn(IsIgnore = true)]
        //public DateTime? SyncTimeTop { get; set; }
        ///// <summary>
        ///// 最近同步时间 （查询下限）
        ///// </summary>
        //[SugarColumn(IsIgnore = true)]
        //public DateTime? SyncTimeBottom { get; set; }

        ///// <summary>
        ///// 哈希值
        ///// </summary>
        //public override int GetHashCode()
        //{
        //    return (Id == null) ? 0 : Id.GetHashCode();
        //}

        ///// <summary>
        ///// 两个对象是否相等
        ///// </summary>
        //public override bool Equals(Object obj)
        //{
        //    if (Id == null || obj == null || !(obj is ProcessUnit))
        //    {
        //        return false;
        //    }

        //    return Id.Equals(((ProcessUnit)obj).Id);
        //}
    }

    public class BizEntity
    { 
    }
}