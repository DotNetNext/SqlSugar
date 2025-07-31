using System.ComponentModel.DataAnnotations;
using SqlSugar;

namespace WebApplication4
{
    /// <summary>
    /// 账号信息
    /// </summary>
    [SugarTable("account")]
    public class Account
    {
        /// <summary>
        /// 账号Id
        /// </summary>
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        /// 登录账号
        /// </summary>
        [SugarColumn(ColumnName = "user_name", Length = 50)]
        public string UserName { get; set; }

        /// <summary>
        /// 登录密码
        /// </summary>
        [SugarColumn(ColumnName = "password", Length = 50)]
        public string Password { get; set; }
        
    }

    /// <summary>
    /// 账号角色类型
    /// </summary>
    public enum RoleType
    {
        [Display(Name = "匿名")]
        Anonymous = -1,

        [Display(Name = "管理员")]
        Admin = 0,
        
        [Display(Name = "客服")]
        Employee = 1,

        [Display(Name = "企业")]
        Company = 2,

        [Display(Name = "坐席")]
        User = 3,

        [Display(Name = "渠道")]
        Partner = 4,

        [Display(Name = "代理")]
        Agent = 5,
    }
}