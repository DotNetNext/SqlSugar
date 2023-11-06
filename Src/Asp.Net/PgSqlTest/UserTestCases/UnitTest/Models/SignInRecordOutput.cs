using System;
 

public class SignInRecordOutput
{

    /// <summary>
    /// 用户Id
    /// </summary>
    public long UserId { get; set; }
    /// <summary>
    /// 用户
    /// </summary>
    public SysUser sysUser { get; set; }

    /// <summary>
    /// 需签到次数
    /// </summary>
    public long SignInNeedCount { get; set; }
    /// <summary>
    /// 签到次数
    /// </summary>
    public long SignInCount { get; set; }
    /// <summary>
    /// 迟到次数
    /// </summary>
    public long LateCount { get; set; }
    /// <summary>
    /// 请假次数
    /// </summary>
    public long LeaveCount { get; set; }
    /// <summary>
    /// 出勤率
    /// </summary>
    public double AttendanceRate { get; set; }
}
