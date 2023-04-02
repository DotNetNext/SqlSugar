// **************************************
// 生成：CodeBuilder (http://www.fireasy.cn/codebuilder)
// 项目：非机动车车库系统
// 版权：Copyright WWB
// 作者：WWB
// 时间：02/23/2023 10:28:06
// **************************************
using SqlSugar;
 

namespace WWB.Park.Entity.System
{

    /// <summary>
    ///
    /// </summary>
    [SugarTable("sys_user")]
    public partial class SysUser : AuditDeleteEntityBase<long>
    {
        

        /// <summary>
        ///
        /// </summary>
        [SugarColumn(ColumnName = "user_name", Length = 32)]
        public string UserName { get; set; }

        /// <summary>
        ///
        /// </summary>
        [SugarColumn(ColumnName = "real_name", Length = 32)]
        public string RealName { get; set; }

        /// <summary>
        ///
        /// </summary>
        [SugarColumn(ColumnName = "nick_name", Length = 32, IsNullable = true)]
        public string NickName { get; set; }

        /// <summary>
        ///
        /// </summary>
        [SugarColumn(ColumnName = "avatar", Length = 512, IsNullable = true)]
        public string Avatar { get; set; }

        /// <summary>
        ///
        /// </summary>
        [SugarColumn(ColumnName = "phone", Length = 11)]
        public string Phone { get; set; }

        /// <summary>
        ///
        /// </summary>
        [SugarColumn(ColumnName = "password", Length = 32, IsNullable = true)]
        public string Password { get; set; }

        /// <summary>
        ///
        /// </summary>
        [SugarColumn(ColumnName = "open_id", Length = 64, IsNullable = true)]
        public string OpenId { get; set; }

        /// <summary>
        ///
        /// </summary>
        [SugarColumn(ColumnName = "is_admin")]
        public int IsAdmin { get; set; }

       

        /// <summary>
        ///
        /// </summary>
        [SugarColumn(ColumnName = "last_login_ip", Length = 32, IsNullable = true)]
        public string LastLoginIp { get; set; }

     

        /// <summary>
        ///
        /// </summary>
        [SugarColumn(ColumnName = "remark", Length = 255, IsNullable = true)]
        public string Remark { get; set; }
    }

}