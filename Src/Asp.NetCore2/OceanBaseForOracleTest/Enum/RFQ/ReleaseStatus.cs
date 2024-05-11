using System.ComponentModel;

namespace xTPLM.RFQ.Common.Enum
{
    /// <summary>
    /// 询价下达状态  0:未下达，5:下达中，10:下达成功，-1:下达失败
    /// </summary>
    public enum ReleaseStatus
    {
        /// <summary>
        /// 未下达
        /// </summary>
        [Description("未下达")]
        NoneRelease = 0,

        /// <summary>
        /// 下达中
        /// </summary>
        [Description("下达中")]
        Releaseing = 5,

        /// <summary>
        /// 下达成功
        /// </summary>
        [Description("下达成功")]
        Released = 10,

        /// <summary>
        /// 下达失败
        /// </summary>
        [Description("下达失败")]
        ReleaseFail = -1
    }
}
