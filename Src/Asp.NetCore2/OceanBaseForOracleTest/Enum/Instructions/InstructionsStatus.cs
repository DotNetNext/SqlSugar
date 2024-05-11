using System.ComponentModel;

namespace xTPLM.RFQ.Common.Enum
{
    /// <summary>
    /// 指令状态
    /// </summary>
    public enum InstructionsStatus
    {
        /// <summary>
        /// 作废
        /// </summary>
        [Description("作废")]
        Close = -2,

        /// <summary>
        /// 审批不通过
        /// </summary>
        [Description("审批不通过")]
        ApprovalFail = -1,

        /// <summary>
        /// 新建
        /// </summary>
        [Description("新建")]
        Create = 0,

        /// <summary>
        /// 提交中
        /// </summary>
        [Description("提交中")]
        Submiting = 3,

        /// <summary>
        /// 审批中
        /// </summary>
        [Description("审批中")]
        Approval = 5,

        /// <summary>
        /// 审批通过
        /// </summary>
        [Description("审批通过")]
        Approved = 10,

        /// <summary>
        /// 已完成
        /// </summary>
        [Description("已完成")]
        Complete = 20,

    }
}
