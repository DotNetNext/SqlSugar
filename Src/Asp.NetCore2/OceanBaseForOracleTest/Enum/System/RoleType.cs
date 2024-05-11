using System.ComponentModel;

namespace xTPLM.RFQ.Common
{
    /// <summary>
    /// 角色类型
    /// </summary>
    public enum RoleType
    {
        /// <summary>
        /// 组员
        /// </summary>
        [Description("组员")]
        Staff = 1,

        /// <summary>
        /// 组长
        /// </summary>
        [Description("组长")]
        Leader = 2
    }
}
