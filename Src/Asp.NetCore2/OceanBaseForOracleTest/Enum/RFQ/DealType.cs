using System.ComponentModel;

namespace xTPLM.RFQ.Common
{
    /// <summary>
    /// 报价方式
    /// </summary>
    public enum DealType
    {
        /// <summary>
        /// 空
        /// </summary>
        [Description("空")]
        Empty = -1,

        /// <summary>
        /// 做市报价
        /// </summary>
        [Description("做市报价")]
        MarketQuotation = 0,

        /// <summary>
        /// 对话报价
        /// </summary>
        [Description("对话报价")]
        NegotiationQuotation = 2,

        /// <summary>
        /// 其它
        /// </summary>
        [Description("其它")]
        OtherQuotation = 3,

        /// <summary>
        /// 指定对手方报价
        /// </summary>
        [Description("指定对手方报价")]
        DesignatedCounterparty = 4,

        /// <summary>
        /// 询价
        /// </summary>
        [Description("询价")]
        InquiryQuotation = 5,

        /// <summary>
        /// 请求报价
        /// </summary>
        [Description("请求报价")]
        RequestQuotation = 6,

        /// <summary>
        /// 匿名报价
        /// </summary>
        [Description("匿名报价")]
        X_Bond = 25,

        /// <summary>
        /// 匹配成交
        /// </summary>
        [Description("匹配成交")]
        Match = 46,

        /// <summary>
        /// 协商成交
        /// </summary>
        [Description("协商成交")]
        Negotiation = 47,

        /// <summary>
        /// 询价成交
        /// </summary>
        /// <remarks>
        [Description("询价成交")]
        Enquiry = 49,
    }
}
