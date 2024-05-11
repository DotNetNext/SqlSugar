using System.ComponentModel;

namespace xTPLM.RFQ.Common.Enum.RFQ
{
    /// <summary>
    /// 交易对手类型
    /// </summary>
    public enum PartyType
    {
        /// <summary>
        /// 法人户
        /// </summary>
        [Description("法人户")]
        LegalPerson = 1,

        /// <summary>
        /// 非法人户
        /// </summary>
        [Description("非法人户")]
        NotLegalPerson = 2
    }
}
