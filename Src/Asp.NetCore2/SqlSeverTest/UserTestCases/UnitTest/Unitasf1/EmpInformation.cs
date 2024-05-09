using SqlSugar;
using SqlSugar.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OrmTest
{
    /// <summary>
    ///数据实体对象
    /// @author 作者:曹伟
    /// @date 2022/11/3 15:30:19
    /// </summary>
    [SugarTable("Emp_Information")]
    public class EmpInformation
    {
        /// <summary>
        /// 描述 :主键
        /// 空值 : false          /// </summary>
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnName = "Id")]
        public long Id { get; set; }
        /// <summary>
        /// 描述 :姓名
        /// 空值 : true 
        /// </summary>
        [SugarColumn(ColumnName = "EmpName")]
        public string? EmpName { get; set; }
        /// <summary>
        /// 描述 :性别
        /// 空值 : true 
        /// </summary>
        [SugarColumn(ColumnName = "EmpSex")]
        public byte? EmpSex { get; set; }
        /// <summary>
        /// 描述 :身份证号码
        /// 空值 : true 
        /// </summary>
        [SugarColumn(ColumnName = "EmpIdentityCard")]
        public string? EmpIdentityCard { get; set; }
        /// <summary>
        /// 描述 :工作证号
        /// 空值 : true 
        /// </summary>
        [SugarColumn(ColumnName = "EmpWorkID")]
        public string? EmpWorkID { get; set; }
        /// <summary>
        /// 描述 :公司
        /// 空值 : true 
        /// </summary>
        [SugarColumn(ColumnName = "EmpCompanyId")]
        public long? EmpCompanyId { get; set; }
        /// <summary>
        /// 描述 :公司类型 本部1 2委外
        /// 空值 : true 
        /// </summary>
        [SugarColumn(ColumnName = "EmpCompanyType")]
        public string? EmpCompanyType { get; set; }
        /// <summary>
        /// 描述 :用工性质
        /// 空值 : true 
        /// </summary>
        [SugarColumn(ColumnName = "EmpNatureValue")]
        public string? EmpNatureValue { get; set; }
        /// <summary>
        /// 描述 :手机号码
        /// 空值 : true 
        /// </summary>
        [SugarColumn(ColumnName = "EmpPhoneNumber")]
        public string? EmpPhoneNumber { get; set; }
        /// <summary>
        /// 描述 :固定电话
        /// 空值 : true 
        /// </summary>
        [SugarColumn(ColumnName = "EmpFixPhoneNumber")]
        public string? EmpFixPhoneNumber { get; set; }
        /// <summary>
        /// 描述 :邮政编码
        /// 空值 : true 
        /// </summary>
        [SugarColumn(ColumnName = "EmpZipCode")]
        public string? EmpZipCode { get; set; }
        /// <summary>
        /// 描述 :家庭住址
        /// 空值 : true 
        /// </summary>
        [SugarColumn(ColumnName = "EmpHomeAddress")]
        public string? EmpHomeAddress { get; set; }
        /// <summary>
        /// 描述 :户籍地址
        /// 空值 : true 
        /// </summary>
        [SugarColumn(ColumnName = "EmpResidenceAddress")]
        public string? EmpResidenceAddress { get; set; }
        /// <summary>
        /// 描述 :所属街道
        /// 空值 : true 
        /// </summary>
        [SugarColumn(ColumnName = "EmpStreet")]
        public string? EmpStreet { get; set; }
        /// <summary>
        /// 描述 :民族
        /// 空值 : true 
        /// </summary>
        [SugarColumn(ColumnName = "EmpNationValue")]
        public string? EmpNationValue { get; set; }
        /// <summary>
        /// 描述 :政治面貌
        /// 空值 : true 
        /// </summary>
        [SugarColumn(ColumnName = "EmpPoliticalValue")]
        public string? EmpPoliticalValue { get; set; }
        /// <summary>
        /// 描述 :账号类型
        /// 空值 : true 
        /// </summary>
        [SugarColumn(ColumnName = "EmpAccountTypeValue")]
        public string? EmpAccountTypeValue { get; set; }
        /// <summary>
        /// 描述 :状态 emp_action_status在职 emp_leave_status离职
        /// 空值 : true 
        /// </summary>
        [SugarColumn(ColumnName = "EmpStatusType")]
        public string? EmpStatusType { get; set; }
        /// <summary>
        /// 描述 :状态关系
        /// 空值 : true 
        /// </summary>
        [SugarColumn(ColumnName = "EmpStatusValue")]
        public string? EmpStatusValue { get; set; }
        /// <summary>
        /// 描述 :是否机关各部门老大
        /// 空值 : true 
        /// </summary>
        [SugarColumn(ColumnName = "IsJBoss")]
        public byte? IsJBoss { get; set; }
        /// <summary>
        /// 描述 :
        /// 空值 : true 
        /// </summary>
        [SugarColumn(ColumnName = "CreateById")]
        public long? CreateById { get; set; }
        /// <summary>
        /// 描述 :
        /// 空值 : true 
        /// </summary>
        [SugarColumn(ColumnName = "CreateBy")]
        public string? CreateBy { get; set; }
        /// <summary>
        /// 描述 :
        /// 空值 : true 
        /// </summary>
        [SugarColumn(ColumnName = "CreateTime")]
        public DateTime? CreateTime { get; set; }
        /// <summary>
        /// 描述 :
        /// 空值 : true 
        /// </summary>
        [SugarColumn(ColumnName = "LastUpdateById")]
        public long? LastUpdateById { get; set; }
        /// <summary>
        /// 描述 :
        /// 空值 : true 
        /// </summary>
        [SugarColumn(ColumnName = "LastUpdateBy")]
        public string? LastUpdateBy { get; set; }
        /// <summary>
        /// 描述 :
        /// 空值 : true 
        /// </summary>
        [SugarColumn(ColumnName = "LastUpdateTime")]
        public DateTime? LastUpdateTime { get; set; }

        [Navigate(NavigateType.OneToMany, nameof(EmpDepartmentJob.EmpId))]//部门岗位表中的EmpId
        public List<EmpDepartmentJob>? EmpDepartmentJobs { get; set; }//注意禁止给手动赋值

        [Navigate(NavigateType.OneToMany, nameof(EmpLicense.IdentityCard), nameof(EmpIdentityCard))]//部门岗位表中的EmpId
        public List<EmpLicense>? EmpLicenses { get; set; }//注意禁止给手动赋值

    }
}