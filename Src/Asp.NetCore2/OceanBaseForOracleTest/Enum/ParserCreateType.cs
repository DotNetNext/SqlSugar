using System.ComponentModel;

namespace xTPLM.RFQ.Common
{
    /// <summary>
    /// 语料生成配置类型
    /// </summary>
    public enum ParserCreateType
    {
        [Description("询价指令")]
        Instructions = 1,
        [Description("资管现券询价指令")]
        AmcBondInstructions = 2,
    }
}
