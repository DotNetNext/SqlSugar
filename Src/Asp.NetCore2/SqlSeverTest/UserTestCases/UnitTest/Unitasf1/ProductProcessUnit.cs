using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations; 
using SqlSugar;

namespace tech.fuhong.mes.basis.entity
{

    /// <summary>
    /// 产品工艺下的工序明细 - 数据实体
    /// 
    /// @author 复弘智能
    /// @version 1.0  2023-12-22
    /// </summary>
    [SugarTable("basis_product_process_unit")]
    [Description("产品工艺下的工序明细")]
    public class ProductProcessUnit : BizEntity
    {
        /// <summary>
        /// 记录编号（关键字）
        /// </summary>
        [SugarColumn(ColumnName = "id", ColumnDescription = "记录编号", Length = 10, IsPrimaryKey = true, IsIdentity = true)]
        public long? Id { get; set; }

        ///// <summary>
        ///// 产品工艺记录编号，关联basis_product_process.process_id
        ///// </summary>
        //[SugarColumn(ColumnName = "process_id", ColumnDescription = "产品工艺记录编号，关联basis_product_process.process_id", Length = 10, IsNullable = false)]
        //public long? ProcessId { get; set; }
        ///// <summary>
        ///// 工序记录编号，关联basis_process_unit.process_unit_id
        ///// </summary>
        //[SugarColumn(ColumnName = "process_unit_id", ColumnDescription = "工序记录编号，关联basis_process_unit.process_unit_id", Length = 10, IsNullable = false)]
        //public long? ProcessUnitId { get; set; }
        /// <summary>
        /// 工序信息
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public ProcessUnit? ProcessUnit { get; set; }
        ///// <summary>
        ///// 作业指导书文档地址
        ///// </summary>
        //[SugarColumn(ColumnName = "working_instruction_url", ColumnDescription = "作业指导书文档地址", Length = 200, IsNullable = true)]
        //public String? WorkingInstructionUrl { get; set; }

        /// <summary>
        /// 检验标准文档地址
        /// </summary>
        [SugarColumn(ColumnName = "inspection_standard_url", ColumnDescription = "检验标准文档地址", Length = 200, IsNullable = true)]
        public String? InspectionStandardUrl { get; set; }

        ///// <summary>
        ///// 排序号（正序）
        ///// </summary>
        //[SugarColumn(ColumnName = "sort_number", ColumnDescription = "排序号（正序）", Length = 4, IsNullable = false)]
        //public int? SortNumber { get; set; }

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
        ///// 产品记录编号，关联basis_material.material_id
        ///// </summary>
        //[SugarColumn(ColumnName = "product_id", ColumnDescription = "产品记录编号，关联basis_material.material_id", Length = 10, IsNullable = false)]
        //public long? ProductId { get; set; }
        ///// <summary>
        ///// 下道工序记录编号，关联basis_process_unit.process_unit_id
        ///// </summary>
        //[SugarColumn(ColumnName = "next_process_unit_id", ColumnDescription = "下道工序记录编号，关联basis_process_unit.process_unit_id", Length = 10, IsNullable = true)]
        //public long? NextProcessUnitId { get; set; }
        /// <summary>
        /// 下一道工序信息
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public ProcessUnit? NextProcessUnit { get; set; }

        /// <summary>
        /// 哈希值
        /// </summary>
        public override int GetHashCode()
        {
            return (Id == null) ? 0 : Id.GetHashCode();
        }

        /// <summary>
        /// 两个对象是否相等
        /// </summary>
        public override bool Equals(Object obj)
        {
            if (Id == null || obj == null || !(obj is ProductProcessUnit))
            {
                return false;
            }

            return Id.Equals(((ProductProcessUnit)obj).Id);
        }
    }
}