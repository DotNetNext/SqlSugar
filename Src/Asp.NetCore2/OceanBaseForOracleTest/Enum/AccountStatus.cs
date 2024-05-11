using System.ComponentModel;

namespace xTPLM.RFQ.Common.Enum
{
    /// <summary>
    /// 账户状态
    /// </summary>
    public enum AccountStatus
    {
        /// <summary>
        /// 创建中
        /// </summary>
        [Description("创建中")]
        Opening = 0,

        /// <summary>
        /// 已启用
        /// </summary>
        [Description("已启用")]
        Opened = 1,

        /// <summary>
        /// 停用中
        /// </summary>
        [Description("停用中")]
        Closing = 2,

        /// <summary>
        /// 已停用
        /// </summary>
        [Description("已停用")]
        Closed = 3
    }
}
