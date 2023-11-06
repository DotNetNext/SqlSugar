
using SqlSugar;

namespace IWMS.Bill.Models
{
    /// <summary>
    /// 单据明细
    /// </summary>
    [SugarTable("IMS_BILL_DTL")]
    public class ImsBillDtl  
    {
        /// <summary>
        /// 主表ID
        /// </summary>
        [SugarColumn(ColumnName = "MST_ID")]
        public decimal MstId { get; set; }

        /// <summary>
        /// 来源主表ID
        /// </summary>
        [SugarColumn(ColumnName = "ORIG_MST_ID")]
        public decimal OrigMstId { get; set; }

        /// <summary>
        /// 行号
        /// </summary>
        [SugarColumn(ColumnName = "LINE_NO")]
        public decimal? LineNo { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(ColumnName = "DESCRIPTION")]
        public string Description { get; set; }

        /// <summary>
        /// 项目文本
        /// </summary>
        [SugarColumn(ColumnName = "ITEM_TEXT")]
        public string ItemText { get; set; }

        /// <summary>
        /// 料号ID
        /// </summary>
        [SugarColumn(ColumnName = "PART_ID")]
        public decimal? PartId { get; set; }

        /// <summary>
        /// 成品号
        /// </summary>
        [SugarColumn(ColumnName = "PID")]
        public string Pid { get; set; }

        /// <summary>
        /// 机芯
        /// </summary>
        [SugarColumn(ColumnName = "CORE")]
        public string Core { get; set; }

        /// <summary>
        /// 发出库别
        /// </summary>
        [SugarColumn(ColumnName = "FROM_SIC_ID")]
        public decimal? FromSicId { get; set; }

        /// <summary>
        /// 发出储位
        /// </summary>
        [SugarColumn(ColumnName = "FROM_LOC_ID")]
        public decimal? FromLocId { get; set; }

        /// <summary>
        /// 发出工厂
        /// </summary>
        [SugarColumn(ColumnName = "FROM_BU_ID")]
        public decimal? FromBuId { get; set; }

        /// <summary>
        /// 接收工厂
        /// </summary>
        [SugarColumn(ColumnName = "TO_BU_ID")]
        public decimal? ToBuId { get; set; }

        /// <summary>
        /// 接收仓库
        /// </summary>
        [SugarColumn(ColumnName = "TO_SIC_ID")]
        public decimal? ToSicId { get; set; }

        /// <summary>
        /// 接收储位
        /// </summary>
        [SugarColumn(ColumnName = "TO_LOC_ID")]
        public decimal? ToLocId { get; set; }

        /// <summary>
        /// 转换后料号
        /// </summary>
        [SugarColumn(ColumnName = "TO_PART_ID")]
        public decimal? ToPartId { get; set; }

        /// <summary>
        /// 总箱数
        /// </summary>
        [SugarColumn(ColumnName = "TOTAL_BOX")]
        public decimal? TotalBox { get; set; }

        /// <summary>
        /// 最小包装
        /// </summary>
        [SugarColumn(ColumnName = "MPQ")]
        public decimal? Mpq { get; set; }

        /// <summary>
        /// 开单量
        /// </summary>
        [SugarColumn(ColumnName = "QTY")]
        public decimal Qty { get; set; }

