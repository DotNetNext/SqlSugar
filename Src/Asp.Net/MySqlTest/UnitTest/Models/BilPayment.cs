using SqlSugar;
using System;

namespace OrmTest
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
        public   long Id { get; set; }

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
}