using System.ComponentModel;

namespace xTPLM.RFQ.Common.Enum
{
    /// <summary>
    /// 报价来源
    /// </summary>
    public enum BiddDetailFromSource
    {
        /// <summary>
        /// 手动录入
        /// </summary>
        [Description("手动录入")]
        Manual = 0,

        /// <summary>
        /// QTrade
        /// </summary>
        [Description("QTrade")]
        QTrade = 1
    }
}
