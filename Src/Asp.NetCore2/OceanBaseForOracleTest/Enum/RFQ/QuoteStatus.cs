using System.ComponentModel;

namespace xTPLM.RFQ.Common.Enum
{
    /// <summary>
    /// 询价状态
    /// </summary>
    public enum QuoteStatus
    {
        /// <summary>
        /// 已撤销
        /// </summary>
        [Description("已撤销")]
        Revoke = -1,

        /// <summary>
        /// 新建
        /// </summary>
        [Description("新建")]
        Create = 0,

        /// <summary>
        /// 交谈中
        /// </summary>
        [Description("交谈中")]
        Chating = 5,

        /// <summary>
        /// 已确认
        /// </summary>
        [Description("已确认")]
        Confirmed = 10,

        /// <summary>
        /// 已下达
        /// </summary>
        [Description("已下达")]
        Release = 20
    }
}