        /// <summary>
        /// 实际收发量
        /// </summary>
        [SugarColumn(ColumnName = "ACTUAL_QTY")]
        public decimal ActualQty { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [SugarColumn(ColumnName = "STATUS", IsOnlyIgnoreInsert = true)]
        public string Status { get; set; }

        /// <summary>
        /// 进向交货单
        /// </summary>
        [SugarColumn(ColumnName = "INNER_DN_NO")]
        public string InnerDnNo { get; set; }

        /// <summary>
        /// 账册序号
        /// </summary>
        [SugarColumn(ColumnName = "BOOK_NO")]
        public string BookNo { get; set; }

        /// <summary>
        /// 备案申请号
        /// </summary>
        [SugarColumn(ColumnName = "APP_NO")]
        public string AppNo { get; set; }

        /// <summary>
        /// 采购单
        /// </summary>
        [SugarColumn(ColumnName = "PO_NO")]
        public string PoNo { get; set; }

        /// <summary>
        /// 采购单行号
        /// </summary>
        [SugarColumn(ColumnName = "PO_LINE")]
        public string PoLine { get; set; }

        /// <summary>
        /// 贸易类型
        /// </summary>
        [SugarColumn(ColumnName = "ZTYPE")]
        public string Ztype { get; set; }

        /// <summary>
        /// 采购单位
        /// </summary>
        [SugarColumn(ColumnName = "PO_UNIT")]
        public string PoUnit { get; set; }

        /// <summary>
        /// 库存单位
        /// </summary>
        [SugarColumn(ColumnName = "BASE_UNIT")]
        public string BaseUnit { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        [SugarColumn(ColumnName = "UNIT_PRICE")]
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 币别
        /// </summary>
        [SugarColumn(ColumnName = "CURRENCY")]
        public string Currency { get; set; }

        /// <summary>
        /// 工单号
        /// </summary>
        [SugarColumn(ColumnName = "MO")]
        public string Mo { get; set; }

        /// <summary>
        /// 工单行号
        /// </summary>
        [SugarColumn(ColumnName = "MO_LINE")]
        public string MoLine { get; set; }

        /// <summary>
        /// 出通单号
        /// </summary>
        [SugarColumn(ColumnName = "DN_NO")]
        public string DnNo { get; set; }

        /// <summary>
        /// 出通单行号
        /// </summary>
        [SugarColumn(ColumnName = "DN_LINE")]
        public string DnLine { get; set; }

        /// <summary>
        /// 销单号
        /// </summary>
        [SugarColumn(ColumnName = "SO")]
        public string So { get; set; }

        /// <summary>
        /// 销单行号
        /// </summary>
        [SugarColumn(ColumnName = "SO_LINE")]
        public string SoLine { get; set; }

        /// <summary>
        /// 销售国
        /// </summary>
        [SugarColumn(ColumnName = "SO_CONTRY")]
        public string SoContry { get; set; }

        /// <summary>
        /// 预留单号
        /// </summary>
        [SugarColumn(ColumnName = "RSV_NO")]
        public string RsvNo { get; set; }

        /// <summary>
        /// 预留单行号
        /// </summary>
        [SugarColumn(ColumnName = "RSV_LINE")]
        public string RsvLine { get; set; }

        /// <summary>
        /// 装箱单号
        /// </summary>
        [SugarColumn(ColumnName = "PK_NO")]
        public string PkNo { get; set; }

        /// <summary>
        /// 装箱单项次
        /// </summary>
        [SugarColumn(ColumnName = "PK_LINE")]
        public string PkLine { get; set; }

        /// <summary>
        /// 凭证号
        /// </summary>
        [SugarColumn(ColumnName = "EVIDENCE")]
        public string Evidence { get; set; }

        /// <summary>
        /// 凭证行
        /// </summary>
        [SugarColumn(ColumnName = "EVIDENCE_LINE")]
        public string EvidenceLine { get; set; }

        /// <summary>
        /// 凭证年份
        /// </summary>
        [SugarColumn(ColumnName = "EVIDENCE_YEAR")]
        public string EvidenceYear { get; set; }

        /// <summary>
        /// 科目
        /// </summary>
        [SugarColumn(ColumnName = "ACCOUNT")]
        public string Account { get; set; }

        /// <summary>
        /// 成本中心
        /// </summary>
        [SugarColumn(ColumnName = "COST_CENTER")]
        public string CostCenter { get; set; }

        /// <summary>
        /// 反冲标识
        /// </summary>
        [SugarColumn(ColumnName = "BACK_FLUSH")]
        public string BackFlush { get; set; }

        /// <summary>
        /// 配送目的地
        /// </summary>
        [SugarColumn(ColumnName = "DESTINATION")]
        public string Destination { get; set; }

        /// <summary>
        /// 移动原因
        /// </summary>
        [SugarColumn(ColumnName = "MOVE_REASON")]
        public string MoveReason { get; set; }

        /// <summary>
        /// 卸货点
        /// </summary>
        [SugarColumn(ColumnName = "UNLOADING_POINT")]
        public string UnloadingPoint { get; set; }

        /// <summary>
        /// 加工方式
        /// </summary>
        [SugarColumn(ColumnName = "PROCESS_WAY")]
        public string ProcessWay { get; set; }

        /// <summary>
        /// 串号
        /// </summary>
        [SugarColumn(ColumnName = "ARTNO")]
        public string Artno { get; set; }

        /// <summary>
        /// 报关单号
        /// </summary>
        [SugarColumn(ColumnName = "ENTRYID")]
        public string Entryid { get; set; }

        /// <summary>
        /// 报关单行号
        /// </summary>
        [SugarColumn(ColumnName = "GNO")]
        public string Gno { get; set; }

        /// <summary>
        /// 供应商发票号
        /// </summary>
        [SugarColumn(ColumnName = "VD_INV_NO")]
        public string VdInvNo { get; set; }

        /// <summary>
        /// 香港发票号
        /// </summary>
        [SugarColumn(ColumnName = "HK_INV_NO")]
        public string HkInvNo { get; set; }

        /// <summary>
        /// 供应商编号
        /// </summary>
        [SugarColumn(ColumnName = "VENDOR_CODE")]
        public string VendorCode { get; set; }

        /// <summary>
        /// 航班号
        /// </summary>
        [SugarColumn(ColumnName = "FLIGHT_NO")]
        public string FlightNo { get; set; }

        /// <summary>
        /// 运输方式
        /// </summary>
        [SugarColumn(ColumnName = "SHIP_WAY")]
        public string ShipWay { get; set; }

        /// <summary>
        /// 货柜号
        /// </summary>
        [SugarColumn(ColumnName = "CONTAINER")]
        public string Container { get; set; }

        /// <summary>
        /// 柜型
        /// </summary>
        [SugarColumn(ColumnName = "CONTAINER_TYPE")]
        public string ContainerType { get; set; }

        /// <summary>
        /// 货柜尺寸
        /// </summary>
        [SugarColumn(ColumnName = "CONTAINER_SIZE")]
        public string ContainerSize { get; set; }

        /// <summary>
        /// 封条号
        /// </summary>
        [SugarColumn(ColumnName = "SEAL_NO")]
        public string SealNo { get; set; }

        /// <summary>
        /// 厂封
        /// </summary>
        [SugarColumn(ColumnName = "F_SEAL_NO")]
        public string FSealNo { get; set; }

        /// <summary>
        /// 套件值
        /// </summary>
        [SugarColumn(ColumnName = "GROUP_KEY")]
        public string GroupKey { get; set; }

        /// <summary>
        /// 急料标记
        /// </summary>
        [SugarColumn(ColumnName = "URGENT_FLAG")]
        public string UrgentFlag { get; set; }

        /// <summary>
        /// 试产标记
        /// </summary>
        [SugarColumn(ColumnName = "TRIAL_FLAG")]
        public string TrialFlag { get; set; }

        /// <summary>
        /// 开始处理人
        /// </summary>
        [SugarColumn(ColumnName = "START_BY")]
        public string StartBy { get; set; }

        /// <summary>
        /// 开始处理时间
        /// </summary>
        [SugarColumn(ColumnName = "START_DATE")]
        public System.DateTime? StartDate { get; set; }

        /// <summary>
        /// 结单人员
        /// </summary>
        [SugarColumn(ColumnName = "CLOSED_BY")]
        public string ClosedBy { get; set; }

        /// <summary>
        /// 结单时间
        /// </summary>
        [SugarColumn(ColumnName = "CLOSED_DATE")]
        public System.DateTime? ClosedDate { get; set; }

        /// <summary>
        /// 过账时间
        /// </summary>
        [SugarColumn(ColumnName = "BOOK_DATE")]
        public System.DateTime? BookDate { get; set; }

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
        /// 扩展6
        /// </summary>
        [SugarColumn(ColumnName = "ATT6")]
        public string Att6 { get; set; }

        /// <summary>
        /// 扩展7
        /// </summary>
        [SugarColumn(ColumnName = "ATT7")]
        public string Att7 { get; set; }

        /// <summary>
        /// 扩展8
        /// </summary>
        [SugarColumn(ColumnName = "ATT8")]
        public string Att8 { get; set; }

        /// <summary>
        /// 扩展9
        /// </summary>
        [SugarColumn(ColumnName = "ATT9")]
        public string Att9 { get; set; }

        /// <summary>
        /// 扩展10
        /// </summary>
        [SugarColumn(ColumnName = "ATT10")]
        public string Att10 { get; set; }

        /// <summary>
        /// 备用数值1
        /// </summary>
        [SugarColumn(ColumnName = "NUM1")]
        public decimal? Num1 { get; set; }

        /// <summary>
        /// 备用数值2
        /// </summary>
        [SugarColumn(ColumnName = "NUM2")]
        public decimal? Num2 { get; set; }

        /// <summary>
        /// 备用数值3
        /// </summary>
        [SugarColumn(ColumnName = "NUM3")]
        public decimal? Num3 { get; set; }

        /// <summary>
        /// 备用数值4
        /// </summary>
        [SugarColumn(ColumnName = "NUM4")]
        public decimal? Num4 { get; set; }

        /// <summary>
        /// 备用数值5
        /// </summary>
        [SugarColumn(ColumnName = "NUM5")]
        public decimal? Num5 { get; set; }

        /// <summary>
        /// 备用日期1
        /// </summary>
        [SugarColumn(ColumnName = "DT1")]
        public System.DateTime? Dt1 { get; set; }

        /// <summary>
        /// 备用日期2
        /// </summary>
        [SugarColumn(ColumnName = "DT2")]
        public System.DateTime? Dt2 { get; set; }

        /// <summary>
        /// 备用日期3
        /// </summary>
        [SugarColumn(ColumnName = "DT3")]
        public System.DateTime? Dt3 { get; set; }

        /// <summary>
        /// 备用日期4
        /// </summary>
        [SugarColumn(ColumnName = "DT4")]
        public System.DateTime? Dt4 { get; set; }

        /// <summary>
        /// 备用日期5
        /// </summary>
        [SugarColumn(ColumnName = "DT5")]
        public System.DateTime? Dt5 { get; set; }
        public decimal Id { get; internal set; }
    }
}
