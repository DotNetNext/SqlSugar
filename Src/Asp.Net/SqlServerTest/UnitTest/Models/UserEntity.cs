using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugarDemo
{

    /// <summary>
    /// 系统账号表
    /// </summary>
    [SugarTable("Unit_SYS_User")]
    public partial class UserEntity
    {

        /// <summary>
        /// 系统账号Id
        /// </summary>	
        [SugarColumn(IsPrimaryKey = true)]
        public Guid UserId { get; set; }

        /// <summary>
        /// 自定义账号
        /// </summary>	
        public string UserAccount { get; set; }

        /// <summary>
        /// 手机账号
        /// </summary>	
        public string PhoneAccount { get; set; }

        /// <summary>
        /// 邮箱账号
        /// </summary>	
        public string EmailAccount { get; set; }

        /// <summary>
        /// 企业微信
        /// </summary>	
        public string CompanyWX { get; set; }

        /// <summary>
        /// 密码凭证
        /// </summary>	
        public string Credential { get; set; }

        /// <summary>
        /// 账号类型|100401-平台管理员|100402-平台用户|100403-平台OpenId|100404-租户管理员|100405-租户用户|100406-租户OpenId|100407-游客
        /// </summary>	
        public int UserType { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>	
        public string NickName { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public int Sex { get; set; }

        /// <summary>
        /// 有效开始时间
        /// </summary>	
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 有效截止时间
        /// </summary>	
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 是否强制修改密码
        /// </summary>	
        public bool IsChangePassword { get; set; }

        /// <summary>
        /// 安全手机号
        /// </summary>	
        public string SafePhone { get; set; }

        /// <summary>
        /// 是否实名|0-否|1-是
        /// </summary>	
        public int IsReal { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>	
        public string RealName { get; set; }

        /// <summary>
        /// 身份证号
        /// </summary>	
        public string CardNo { get; set; }

        /// <summary>
        /// 最后登录时间
        /// </summary>	
        public DateTime? LastLoginDate { get; set; }

        /// <summary>
        /// 失败登录次数
        /// </summary>	
        public int? FailedLoginPwdCount { get; set; }

        /// <summary>
        /// 失败登录时间
        /// </summary>	
        public DateTime? VerificationLoginPwdDate { get; set; }

        /// <summary>
        /// 禁止用户登录时间
        /// </summary>	
        public DateTime? StopLoginTime { get; set; }

        /// <summary>
        /// 所属账号(账号表Id)
        /// </summary>	
        public Guid ManageAccount { get; set; }

        /// <summary>
        /// 所属机构(账号所属公司的一级机构)
        /// </summary>	
        public Guid ManageOrg { get; set; }

        /// <summary>
        /// 用户角色
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public List<UserRoleEntity> UserRoleEntities { get; set; }

        /// <summary>
        /// 角色主表ID集合
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public List<RoleEntity> RoleEntities { get; set; }
    }
}
