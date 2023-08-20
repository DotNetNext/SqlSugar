using SqlSugar;
using System;
 

/// <summary>
/// 系统用户表
/// </summary>
[SugarTable(null, "系统用户表")]
public class SysUser
{
    /// <summary>
    /// 雪花Id
    /// </summary>
    [SugarColumn(ColumnDescription = "Id", IsPrimaryKey = true, IsIdentity = false)]
    public virtual long Id { get; set; }

    /// <summary>
    /// 真实姓名
    /// </summary>
    [SugarColumn(ColumnDescription = "真实姓名", Length = 32)]
    public virtual string RealName { get; set; }

    /// <summary>
    /// 昵称
    /// </summary>
    [SugarColumn(ColumnDescription = "昵称", Length = 32)]
    public string  NickName { get; set; }

    /// <summary>
    /// 头像
    /// </summary>
    [SugarColumn(ColumnDescription = "头像", Length = 512)]
    public string  Avatar { get; set; }

    /// <summary>
    /// 年龄
    /// </summary>
    [SugarColumn(ColumnDescription = "年龄")]
    public int Age { get; set; }

    /// <summary>
    /// 出生日期
    /// </summary>
    [SugarColumn(ColumnDescription = "出生日期")]
    public DateTime? Birthday { get; set; }

    /// <summary>
    /// 民族
    /// </summary>
    [SugarColumn(ColumnDescription = "民族", Length = 32)]
    public string  Nation { get; set; }

    /// <summary>
    /// 手机号码
    /// </summary>
    [SugarColumn(ColumnDescription = "手机号码", Length = 16)]
    public string  Phone { get; set; }


    /// <summary>
    /// 身份证号
    /// </summary>
    [SugarColumn(ColumnDescription = "身份证号", Length = 32)]
    public string  IdCardNum { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    [SugarColumn(ColumnDescription = "邮箱", Length = 64)]
    public string  Email { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    [SugarColumn(ColumnDescription = "地址", Length = 256)]
    public string  Address { get; set; }

    /// <summary>
    /// 政治面貌
    /// </summary>
    [SugarColumn(ColumnDescription = "政治面貌", Length = 16)]
    public string  PoliticalOutlook { get; set; }

    /// <summary>
    /// 毕业院校
    /// </summary>COLLEGE
    [SugarColumn(ColumnDescription = "毕业院校", Length = 128)]
    public string  College { get; set; }

    /// <summary>
    /// 办公电话
    /// </summary>
    [SugarColumn(ColumnDescription = "办公电话", Length = 16)]
    public string  OfficePhone { get; set; }

    /// <summary>
    /// 紧急联系人
    /// </summary>
    [SugarColumn(ColumnDescription = "紧急联系人", Length = 32)]
    public string  EmergencyContact { get; set; }

    /// <summary>
    /// 紧急联系人电话
    /// </summary>
    [SugarColumn(ColumnDescription = "紧急联系人电话", Length = 16)]
    public string  EmergencyPhone { get; set; }

    /// <summary>
    /// 紧急联系人地址
    /// </summary>
    [SugarColumn(ColumnDescription = "紧急联系人地址", Length = 256)]
    public string  EmergencyAddress { get; set; }

    /// <summary>
    /// 个人简介
    /// </summary>
    [SugarColumn(ColumnDescription = "个人简介", Length = 512)]
    public string  Introduction { get; set; }

}