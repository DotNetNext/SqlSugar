using System.ComponentModel;

namespace xTPLM.RFQ.Common.Enum.RFQ
{
    /// <summary>
    /// 一级询价交易方向
    /// </summary>
    public enum QuotaPrimaryTradeType
    {
        /// <summary>
        /// 分销买入
        /// </summary>
        [Description("分销买入")]
        DistributionBuy = 61,

        /// <summary>
        /// 分销卖出
        /// </summary>
        [Description("分销卖出")]
        DistributionSell = 62
    }
}
