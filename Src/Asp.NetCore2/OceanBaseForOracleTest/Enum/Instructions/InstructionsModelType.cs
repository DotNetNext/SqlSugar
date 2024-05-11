using System.ComponentModel;

namespace xTPLM.RFQ.Common.Enum
{
    /// <summary>
    /// 指令类型
    /// </summary>
    public enum InstructionsModelType
    {
        /// <summary>
        /// 现券指令
        /// </summary>
        [Description("现券指令")]
        Bond = 0,

        /// <summary>
        /// 一级指令
        /// </summary>
        [Description("一级指令")]
        Primary = 1
    }
}
