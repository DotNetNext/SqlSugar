using System.ComponentModel;

namespace xTPLM.RFQ.Common.Enum
{
    public enum ActionType
    {
        /// <summary>
        /// 修改指令额度
        /// </summary>
        [Description("修改指令额度")]
        EditInstructionOrderMoney = 1,

        /// <summary>
        /// 指令撤回审批
        /// </summary>
        [Description("指令撤回审批")]
        InstructionRevoke = 2
    }
}
