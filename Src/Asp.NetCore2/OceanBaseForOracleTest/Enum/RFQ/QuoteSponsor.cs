using System.ComponentModel;

namespace xTPLM.RFQ.Common
{
    /// <summary>
    /// 询价发起方
    /// </summary>
    public enum QuoteSponsor
    {
        /// <summary>
        /// 本方发起
        /// </summary>
        [Description("本方发起")]
        This = 0,

        /// <summary>
        /// 对方发起
        /// </summary>
        [Description("对方发起")]
        Competitor = 1
    }
}
