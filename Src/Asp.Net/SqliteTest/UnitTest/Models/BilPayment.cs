using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace SqliteTest.UnitTest.Models
{
    /// <summary>
    /// 应付费用表，记录与服务商之间的应收/应付费用。
    ///</summary>
    [SugarTable("bil_payment", "应付费用表，记录与服务商之间的应收/应付费用。")]
    public partial class BilPayment
    {
        /// <summary>
        /// 应付费用id 
        ///</summary>
        [SugarColumn(ColumnName = "pm_id", IsPrimaryKey = true, IsIdentity = true, ColumnDescription = "应付费用id")]
        public long Id { get; set; }

        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "tms_id", ColumnDescription = "")]
        public int? TmsId { get; set; }

        /// <summary>
        /// 业务id 
        ///</summary>
        [SugarColumn(ColumnName = "bs_id", ColumnDescription = "业务id")]
        public long BsId { get; set; }

        /// <summary>
        /// 服务商id 
        ///</summary>
        [SugarColumn(ColumnName = "server_id", ColumnDescription = "服务商id")]
        public long ServerId { get; set; }

        /// <summary>
        /// 费用类型 
        ///</summary>
        [SugarColumn(ColumnName = "fk_code", ColumnDescription = "费用类型")]
        public string FkCode { get; set; }

        /// <summary>
        /// 计费单位 
        ///</summary>
        [SugarColumn(ColumnName = "unit_code", ColumnDescription = "计费单位")]
        public string UnitCode { get; set; }

        /// <summary>
        /// 金额，负数表示服务商退回费用 
        ///</summary>
        [SugarColumn(ColumnName = "pm_amount", ColumnDescription = "金额，负数表示服务商退回费用")]
        public decimal PmAmount { get; set; }

        /// <summary>
        /// 原币种 
        ///</summary>
        [SugarColumn(ColumnName = "currency_code", ColumnDescription = "原币种")]
        public string CurrencyCode { get; set; }

        /// <summary>
        /// 汇率 
        ///</summary>
        [SugarColumn(ColumnName = "pm_currencyrate", ColumnDescription = "汇率")]
        public decimal PmCurrencyrate { get; set; }

        /// <summary>
        /// 本位币金额 
        ///</summary>
        [SugarColumn(ColumnName = "pm_currencyamount", ColumnDescription = "本位币金额")]
        public decimal? PmCurrencyamount { get; set; }

 
    
        /// <summary>
        /// 核销完成标志，y表示本费用被核销  
        ///</summary>
        [SugarColumn(ColumnName = "pm_writeoffsign", ColumnDescription = "核销完成标志，y表示本费用被核销 ")]
        public string PmWriteoffsign { get; set; }

        /// <summary>
        /// 应付费用付清时间 
        ///</summary>
        [SugarColumn(ColumnName = "pm_writeoffdate", ColumnDescription = "应付费用付清时间")]
        public DateTime? PmWriteoffdate { get; set; }

        /// <summary>
        /// 费用发生日期。快件指出货日期，其它指入机日期 
        ///</summary>
        [SugarColumn(ColumnName = "pd_occurdate", ColumnDescription = "费用发生日期。快件指出货日期，其它指入机日期")]
        public DateTime PdOccurdate { get; set; }

        /// <summary>
        /// 计费价格表id 
        ///</summary>
        [SugarColumn(ColumnName = "pd_pricesheetid", ColumnDescription = "计费价格表id")]
        public long? PdPricesheetid { get; set; }

        /// <summary>
        /// 计费价格值id 
        ///</summary>
        [SugarColumn(ColumnName = "pv_pricevalueid", ColumnDescription = "计费价格值id")]
        public long? PvPricevalueid { get; set; }

        /// <summary>
        /// 计费分区 
        ///</summary>
        [SugarColumn(ColumnName = "pd_zoneid", ColumnDescription = "计费分区")]
        public long? PdZoneid { get; set; }

        /// <summary>
        /// 备注 
        ///</summary>
        [SugarColumn(ColumnName = "pd_note", ColumnDescription = "备注")]
        public string PdNote { get; set; }

        /// <summary>
        /// 服务商余额顺序ID 
        ///</summary>
        [SugarColumn(ColumnName = "pm_server_surplusorderid", ColumnDescription = "服务商余额顺序ID")]
        public long? PmServerSurplusorderid { get; set; }

        /// <summary>
        /// 对账标志，Y为已完成服务商金额对账，N为未完成对账 
        ///</summary>
        [SugarColumn(ColumnName = "servebill_checksign", ColumnDescription = "对账标志，Y为已完成服务商金额对账，N为未完成对账")]
        public string ServebillChecksign { get; set; }

        /// <summary>
        /// 服务商对账时间 
        ///</summary>
        [SugarColumn(ColumnName = "servebill_checkdate", ColumnDescription = "服务商对账时间")]
        public DateTime? ServebillCheckdate { get; set; }

        /// <summary>
        /// 服务商账单ID 
        ///</summary>
        [SugarColumn(ColumnName = "servebill_id", ColumnDescription = "服务商账单ID")]
        public long? ServebillId { get; set; }

        /// <summary>
        /// 导入时的服务商账单ID 
        ///</summary>
        [SugarColumn(ColumnName = "import_servebill_id", ColumnDescription = "导入时的服务商账单ID")]
        public long? ImportServebillId { get; set; }

        /// <summary>
        /// 费用冲抵id 
        ///</summary>
        [SugarColumn(ColumnName = "offset_pm_id", ColumnDescription = "费用冲抵id")]
        public long? OffsetPmId { get; set; }

        /// <summary>
        /// 成本费用分摊id 
        ///</summary>
        [SugarColumn(ColumnName = "share_id", ColumnDescription = "成本费用分摊id")]
        public long? ShareId { get; set; }

        /// <summary>
        /// 应付账单id 
        ///</summary>
        [SugarColumn(ColumnName = "paymentbill_id", ColumnDescription = "应付账单id")]
        public long? PaymentbillId { get; set; }

        /// <summary>
        /// 供应商账单ID 
        ///</summary>
        [SugarColumn(ColumnName = "supplierbill_id", ColumnDescription = "供应商账单ID")]
        public long? SupplierbillId { get; set; }

        /// <summary>
        /// 差异原因，基础表：bsd_servebilldifference_reason(人工导入的费用，标记与系统自动算出的费用的差异原因) 
        ///</summary>
        [SugarColumn(ColumnName = "difference_reason", ColumnDescription = "差异原因，基础表：bsd_servebilldifference_reason(人工导入的费用，标记与系统自动算出的费用的差异原因)")]
        public string DifferenceReason { get; set; }

    }
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("bil_costshare", "")]
    public partial class BilCostshare
    {
        /// <summary>
        /// 分摊自增长ID 
        ///</summary>
        [SugarColumn(ColumnName = "share_id", IsPrimaryKey = true, IsIdentity = true, ColumnDescription = "分摊自增长ID")]
        public long Id { get; set; }

        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "tms_id", ColumnDescription = "")]
        public int? TmsId { get; set; }

        /// <summary>
        /// 组织机构 
        ///</summary>
        [SugarColumn(ColumnName = "og_id", ColumnDescription = "组织机构")]
        public long? OgId { get; set; }

        /// <summary>
        /// 状态 A待审核 B待分摊 C已分摊 D已删除 
        ///</summary>
        [SugarColumn(ColumnName = "share_status", ColumnDescription = "状态 A待审核 B待分摊 C已分摊 D已删除")]
        public string ShareStatus { get; set; }

        /// <summary>
        /// 付款状态,N-未付款,Y-已付款 
        ///</summary>
        [SugarColumn(ColumnName = "pay_status", ColumnDescription = "付款状态,N-未付款,Y-已付款")]
        public string PayStatus { get; set; }

        /// <summary>
        /// 航空主单号 
        ///</summary>
        [SugarColumn(ColumnName = "airmaster_number", ColumnDescription = "航空主单号")]
        public string AirmasterNumber { get; set; }

        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "fk_code", ColumnDescription = "")]
        public string FkCode { get; set; }

        /// <summary>
        /// 金额（总费用） 
        ///</summary>
        [SugarColumn(ColumnName = "master_amount", ColumnDescription = "金额（总费用）")]
        public decimal MasterAmount { get; set; }

        /// <summary>
        /// 币种 
        ///</summary>
        [SugarColumn(ColumnName = "master_currency", ColumnDescription = "币种")]
        public string MasterCurrency { get; set; }

        /// <summary>
        /// 汇率 
        ///</summary>
        [SugarColumn(ColumnName = "master_rate", ColumnDescription = "汇率")]
        public decimal? MasterRate { get; set; }

        /// <summary>
        /// 主单计费重 
        ///</summary>
        [SugarColumn(ColumnName = "master_weight", ColumnDescription = "主单计费重")]
        public string MasterWeight { get; set; }

        /// <summary>
        /// 服务商 
        ///</summary>
        [SugarColumn(ColumnName = "server_id", ColumnDescription = "服务商")]
        public long ServerId { get; set; }

        /// <summary>
        /// 差异原因，分摊时需要导入到应付费用明细表 
        ///</summary>
        [SugarColumn(ColumnName = "difference_reason", ColumnDescription = "差异原因，分摊时需要导入到应付费用明细表")]
        public string DifferenceReason { get; set; }

      
        /// <summary>
        /// 审核人 
        ///</summary>
        [SugarColumn(ColumnName = "audit_st_id", ColumnDescription = "审核人")]
        public long? AuditStId { get; set; }

        /// <summary>
        /// 审核人时间 
        ///</summary>
        [SugarColumn(ColumnName = "audit_date", ColumnDescription = "审核人时间")]
        public DateTime? AuditDate { get; set; }

        /// <summary>
        /// 分摊人 
        ///</summary>
        [SugarColumn(ColumnName = "share_st_id", ColumnDescription = "分摊人")]
        public long? ShareStId { get; set; }

        /// <summary>
        /// 分摊时间 
        ///</summary>
        [SugarColumn(ColumnName = "share_date", ColumnDescription = "分摊时间")]
        public DateTime? ShareDate { get; set; }

        /// <summary>
        /// 分摊账单代码 
        ///</summary>
        [SugarColumn(ColumnName = "share_code", ColumnDescription = "分摊账单代码")]
        public string ShareCode { get; set; }

        /// <summary>
        /// 账单名称 
        ///</summary>
        [SugarColumn(ColumnName = "share_name", ColumnDescription = "账单名称")]
        public string ShareName { get; set; }

        /// <summary>
        /// 付款备注 
        ///</summary>
        [SugarColumn(ColumnName = "pay_note", ColumnDescription = "付款备注")]
        public string PayNote { get; set; }

        /// <summary>
        /// 付款审核人 
        ///</summary>
        [SugarColumn(ColumnName = "st_id_paycheck", ColumnDescription = "付款审核人")]
        public long? StIdPaycheck { get; set; }

        /// <summary>
        /// 付款审核时间 
        ///</summary>
        [SugarColumn(ColumnName = "paycheck_date", ColumnDescription = "付款审核时间")]
        public DateTime? PaycheckDate { get; set; }

        /// <summary>
        /// 分摊状态，Y正在分摊 
        ///</summary>
        [SugarColumn(ColumnName = "share_state", ColumnDescription = "分摊状态，Y正在分摊")]
        public string ShareState { get; set; }

        /// <summary>
        /// 账单所属期,到月,如2021-04 
        ///</summary>
        [SugarColumn(ColumnName = "servebill_period_date", ColumnDescription = "账单所属期,到月,如2021-04")]
        public string ServebillPeriodDate { get; set; }

        /// <summary>
        /// 分摊重量类型：A按出货实重分摊 B按分支计费重分摊 
        ///</summary>
        [SugarColumn(ColumnName = "share_weighttype", ColumnDescription = "分摊重量类型：A按出货实重分摊 B按分支计费重分摊")]
        public string ShareWeighttype { get; set; }

        /// <summary>
        /// 暂估回填费用ID，对应bil_estimate_backfill表主键 
        ///</summary>
        [SugarColumn(ColumnName = "backfill_id", ColumnDescription = "暂估回填费用ID，对应bil_estimate_backfill表主键")]
        public long? BackfillId { get; set; }

        /// <summary>
        /// 来源：A暂估回填界面添加 B成本费用分摊 
        ///</summary>
        [SugarColumn(ColumnName = "receive_from", ColumnDescription = "来源：A暂估回填界面添加 B成本费用分摊")]
        public string ReceiveFrom { get; set; }

        /// <summary>
        /// 暂估回填审核人 
        ///</summary>
        [SugarColumn(ColumnName = "backfillaudit_st_id", ColumnDescription = "暂估回填审核人")]
        public long? BackfillauditStId { get; set; }

        /// <summary>
        /// 暂估回填审核时间 
        ///</summary>
        [SugarColumn(ColumnName = "backfillaudit_date", ColumnDescription = "暂估回填审核时间")]
        public DateTime? BackfillauditDate { get; set; }

        /// <summary>
        /// 网络类型 
        ///</summary>
        [SugarColumn(ColumnName = "servertype_code", ColumnDescription = "网络类型")]
        public string ServertypeCode { get; set; }

        /// <summary>
        /// 对账有差异时先更新此字段，对账完成才更新servebill_id字段 
        ///</summary>
        [SugarColumn(ColumnName = "import_servebill_id", ColumnDescription = "对账有差异时先更新此字段，对账完成才更新servebill_id字段")]
        public long? ImportServebillId { get; set; }

        /// <summary>
        /// 服务商账单ID，对应bil_servebill表主键 
        ///</summary>
        [SugarColumn(ColumnName = "servebill_id", ColumnDescription = "服务商账单ID，对应bil_servebill表主键")]
        public long? ServebillId { get; set; }

        /// <summary>
        /// 是否也分摊到分公司成本费用 
        ///</summary>
        [SugarColumn(ColumnName = "share_internalcost", ColumnDescription = "是否也分摊到分公司成本费用")]
        public long? ShareInternalcost { get; set; }

        /// <summary>
        /// 成本归属 
        ///</summary>
        [SugarColumn(ColumnName = "pm_og_id", ColumnDescription = "成本归属")]
        public long? PmOgId { get; set; }
    }
}
