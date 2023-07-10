using Newtonsoft.Json;
using SqlSugar;
using SqlSugar.Extensions;
using System;

namespace OrmTest
{
    /// <summary>
    ///数据实体对象
    /// @author 作者:曹伟
    /// @date 2023-02-13 18:02:23
    /// </summary>
    [SugarTable("Emp_License")]
    public class EmpLicense
    {
        /// <summary>
        /// 描述 :主键
        /// 空值 : false 
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnName = "Id")]
        public long Id { get; set; }
        /// <summary>
        /// 描述 :姓名
        /// 空值 : true 
        /// </summary>
        [SugarColumn(ColumnName = "Name")]
        public string? Name { get; set; }
        /// <summary>
        /// 描述 :身份证号码
        /// 空值 : true 
        /// </summary>
        [SugarColumn(ColumnName = "IdentityCard")]
        public string? IdentityCard { get; set; }
        /// <summary>
        /// 描述 :证照类别
        /// 空值 : true 
        /// </summary>
        [SugarColumn(ColumnName = "LicenseClassify")]
        public long? LicenseClassify { get; set; }
        /// <summary>
        /// 描述 :证照名称
        /// 空值 : true 
        /// </summary>
        [SugarColumn(ColumnName = "LicenseValue")]
        public string? LicenseValue { get; set; }
        /// <summary>
        /// 描述 :证照编号
        /// 空值 : true 
        /// </summary>
        [SugarColumn(ColumnName = "LicenseCode")]
        public string? LicenseCode { get; set; }
        /// <summary>
        /// 描述 :证照所属
        /// 空值 : true 
        /// </summary>
        [SugarColumn(ColumnName = "LicenseBelong")]
        public int? LicenseBelong { get; set; }
        /// <summary>
        /// 描述 :初证日期
        /// 空值 : true 
        /// </summary>
        [SugarColumn(ColumnName = "InitialDate")]
        public DateTime? InitialDate { get; set; }
        /// <summary>
        /// 描述 :复证日期
        /// 空值 : true 
        /// </summary>
        [SugarColumn(ColumnName = "RepeatDate")]
        public DateTime? RepeatDate { get; set; }
        /// <summary>
        /// 描述 :有效日期
        /// 空值 : true 
        /// </summary>
        [SugarColumn(ColumnName = "EffectiveDate")]
        public DateTime? EffectiveDate { get; set; }
        /// <summary>
        /// 描述 :核证日期
        /// 空值 : true 
        /// </summary>
        [SugarColumn(ColumnName = "VerifyDate")]
        public DateTime? VerifyDate { get; set; }
        /// <summary>
        /// 描述 :原件位置
        /// 空值 : true 
        /// </summary>
        [SugarColumn(ColumnName = "Location")]
        public string? Location { get; set; }
        /// <summary>
        /// 描述 :状态
        /// 空值 : true 
        /// </summary>
        [SugarColumn(ColumnName = "Status")]
        public int? Status { get; set; }
        /// <summary>
        /// 描述 :创建人id
        /// 空值 : true 
        /// </summary>
        [SugarColumn(ColumnName = "OperateById")]
        public long? OperateById { get; set; }
        /// <summary>
        /// 描述 :创建人姓名
        /// 空值 : true 
        /// </summary>
        [SugarColumn(ColumnName = "OperateBy")]
        public string? OperateBy { get; set; }
        /// <summary>
        /// 描述 :创建时间
        /// 空值 : true 
        /// </summary>
        [SugarColumn(ColumnName = "OperateTime")]
        public DateTime? OperateTime { get; set; }
        /// <summary>
        /// 描述 :创建人id
        /// 空值 : true 
        /// </summary>
        [SugarColumn(ColumnName = "CreateById", IsOnlyIgnoreUpdate = true)]
        public long? CreateById { get; set; }
        /// <summary>
        /// 描述 :创建人姓名
        /// 空值 : true 
        /// </summary>
        [SugarColumn(ColumnName = "CreateBy", IsOnlyIgnoreUpdate = true)]
        public string? CreateBy { get; set; }
        /// <summary>
        /// 描述 :创建时间
        /// 空值 : true 
        /// </summary>
        [SugarColumn(ColumnName = "CreateTime", IsOnlyIgnoreUpdate = true)]
        public DateTime? CreateTime { get; set; }
        /// <summary>
        /// 描述 :最后修改人id
        /// 空值 : true 
        /// </summary>
        [SugarColumn(ColumnName = "LastUpdateById", IsOnlyIgnoreInsert = true)]
        public long? LastUpdateById { get; set; }
        /// <summary>
        /// 描述 :最后修改人
        /// 空值 : true 
        /// </summary>
        [SugarColumn(ColumnName = "LastUpdateBy", IsOnlyIgnoreInsert = true)]
        public string? LastUpdateBy { get; set; }
        /// <summary>
        /// 描述 :最后修改时间
        /// 空值 : true 
        /// </summary>
        [SugarColumn(ColumnName = "LastUpdateTime", IsOnlyIgnoreInsert = true)]
        public DateTime? LastUpdateTime { get; set; }

        [Navigate(NavigateType.OneToOne, nameof(IdentityCard), nameof(OrmTest.EmpInformation.EmpIdentityCard))] //自定义关系映射
        public EmpInformation? EmpInformation { get; set; } //只能是null 不能赋默认值


        //证照注销信息
        [Navigate(NavigateType.OneToOne, nameof(Id), nameof(OrmTest.EmpLicenseLogOff.LicenseId))]//
        public EmpLicenseLogOff? EmpLicenseLogOff { get; set; }

    }
}