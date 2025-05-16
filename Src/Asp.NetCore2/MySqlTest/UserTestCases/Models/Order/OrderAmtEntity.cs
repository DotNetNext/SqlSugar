using System;
using System.Collections.Generic;
using ERP1.Entity;
using SqlSugar;

namespace ERP1.Entity
{
    [Serializable]
    [SugarTable("t_orderamt")]
    public class OrderAmtEntity
    {
        #region # 数据库字段 #

        /// <summary>订单单号</summary>
        public string SaleNo { set; get; }

        /// <summary>类型</summary>
        public string Type { set; get; }

        /// <summary>异常款项单号</summary>
        public string? AbMoneyNo { set; get; }

        /// <summary>售后单号</summary>
        public string? ServiceNo { set; get; }

        /// <summary>收款方式</summary>
        public string? PaymentMethod { set; get; }

        /// <summary>实收币种</summary>
        public string? PaymentCurrency { set; get; }

        /// <summary>实收金额</summary>
        public Decimal? PaymentAmt { set; get; }

        /// <summary>收款流水号</summary>
        public string? PaymentNo { set; get; }

        /// <summary>收款备注</summary>
        public string? PaymentRemark { set; get; }

        /// <summary>创建时间</summary>
        public DateTime AddTime { set; get; }

        #endregion

        /// <summary>收款方式名称</summary>
        [SugarColumn(IsIgnore = true)]
        public string? PaymentMethodName { set; get; }

        /// <summary>实收币种名称</summary>
        [SugarColumn(IsIgnore = true)]
        public string? PaymentCurrencyName { set; get; }

        ///// <summary>订单信息</summary>
        [SugarColumn(IsIgnore = true)]
        public OrderEntity OrderEntity { get; set; }
    }
}
