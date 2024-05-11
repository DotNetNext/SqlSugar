using System.ComponentModel;

namespace xTPLM.RFQ.Common
{
    public enum DefaultStatus
    {
        /// <summary>
        /// 正常
        /// </summary>
        [Description("正常")]
        Normal = 0,

        /// <summary>
        /// 兑息
        /// </summary>
        [Description("兑息")]
        PayInterest = 1,

        /// <summary>
        /// 兑付
        /// </summary>
        [Description("兑付")]
        Mtr = 2,

        /// <summary>
        /// 回售
        /// </summary>
        [Description("回售")]
        SellBack = 3,

        /// <summary>
        /// 还本付息
        /// </summary>
        [Description("还本付息")]
        ReturnCorpusPayInterest = 4,

        /// <summary>
        /// 其他违约  TT14433 2020-05-19 张景辉 
        /// </summary>
        [Description("其他违约")]
        Other = 5,

        /// <summary>
        /// 非标违约
        /// </summary>
        /// <remarks>2020-12-08 丁信丽 P008XIR-17685 非标违约</remarks>
        [Description("非标违约")]
        CashLBDebt = 11,
    }
}
