using System.ComponentModel; 

/// <summary>
/// 签到结果枚举
/// </summary>
[Description("签到结果枚举")]
public enum SignInResultEnum
{
    /// <summary>
    /// 未签到
    /// </summary>
    [Description("未签到")]
    None = 0,

    /// <summary>
    /// 已签到
    /// </summary>
    [Description("已签到")]
    SignedIn = 1,

    /// <summary>
    /// 迟到
    /// </summary>
    [Description("迟到")]
    Late = 2,

    /// <summary>
    /// 请假
    /// </summary>
    [Description("请假")]
    Leave = 3,
}