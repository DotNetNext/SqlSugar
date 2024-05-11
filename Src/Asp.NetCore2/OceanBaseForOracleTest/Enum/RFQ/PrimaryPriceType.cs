using System.ComponentModel;

namespace xTPLM.RFQ.Common.Enum.RFQ
{
    /// <summary>
    /// 一级询价价格类型
    /// </summary>
    public enum PrimaryPriceType
    {
        /// <summary>
        /// 分销价格
        /// </summary>
        [Description("分销价格")]
        ORDPRICE = 0,

        /// <summary>
        /// 到期收益率
        /// </summary>
        [Description("到期收益率")]
        BND_YTM = 1,

        /// <summary>
        /// 票面利率
        /// </summary>
        [Description("票面利率")]
        COUPON = 2,
    }
}
