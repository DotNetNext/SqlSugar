using SqlSugar;
using System;
 

/// <summary>
/// 签到表
/// </summary>
[SugarTable(null, "签到表")]
public class SignInRecord
{

    /// <summary>
    /// 报名用户Id
    /// </summary>
    [SugarColumn(ColumnDescription = "用户Id")]
    public long UserId { get; set; }

    /// <summary>
    /// 系统用户
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    [Navigate(NavigateType.OneToOne, nameof(UserId))]
    public SysUser sysUser { get; set; }

    /// <summary>
    /// 签到日期
    /// </summary>
    [SugarColumn(ColumnDescription = "签到日期")]
    public DateTime SignInDate { get; set; }
    /// <summary>
    /// 上午签到时间
    /// </summary>
    [SugarColumn(ColumnDescription = "上午签到时间")]
    public DateTime? MorningSignInTime { get; set; }
    /// <summary>
    /// 上午签到地点
    /// </summary>
    [SugarColumn(ColumnDescription = "上午签到地点", Length = 64)]
    public string MorningSignInAddress { get; set; }
    /// <summary>
    /// 上午签到结果
    /// </summary>
    [SugarColumn(ColumnDescription = "上午签到结果")]
    public SignInResultEnum MorningSignInResult { get; set; } = 0;
    /// <summary>
    /// 下午签到时间
    /// </summary>
    [SugarColumn(ColumnDescription = "下午签到时间")]
    public DateTime? AfternoonSignInTime { get; set; }
    /// <summary>
    /// 下午签到地点
    /// </summary>
    [SugarColumn(ColumnDescription = "下午签到地点", Length = 64)]
    public string  AfternoonSignInAddress { get; set; }
    /// <summary>
    /// 下午签到结果
    /// </summary>
    [SugarColumn(ColumnDescription = "下午签到结果")]
    public SignInResultEnum AfternoonSignInResult { get; set; } = 0;

}