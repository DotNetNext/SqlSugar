using SqlSugar;
using SqlSugar.Extensions;
using System;

namespace OrmTest
{
    /// <summary>
    ///数据实体对象
    /// @author 作者:曹伟
    /// @date 2023/7/5 17:08:20
    /// </summary>
    [SugarTable("Emp_License_LogOff")]
    public class EmpLicenseLogOff
    {
        /// <summary>
        /// 描述 :主键
        /// 空值 : false          /// </summary>
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnName = "Id")]
        public long Id { get; set; }
        /// <summary>
        /// 描述 :关联自有证照id
        /// 空值 : false          /// </summary>
        [SugarColumn(ColumnName = "LicenseId")]
        public long LicenseId { get; set; }
        /// <summary>
        /// 描述 :证照注销原因
        /// 空值 : true 
        /// </summary>
        [SugarColumn(ColumnName = "LogOffReasonValue")]
        public string? LogOffReasonValue { get; set; }
        /// <summary>
        /// 描述 :备注
        /// 空值 : true 
        /// </summary>
        [SugarColumn(ColumnName = "Remarks")]
        public string? Remarks { get; set; }
        /// <summary>
        /// 描述 :
        /// 空值 : true 
        /// </summary>
        [SugarColumn(ColumnName = "Status")]
        public int? Status { get; set; }
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


        //委外证照信息
        [Navigate(NavigateType.OneToOne, nameof(LicenseId), nameof(OrmTest.EmpLicense.Id))]//
        public EmpLicense? EmpLicense { get; set; }

    }
}