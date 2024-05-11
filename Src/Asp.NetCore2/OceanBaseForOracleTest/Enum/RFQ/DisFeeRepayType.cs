using System.ComponentModel;

namespace xTPLM.RFQ.Common.Enum.RFQ
{
    /// <summary>
    /// 手续费返还方式
    /// </summary>
    public enum DisFeeRepayType
    {
        /// <summary>
        /// 不返
        /// </summary>
        [Description("不返")]
        None = 0,

        /// <summary>
        /// 单返
        /// </summary>
        [Description("单返")]
        SingleReturn = 1,

        /// <summary>
        /// 折价
        /// </summary>
        [Description("折价")]
        Convert = 2,

        /// <summary>
        /// 溢价
        /// </summary>
        [Description("溢价")]
        Premium = 3,
    }
}
