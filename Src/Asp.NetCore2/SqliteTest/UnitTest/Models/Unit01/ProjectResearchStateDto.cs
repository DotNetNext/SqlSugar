using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlugarDemo
{
    public enum ProjectResearchStateDto : int
    {
        /// <summary>
        /// 正在调研
        /// </summary>
        [Description("调研中")]
        RESEARCHING = 0,
        /// <summary>
        /// 完成调研
        /// </summary>
        [Description("调研完成")]
        FINISH_RESEARCH = 1
    }

    public enum RessearchReplyType
    {
        /// <summary>
        /// 接受
        /// </summary>
        [Description("接受")]
        Accept,

        /// <summary>
        /// 拒绝
        /// </summary>
        [Description("拒绝")]
        Reject,

        /// <summary>
        /// 未回复
        /// </summary>
        [Description("未回复")]
        NO_REPLY,

        /// <summary>
        /// 已过期
        /// </summary>
        [Description("已过期")]
        EXPIRED,
    }
}
