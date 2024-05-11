using System.ComponentModel;

namespace xTPLM.RFQ.Common
{
    /// <summary>
    /// 是否利率债
    /// </summary>
    public enum RatesEnum
    {
        /// <summary>
        /// 未划分
        /// </summary>
        [Description("未划分")]
        None = 0,

        /// <summary>
        /// 信用债
        /// </summary>
        [Description("信用债")]
        Credit = 1,

        /// <summary>
        /// 利率债
        /// </summary>
        [Description("利率债")]
        Rate = 2
    }
}
