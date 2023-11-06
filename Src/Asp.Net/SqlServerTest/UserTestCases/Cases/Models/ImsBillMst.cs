 
using SqlSugar;

namespace IWMS.Bill.Models
{
    /// <summary>
    /// 单据主表
    /// </summary>
    [SugarTable("IMS_BILL_MST")]
    public class ImsBillMst  
    {
        /// <summary>
        /// 单据编号
        /// </summary>
        [SugarColumn(ColumnName = "CODE")]
        public string Code { get; set; }

        /// <summary>
        /// 合并到的单据
        /// </summary>
        [SugarColumn(ColumnName = "COMBINED_TO")]
        public string CombinedTo { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        [SugarColumn(ColumnName = "DESCRIPTION")]
        public string Description { get; set; }

        /// <summary>
        /// 抬头文本
        /// </summary>
        [SugarColumn(ColumnName = "HEADER_TEXT")]
        public string HeaderText { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [SugarColumn(ColumnName = "STATUS")]
        public string Status { get; set; }

        /// <summary>
        /// 单据来源渠道(1:WMS  2: ERP  3:SRM)
        /// </summary>
        [SugarColumn(ColumnName = "CHANNEL")]
        public decimal Channel { get; set; }

        /// <summary>
        /// 目的地
        /// </summary>
        [SugarColumn(ColumnName = "DESTINATION")]
        public string Destination { get; set; }

        /// <summary>
        /// 单据类型
        /// </summary>
        [SugarColumn(ColumnName = "BILL_TYPE_ID")]
        public decimal BillTypeId { get; set; }

        /// <summary>
        /// 公司
        /// </summary>
        [SugarColumn(ColumnName = "COMPANY_ID")]
        public decimal CompanyId { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        [SugarColumn(ColumnName = "VENDOR_ID")]
        public decimal VendorId { get; set; }

        /// <summary>
        /// 客户
        /// </summary>
        [SugarColumn(ColumnName = "CUSTOMER_ID")]
        public decimal CustomerId { get; set; }

        /// <summary>
        /// 发票号
        /// </summary>
        [SugarColumn(ColumnName = "INVOICE_NO")]
        public string InvoiceNo { get; set; }

        /// <summary>
        /// 装箱单
        /// </summary>
        [SugarColumn(ColumnName = "PK_NO")]
        public string PkNo { get; set; }

        /// <summary>
        /// 装箱单类型
        /// </summary>
        [SugarColumn(ColumnName = "PK_TYPE")]
        public string PkType { get; set; }

        /// <summary>
        /// 保税类型
        /// </summary>
        [SugarColumn(ColumnName = "BONDED_TYPE")]
        public string BondedType { get; set; }

        /// <summary>
        /// 车间
        /// </summary>
        [SugarColumn(ColumnName = "FACTORY")]
        public string Factory { get; set; }

        /// <summary>
        /// 默认发产区域清单
        /// </summary>
        [SugarColumn(ColumnName = "COMMIT_LOCATORS")]
        public string CommitLocators { get; set; }

        /// <summary>
        /// ERP交易路线
        /// </summary>
        [SugarColumn(ColumnName = "ERP_BILL_TYPE")]
        public string ErpBillType { get; set; }

        /// <summary>
        /// ERP交易名称
        /// </summary>
        [SugarColumn(ColumnName = "ERP_BILL_NAME")]
        public string ErpBillName { get; set; }

        /// <summary>
        /// 工作中心
        /// </summary>
        [SugarColumn(ColumnName = "WORK_CENTER")]
        public string WorkCenter { get; set; }

        /// <summary>
        /// 部门
        /// </summary>
        [SugarColumn(ColumnName = "DEPARTMENT")]
        public string Department { get; set; }

        /// <summary>
        /// 到货日期
        /// </summary>
        [SugarColumn(ColumnName = "ETA_DATE")]
        public System.DateTime? EtaDate { get; set; }

        /// <summary>
        /// 司机
        /// </summary>
        [SugarColumn(ColumnName = "DRIVER_NAME")]
        public string DriverName { get; set; }

        /// <summary>
        /// 司机手机号
        /// </summary>
        [SugarColumn(ColumnName = "DRIVER_MOBILE")]
        public string DriverMobile { get; set; }

        /// <summary>
        /// 车牌号
        /// </summary>
        [SugarColumn(ColumnName = "CAR_NO")]
        public string CarNo { get; set; }

        /// <summary>
        /// 码头号
        /// </summary>
        [SugarColumn(ColumnName = "WHARF")]
        public string Wharf { get; set; }

        /// <summary>
        /// 送货方式
        /// </summary>
        [SugarColumn(ColumnName = "DLV_TYPE")]
        public string DlvType { get; set; }

        /// <summary>
        /// 报缺时间
        /// </summary>
        [SugarColumn(ColumnName = "SHORTAGE_TIME")]
        public System.DateTime? ShortageTime { get; set; }

        /// <summary>
        /// 车型
        /// </summary>
        [SugarColumn(ColumnName = "VEHICLE_TYPE")]
        public string VehicleType { get; set; }

        /// <summary>
        /// 箱数
        /// </summary>
        [SugarColumn(ColumnName = "BOX_COUNT")]
        public decimal BoxCount { get; set; }

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
        /// 打印人
        /// </summary>
        [SugarColumn(ColumnName = "PRINT_BY")]
        public string PrintBy { get; set; }

        /// <summary>
        /// 提单号
        /// </summary>
        [SugarColumn(ColumnName = "BILL_OF_LOAD")]
        public string BillOfLoad { get; set; }

        /// <summary>
        /// 出货通知单
        /// </summary>
        [SugarColumn(ColumnName = "SHIPPING_ORDER")]
        public string ShippingOrder { get; set; }

        /// <summary>
        /// 出货日期
        /// </summary>
        [SugarColumn(ColumnName = "SHIPPING_DATE")]
        public System.DateTime? ShippingDate { get; set; }

        /// <summary>
        /// 进厂时间
        /// </summary>
        [SugarColumn(ColumnName = "IN_DOOR_DATE")]
        public System.DateTime? InDoorDate { get; set; }

        /// <summary>
        /// 出厂时间
        /// </summary>
        [SugarColumn(ColumnName = "OUT_DOOR_DATE")]
        public System.DateTime? OutDoorDate { get; set; }

        /// <summary>
        /// 核准状态
        /// </summary>
        [SugarColumn(ColumnName = "SIGN_STATUS")]
        public string SignStatus { get; set; }

        /// <summary>
        /// 核准时间
        /// </summary>
        [SugarColumn(ColumnName = "SIGN_DATE")]
        public System.DateTime? SignDate { get; set; }

        /// <summary>
        /// 核准人
        /// </summary>
        [SugarColumn(ColumnName = "SIGN_BY")]
        public string SignBy { get; set; }

        /// <summary>
        /// 核准说明
        /// </summary>
        [SugarColumn(ColumnName = "SIGN_REMARK")]
        public string SignRemark { get; set; }

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
        public string Deleted { get; internal set; }
        public decimal Id { get; internal set; }
    }
}
