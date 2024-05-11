using System.ComponentModel;

namespace xTPLM.RFQ.Common.Enum.RFQ
{
    /// <summary>
    /// 融资途径
    /// </summary>
    public enum FinancingWay
    {
        [Description("待定")]
        None,

        [Description("质押式正回购")]
        PledgeRepo,

        [Description("买断式正回购")]
        BuySellRepo,

        [Description("同业拆入")]
        Lend,
    }
}
