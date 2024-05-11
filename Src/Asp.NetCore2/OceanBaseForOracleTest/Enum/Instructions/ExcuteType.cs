using System.ComponentModel;

namespace xTPLM.RFQ.Common.Enum
{
    /// <summary>
    /// 指令执行关联类型
    /// </summary>
    public enum ExcuteType
    {
        /// <summary>
        /// 执行人
        /// </summary>
        [Description("执行人")]
        User = 1,

        /// <summary>
        /// 执行部门
        /// </summary>
        [Description("执行部门")]
        Department = 2
    }
}
