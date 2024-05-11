using System.ComponentModel;

namespace xTPLM.RFQ.Common.Enum.RFQ
{
    public enum RepoTradeType
    {
        /// <summary>
        /// 质押式正回购
        /// </summary>
        [Description("质押式正回购")]
        RepoPlus = 40,

        /// <summary>
        /// 质押式逆回购
        /// </summary>
        [Description("质押式逆回购")]
        RepoMinus = 41
    }
}
