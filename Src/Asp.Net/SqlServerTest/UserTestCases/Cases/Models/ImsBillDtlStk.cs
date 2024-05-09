using SqlSugar;

namespace IWMS.Bill.Models
{
    /// <summary>
    /// 单据条码关联表
    /// </summary>
    [SugarTable("IMS_BILL_DTL_STK")]
    public class ImsBillDtlStk
    {
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, ColumnName = "ID")]
        public decimal Id { get; set; }

        /// <summary>
        /// 租户ID
        /// </summary>
        [SugarColumn(ColumnName = "TENANT_ID")]
        public decimal TenantId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnName = "CREATED_TIME", IsOnlyIgnoreInsert = true, IsOnlyIgnoreUpdate = true)]
        public System.DateTime CreatedTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [SugarColumn(ColumnName = "CREATED_BY", IsOnlyIgnoreUpdate = true)]
        public string CreatedBy { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [SugarColumn(ColumnName = "UPDATED_TIME", IsOnlyIgnoreInsert = true)]
        public System.DateTime? UpdatedTime { get; set; }

        /// <summary>
        /// 最后更新人
        /// </summary>
        [SugarColumn(ColumnName = "UPDATED_BY", IsOnlyIgnoreInsert = true)]
        public string UpdatedBy { get; set; }

        /// <summary>
        /// 删除标记
        /// </summary>
        [SugarColumn(ColumnName = "DELETED", IsOnlyIgnoreInsert = true)]
        public string Deleted { get; set; }

        /// <summary>
        /// 单据明细表ID
        /// </summary>
        [SugarColumn(ColumnName = "DTL_ID")]
        public decimal DtlId { get; set; }

        /// <summary>
        /// 单据库存表ID
        /// </summary>
        [SugarColumn(ColumnName = "STK_ID")]
        public decimal StkId { get; set; }

        /// <summary>
        /// 充单量
        /// </summary>
        [SugarColumn(ColumnName = "QTY")]
        public decimal Qty { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public decimal RowIndex { get; set; }
    }
}
