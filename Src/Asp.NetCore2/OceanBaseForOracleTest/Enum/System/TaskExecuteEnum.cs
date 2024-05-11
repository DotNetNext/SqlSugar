using System.ComponentModel;

namespace xTPLM.RFQ.Common.Enum.System
{
    /// <summary>
    /// 计划任务执行状态
    /// </summary>
    public enum TaskExecuteEnum
    {
        /// <summary>
        /// 未执行
        /// </summary>
        [Description("未执行")]
        None = 0,

        /// <summary>
        /// 执行中
        /// </summary>
        [Description("执行中")]
        Executing = 1
    }
}
