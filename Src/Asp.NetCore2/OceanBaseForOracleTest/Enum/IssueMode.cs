using System.ComponentModel;

namespace xTPLM.RFQ.Common.Enum
{
    /// <summary>
    /// 发行方式
    /// </summary>
    public enum IssueMode
    {
        /// <summary>
        /// 公开发行
        /// </summary>
        [Description("公开发行")]
        OpenIssue = 0,

        /// <summary>
        /// 定向发行
        /// </summary>
        [Description("定向发行")]
        OrientationIssue = 1
    }
}
