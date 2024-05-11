using System.ComponentModel;

namespace xTPLM.RFQ.Common.Enum
{
    /// <summary>
    /// 指令类型
    /// </summary>
    public enum InstructionsType
    {
        /// <summary>
        /// 精确指令
        /// </summary>
        [Description("精确指令")]
        Accurate = 1,


        /// <summary>
        /// 模糊指令
        /// </summary>
        [Description("模糊指令")]
        Vague = 2,
    }
}
