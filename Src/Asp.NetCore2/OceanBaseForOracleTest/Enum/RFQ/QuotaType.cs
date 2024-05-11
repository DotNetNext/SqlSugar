using System.ComponentModel;

namespace xTPLM.RFQ.Common.Enum.RFQ
{
    /// <summary>
    /// 询价业务类型
    /// </summary>
    public enum QuotaType
    {
        /// <summary>
        /// 现券询价
        /// </summary>
        [Description("现券询价")]
        BndQuota = 1,

        /// <summary>
        /// 一级询价
        /// </summary>
        [Description("一级询价")]
        PrimaryQuota = 2,

        /// <summary>
        /// 逆回购询价
        /// </summary>
        [Description("逆回购询价")]
        RepoQuota = 3,

        /// <summary>
        /// 正回购询价
        /// </summary>
        [Description("正回购询价")]
        FinancingQuota = 4,
    }
}
