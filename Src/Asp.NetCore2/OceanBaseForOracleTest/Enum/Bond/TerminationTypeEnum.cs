using System.ComponentModel;

namespace xTPLM.RFQ.Common
{
    /// <summary>
    /// 提前兑付类型
    /// </summary>
    public enum TerminationTypeEnum
    {
        /// <summary>
        /// 未知类型
        /// </summary>
        [Description("未知类型")]
        EarlyTerminationUnknown = 0,

        /// <summary>
        /// 提前兑付
        /// </summary>
        [Description("提前兑付")]
        Prepayment = 1,

        /// <summary>
        /// 提前赎回
        /// </summary>
        [Description("提前赎回")]
        Redemption = 2,
    }
}
