using System.ComponentModel;

namespace xTPLM.RFQ.Common.Enum
{
    /// <summary>
    /// 交易系统状态
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>
        /// 新建
        /// </summary>
        [Description("新建")]
        Created = 0,

        /// <summary>
        /// 审批中
        /// </summary>
        [Description("审批中")]
        Ordered = 1,

        /// <summary>
        /// 交易错误
        /// </summary>
        [Description("交易错误")]
        Error = 2,

        /// <summary>
        /// 返回失败
        /// </summary>
        [Description("返回失败")]
        Failed = 3,

        /// <summary>
        /// 审批拒绝
        /// </summary>
        [Description("审批拒绝")]
        Rejected = 4,

        /// <summary>
        /// 审批通过
        /// </summary>
        [Description("审批通过")]
        Confirmed = 5,

        /// <summary>
        /// 该笔委托没有相应的成功撤单操作且成交数量小于委托数量
        /// </summary>
        [Description("该笔委托没有相应的成功撤单操作且成交数量小于委托数量")]
        PartDealed = 6,

        /// <summary>
        /// 成交确认
        /// </summary>
        [Description("成交确认")]
        FullDealed = 7,

        /// <summary>
        /// 撤单操作成功，成交数量小于委托数量
        /// </summary>
        [Description("撤单操作成功，成交数量小于委托数量")]
        PartWithdrawed = 8,

        /// <summary>
        /// 交易撤单
        /// </summary>
        [Description("交易撤单")]
        FullWithdrawed = 9,

        /// <summary>
        /// 风险预审中
        /// </summary>
        [Description("风险预审中")]
        PreCheckRisk = -3,

        /// <summary>
        /// 交易执行中
        /// </summary>
        [Description("交易执行中")]
        Executing = -4,

        /// <summary>
        /// 交易终止
        /// </summary>
        [Description("交易终止")]
        Terminated = 10
    }
}
