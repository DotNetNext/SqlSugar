using SqlSugar;
using SqlSugar.Extensions;

namespace OrmTest
{
    /// <summary>
    ///数据实体对象
    /// @author 作者:曹伟
    /// @date 2022-12-28 19:37:41
    /// </summary>
    [SugarTable("Emp_Department_Job")]
    public class EmpDepartmentJob
    {
        /// <summary>
        /// 描述 :主键
        /// 空值 : false          /// </summary>
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnName = "Id")]
        public long Id { get; set; }
        /// <summary>
        /// 描述 :人员id
        /// 空值 : true 
        /// </summary>
        [SugarColumn(ColumnName = "EmpId")]
        public long? EmpId { get; set; }
        /// <summary>
        /// 描述 :部门id
        /// 空值 : true 
        /// </summary>
        [SugarColumn(ColumnName = "DepId")]
        public long? DepId { get; set; }
        /// <summary>
        /// 描述 :岗位id
        /// 空值 : true 
        /// </summary>
        [SugarColumn(ColumnName = "JobId")]
        public long? JobId { get; set; }
        /// <summary>
        /// 描述 :是否主部门
        /// 空值 : true 
        /// </summary>
        [SugarColumn(ColumnName = "Active")]
        public bool? Active { get; set; }

        /// <summary>
        /// 描述 :是否主部门
        /// 空值 : false 
        /// </summary>
        [SugarColumn(ColumnName = "IsSelect")]
        public bool? IsSelect { get; set; }

    }
}