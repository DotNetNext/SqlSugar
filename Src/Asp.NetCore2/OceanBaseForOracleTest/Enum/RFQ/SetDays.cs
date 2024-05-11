using System.ComponentModel;

namespace xTPLM.RFQ.Common
{
    /// <summary>
    /// 清算速度
    /// </summary>
    public enum SetDays
    {
        /// <summary>
        /// T+0
        /// </summary>
        [Description("T+0")]
        T0 = 0,

        /// <summary>
        /// T+1
        /// </summary>
        [Description("T+1")]
        T1 = 1,
    }
}
