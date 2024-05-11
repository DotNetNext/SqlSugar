using System.ComponentModel;

namespace xTPLM.RFQ.Common.Enum
{
    /// <summary>
    /// 模块类型
    /// </summary>
    public enum ModuleType
    {
        /// <summary>
        /// 菜单
        /// </summary>
        [Description("菜单")]
        Menu = 1,

        /// <summary>
        /// 按钮
        /// </summary>
        [Description("按钮")]
        Btn = 2,

        /// <summary>
        /// 虚拟权限
        /// </summary>
        [Description("虚拟权限")]
        VirtualPermissions = 99
    }
}
