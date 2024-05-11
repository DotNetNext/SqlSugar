using System.ComponentModel;

namespace xTPLM.RFQ.Common
{
    /// <summary>
    /// 价格类型:1-到期收益率  2-行权收益率 3-净价 4-全价
    /// </summary>
    public enum PriceTypeEnum
    {
        /// <summary>
        /// 到期收益率
        /// </summary>
        [Description("到期收益率")]
        YTM = 1,

        /// <summary>
        /// 行权收益率
        /// </summary>
        [Description("行权收益率")]
        YTM_OE = 2,

        /// <summary>
        /// 净价
        /// </summary>
        [Description("净价")]
        NETPRICE = 3,

        /// <summary>
        /// 全价
        /// </summary>
        [Description("全价")]
        PRICE = 4,
    }
}
