using System.ComponentModel;

namespace xTPLM.RFQ.Common.Enum
{
    /// <summary>
    /// 额度类型
    /// </summary>
    public enum LimitResultType
    {
        /// <summary>
        /// 申请额度
        /// </summary>
        [Description("申请额度")]
        Company = 1,

        /// <summary>
        /// 剩余额度
        /// </summary>
        [Description("剩余额度")]
        ISSUER = 2
    }
}
