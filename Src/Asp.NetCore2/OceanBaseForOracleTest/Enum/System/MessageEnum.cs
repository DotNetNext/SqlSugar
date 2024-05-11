using System.ComponentModel;

namespace xTPLM.RFQ.Common
{
    /// <summary>
    /// 消息推送类型
    /// </summary>
    public enum MessagePushType
    {
        /// <summary>
        /// 消息盒子
        /// </summary>
        [Description("消息盒子")]
        MessageBox = 0,

        /// <summary>
        /// 弹窗消息
        /// </summary>
        [Description("弹窗消息")]
        MessagePushNotify = 1
    }

    /// <summary>
    /// 消息业务类型
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// 系统通知
        /// </summary>
        [Description("系统通知")]
        System = 0,


        /// <summary>
        /// 策略匹配
        /// </summary>
        [Description("策略匹配")]
        Strategy = 1
    }

    /// <summary>
    /// 消息重要等级
    /// </summary>
    public enum MessageLevel
    {
        /// <summary>
        /// 普通
        /// </summary>
        [Description("普通")]
        Ordinary = 0,

        /// <summary>
        /// 重要
        /// </summary>
        [Description("普通")]
        Important = 10,

        /// <summary>
        /// 紧急
        /// </summary>
        [Description("紧急")]
        Urgent = 20,
    }

    /// <summary>
    /// 消息状态
    /// </summary>
    public enum MessageStatus
    {
        /// <summary>
        /// 未读
        /// </summary>
        [Description("未读")]
        Unread = 0,

        /// <summary>
        /// 已读
        /// </summary>
        [Description("已读")]
        Read = 1,
    }
}
