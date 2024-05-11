using System.ComponentModel;

namespace xTPLM.RFQ.Common.Enum
{
    public enum ContactType
    {
        /// <summary>
        /// QQ
        /// </summary>
        [Description("QQ")]
        QQ = 10,

        /// <summary>
        /// QTrade
        /// </summary>
        [Description("QTrade")]
        QTrade = 20,

        /// <summary>
        /// 电话
        /// </summary>
        [Description("电话")]
        Phone = 30,

        /// <summary>
        /// 邮箱
        /// </summary>
        [Description("邮箱")]
        Email = 40
    }
}
