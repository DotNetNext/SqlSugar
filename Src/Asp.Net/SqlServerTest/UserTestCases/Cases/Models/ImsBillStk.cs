using SqlSugar;

namespace IWMS.Bill.Models
{
    /// <summary>
    /// 单据条码记录表
    /// </summary>
    [SugarTable("IMS_BILL_STK")]
    public class ImsBillStk
    {
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, ColumnName = "ID")]
        public decimal Id { get; set; }

        /// <summary>
        /// 单据ID
        /// </summary>
        [SugarColumn(ColumnName = "MST_ID")]
        public decimal MstId { get; set; }

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
        /// 预估量
        /// </summary>
        [SugarColumn(ColumnName = "QTY")]
        public decimal Qty { get; set; }

        /// <summary>
        /// 实际量
        /// </summary>
        [SugarColumn(ColumnName = "ACTUAL_QTY")]
        public decimal ActualQty { get; set; }

        /// <summary>
        /// 库存ID
        /// </summary>
        [SugarColumn(ColumnName = "STOCK_ID")]
        public decimal StockId { get; set; }

        /// <summary>
        /// 条码
        /// </summary>
        [SugarColumn(ColumnName = "BCD")]
        public decimal Bcd { get; set; }

        /// <summary>
        /// 原条码
        /// </summary>
        [SugarColumn(ColumnName = "SOURCE_BCD")]
        public decimal SourceBcd { get; set; }

        /// <summary>
        /// 打印标记
        /// </summary>
        [SugarColumn(ColumnName = "PRINT_FLAG")]
        public string PrintFlag { get; set; }

        /// <summary>
        /// 打印次数
        /// </summary>
        [SugarColumn(ColumnName = "PRINT_COUNT")]
        public decimal PrintCount { get; set; }

        /// <summary>
        /// 打印时间
        /// </summary>
        [SugarColumn(ColumnName = "PRINT_DATE")]
        public System.DateTime? PrintDate { get; set; }

        /// <summary>
        /// 打印人员
        /// </summary>
        [SugarColumn(ColumnName = "PRINT_BY")]
        public string PrintBy { get; set; }

        /// <summary>
        /// 需求号
        /// </summary>
        [SugarColumn(ColumnName = "REQ_CODE")]
        public string ReqCode { get; set; }

        /// <summary>
        /// 源库别
        /// </summary>
        [SugarColumn(ColumnName = "SRC_SIC_ID")]
        public decimal SrcSicId { get; set; }

        /// <summary>
        /// 目的库别
        /// </summary>
        [SugarColumn(ColumnName = "DST_SIC_ID")]
        public decimal DstSicId { get; set; }

        /// <summary>
        /// 源储位
        /// </summary>
        [SugarColumn(ColumnName = "FROM_LOC_ID")]
        public decimal FromLocId { get; set; }

        /// <summary>
        /// 目的储位
        /// </summary>
        [SugarColumn(ColumnName = "TO_LOC_ID")]
        public decimal ToLocId { get; set; }

        /// <summary>
        /// 栈板码
        /// </summary>
        [SugarColumn(ColumnName = "PALLET")]
        public string Pallet { get; set; }

        /// <summary>
        /// 外箱码
        /// </summary>
        [SugarColumn(ColumnName = "CARTON")]
        public string Carton { get; set; }

        /// <summary>
        /// 第一层内箱码
        /// </summary>
        [SugarColumn(ColumnName = "BOX_L1")]
        public string BoxL1 { get; set; }

        /// <summary>
        /// 第二层内箱码
        /// </summary>
        [SugarColumn(ColumnName = "BOX_L2")]
        public string BoxL2 { get; set; }

        /// <summary>
        /// 第三层内箱码
        /// </summary>
        [SugarColumn(ColumnName = "BOX_L3")]
        public string BoxL3 { get; set; }

        /// <summary>
        /// 第四层内箱码
        /// </summary>
        [SugarColumn(ColumnName = "BOX_L4")]
        public string BoxL4 { get; set; }

        /// <summary>
        /// BIN值
        /// </summary>
        [SugarColumn(ColumnName = "BIN")]
        public string Bin { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [SugarColumn(ColumnName = "STATUS")]
        public string Status { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(ColumnName = "REMARK")]
        public string Remark { get; set; }

        /// <summary>
        /// 捡料标记
        /// </summary>
        [SugarColumn(ColumnName = "PICKED_FLAG")]
        public string PickedFlag { get; set; }

        /// <summary>
        /// 捡料时间
        /// </summary>
        [SugarColumn(ColumnName = "PICKED_DATE")]
        public System.DateTime PickedDate { get; set; }

        /// <summary>
        /// 捡料人
        /// </summary>
        [SugarColumn(ColumnName = "PICKED_USER")]
        public string PickedUser { get; set; }

        /// <summary>
        /// 捡料备注信息
        /// </summary>
        [SugarColumn(ColumnName = "PICKED_REMARK")]
        public string PickedRemark { get; set; }

        /// <summary>
        /// 交接确认标记
        /// </summary>
        [SugarColumn(ColumnName = "TSF_CFM_FLAG")]
        public string TsfCfmFlag { get; set; }

        /// <summary>
        /// 交接确认时间
        /// </summary>
        [SugarColumn(ColumnName = "TSF_CFM_DATE")]
        public System.DateTime TsfCfmDate { get; set; }

        /// <summary>
        /// 交接确认人
        /// </summary>
        [SugarColumn(ColumnName = "TSF_CFM_USER")]
        public string TsfCfmUser { get; set; }

        /// <summary>
        /// 交接确认数量
        /// </summary>
        [SugarColumn(ColumnName = "TSF_CFM_QTY")]
        public decimal TsfCfmQty { get; set; }

        /// <summary>
        /// 交接确认备注信息
        /// </summary>
        [SugarColumn(ColumnName = "TSF_CFM_REMARK")]
        public string TsfCfmRemark { get; set; }

        /// <summary>
        /// 出货扫描标记
        /// </summary>
        [SugarColumn(ColumnName = "SHIP_MKR_FLAG")]
        public string ShipMkrFlag { get; set; }

        /// <summary>
        /// 出货扫描时间
        /// </summary>
        [SugarColumn(ColumnName = "SHIP_MKR_DATE")]
        public System.DateTime ShipMkrDate { get; set; }

        /// <summary>
        /// 出货扫描人员
        /// </summary>
        [SugarColumn(ColumnName = "SHIP_MKR_USER")]
        public string ShipMkrUser { get; set; }

        /// <summary>
        /// 出货扫描数量
        /// </summary>
        [SugarColumn(ColumnName = "SHIP_MKR_QTY")]
        public decimal ShipMkrQty { get; set; }

        /// <summary>
        /// 出货扫描备注信息
        /// </summary>
        [SugarColumn(ColumnName = "SHIP_MKR")]
        public string ShipMkr { get; set; }

        /// <summary>
        /// OOBA标记
        /// </summary>
        [SugarColumn(ColumnName = "OOBA_FLAG")]
        public string OobaFlag { get; set; }

        /// <summary>
        /// OOBA时间
        /// </summary>
        [SugarColumn(ColumnName = "OOBA_DATE")]
        public System.DateTime OobaDate { get; set; }

        /// <summary>
        /// OOBA人员
        /// </summary>
        [SugarColumn(ColumnName = "OOBA_USER")]
        public string OobaUser { get; set; }

        /// <summary>
        /// OOBA数量
        /// </summary>
        [SugarColumn(ColumnName = "OOBA_QTY")]
        public decimal OobaQty { get; set; }

        /// <summary>
        /// OOBA备注信息
        /// </summary>
        [SugarColumn(ColumnName = "OOBA_REMARK")]
        public string OobaRemark { get; set; }

        /// <summary>
        /// 关闭人员
        /// </summary>
        [SugarColumn(ColumnName = "CLOSED_BY")]
        public string ClosedBy { get; set; }

        /// <summary>
        /// 关闭时间
        /// </summary>
        [SugarColumn(ColumnName = "CLOSED_DATE")]
        public System.DateTime ClosedDate { get; set; }

        /// <summary>
        /// 扩展1
        /// </summary>
        [SugarColumn(ColumnName = "ATT1")]
        public string Att1 { get; set; }

        /// <summary>
        /// 扩展2
        /// </summary>
        [SugarColumn(ColumnName = "ATT2")]
        public string Att2 { get; set; }

        /// <summary>
        /// 扩展3
        /// </summary>
        [SugarColumn(ColumnName = "ATT3")]
        public string Att3 { get; set; }

        /// <summary>
        /// 扩展4
        /// </summary>
        [SugarColumn(ColumnName = "ATT4")]
        public string Att4 { get; set; }

        /// <summary>
        /// 扩展5
        /// </summary>
        [SugarColumn(ColumnName = "ATT5")]
        public string Att5 { get; set; }

        /// <summary>
        /// 备用数值1
        /// </summary>
        [SugarColumn(ColumnName = "NUM1")]
        public decimal Num1 { get; set; }

        /// <summary>
        /// 备用数值2
        /// </summary>
        [SugarColumn(ColumnName = "NUM2")]
        public decimal Num2 { get; set; }

        /// <summary>
        /// 备用数值3
        /// </summary>
        [SugarColumn(ColumnName = "NUM3")]
        public decimal Num3 { get; set; }

        /// <summary>
        /// 备用数值4
        /// </summary>
        [SugarColumn(ColumnName = "NUM4")]
        public decimal Num4 { get; set; }

        /// <summary>
        /// 备用数值5
        /// </summary>
        [SugarColumn(ColumnName = "NUM5")]
        public decimal Num5 { get; set; }

        /// <summary>
        /// 备用日期1
        /// </summary>
        [SugarColumn(ColumnName = "DT1")]
        public System.DateTime Dt1 { get; set; }

        /// <summary>
        /// 备用日期2
        /// </summary>
        [SugarColumn(ColumnName = "DT2")]
        public System.DateTime Dt2 { get; set; }

        /// <summary>
        /// 备用日期3
        /// </summary>
        [SugarColumn(ColumnName = "DT3")]
        public System.DateTime Dt3 { get; set; }

        /// <summary>
        /// 备用日期4
        /// </summary>
        [SugarColumn(ColumnName = "DT4")]
        public System.DateTime Dt4 { get; set; }

        /// <summary>
        /// 备用日期5
        /// </summary>
        [SugarColumn(ColumnName = "DT5")]
        public System.DateTime Dt5 { get; set; }
    }
}
