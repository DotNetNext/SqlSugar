using SqlSugar;
using System;

namespace OrmTest
{

    /// <summary>
    /// 供应商付款核销记录表
    ///</summary>
    [SugarTable("bil_supplierbalancerecord", "供应商付款核销记录表")]
    public partial class BilSupplierbalancerecord
    {
        /// <summary>
        /// 自增ID 
        ///</summary>
        [SugarColumn(ColumnName = "sbr_id", IsPrimaryKey = true, IsIdentity = true, ColumnDescription = "自增ID")]
        public long Id { get; set; }

        /// <summary>
        /// 应付费用ID,对应bil_payment表的pm_id 
        ///</summary>
        [SugarColumn(ColumnName = "pm_id", ColumnDescription = "应付费用ID,对应bil_payment表的pm_id")]
        public long? PmId { get; set; }

        /// <summary>
        /// 种类代码(Q信用额度，S金额)，目前只有S 
        ///</summary>
        [SugarColumn(ColumnName = "ipf_code", ColumnDescription = "种类代码(Q信用额度，S金额)，目前只有S")]
        public string IpfCode { get; set; }

        /// <summary>
        /// 供应商付款ID,对应stm_suppliercurrent表的sc_id 
        ///</summary>
        [SugarColumn(ColumnName = "sbr_payablefundid", ColumnDescription = "供应商付款ID,对应stm_suppliercurrent表的sc_id")]
        public long? SbrPayablefundid { get; set; }

        /// <summary>
        /// 核销金额  
        ///</summary>
        [SugarColumn(ColumnName = "sbr_amount", ColumnDescription = "核销金额 ")]
        public decimal? SbrAmount { get; set; }

        /// <summary>
        /// 核销日期 
        ///</summary>
        [SugarColumn(ColumnName = "sbr_createdate", ColumnDescription = "核销日期")]
        public DateTime CreateTime{get;set;}

        /// <summary>
        /// 核销人 
        ///</summary>
        [SugarColumn(ColumnName = "st_id_create", ColumnDescription = "核销人")]
        public long CreateBy { get; set; }

        /// <summary>
        /// 记录相互冲抵的sbr_id,相互冲抵的应付费用不新建付款来核销了，直接核销 
        ///</summary>
        [SugarColumn(ColumnName = "sbr_offsetid", ColumnDescription = "记录相互冲抵的sbr_id,相互冲抵的应付费用不新建付款来核销了，直接核销")]
        public long? SbrOffsetid { get; set; }

        /// <summary>
        /// 核销金额(原币) 
        ///</summary>
        [SugarColumn(ColumnName = "sbr_amount_ocur", ColumnDescription = "核销金额(原币)")]
        public decimal? SbrAmountOcur { get; set; }

    }
}