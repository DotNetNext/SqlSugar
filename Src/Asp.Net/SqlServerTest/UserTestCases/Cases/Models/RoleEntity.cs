using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugarDemo
{
    /// <summary>
    /// 角色信息表
    /// </summary>
    [SugarTable("Unit_SYS_Role")]
    public partial class RoleEntity
    {

        /// <summary>
        /// 角色Id
        /// </summary>	
        [SugarColumn(IsPrimaryKey = true)]
        public Guid RoleId { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>	
        public string RoleName { get; set; }

        /// <summary>
        /// 角色类型|0-系统超管角色|1-系统普通角色|2-功能组|
        /// </summary>	
        public int RoleType { get; set; }

        /// <summary>
        /// 价格
        /// </summary>	
        public decimal? UnitPrice { get; set; }

        /// <summary>
        /// 数量
        /// </summary>	
        public int? Quantity { get; set; }

        /// <summary>
        /// 排序
        /// </summary>	
        public int? SortNum { get; set; }

        /// <summary>
        /// 所属账号(账号表Id)
        /// </summary>	
        public Guid ManageAccount { get; set; }

        /// <summary>
        /// 所属机构(账号所属公司的一级机构)
        /// </summary>	
        public Guid ManageOrg { get; set; }

        /// <summary>
        /// 账号所属组织
        /// </summary>
        public Guid OrganizationId { get; set; }
    }


}
