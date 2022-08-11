using SqlSugar;
using System;

namespace OrmTest
{
    /// <summary>
    /// 后台用户信息表
    /// </summary>
    public class TB_AdminUser  
    {
        /// <summary>
        /// 后台用户信息表
        /// </summary>
        public TB_AdminUser()
        {
        }

        /// <summary>
        /// 后台用户信息表
        /// </summary>
        /// <param name="setid">是否给ID赋值</param>
        public TB_AdminUser(bool setid)
        {
             
        }

        /// <summary>
        /// 后台用户ID, 主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true) ]
        public System.Int64 ID { get; set; }

        /// <summary>
        /// 用户真实姓名
        /// </summary>
        public System.String Name { get; set; }  

        /// <summary>
        /// 用户职称
        /// </summary>
        public System.String JobTitle { get; set; }   

        /// <summary>
        /// 用户手机号, 亦为登录后台系统的账号
        /// </summary>
        public System.String Phone { get; set; }  

        /// <summary>
        /// 登录密码, MD5加密的, 32位
        /// </summary>
  
        public string Password { get; set; }  

        /// <summary>
        /// 所属部门ID, 逻辑关联TB_AdminDepartment表的ID
        /// </summary>
 
        public System.Int64 AdminDepartmentID { get; set; }

        /// <summary>
        /// 角色ID, 逻辑关联TB_AdminRole表的ID
        /// </summary>
     
        public System.Int64 AdminRoleID { get; set; }

        /// <summary>
        /// 用户状态. -1: 离职停用, 0: 冻结, 1: 正常
        /// </summary>
 
        public AdminUserStatus Status { get; set; }

        /// <summary>
        /// 备注. 
        /// </summary>
  
        public System.String  Remarks { get; set; }

        /// <summary>
        /// 创建后台用户ID, 逻辑关联TB_AdminUser表的ID
        /// </summary>
  
        public System.Int64 CreateAdminUserID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public System.DateTime CreateDateTime { get; set; }

        /// <summary>
        /// 最后修改后台用户ID, 逻辑关联TB__AdminUser表的ID
        /// </summary>
 
        public System.Int64 ModifyAdminUserID { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public System.DateTime ModifyDateTime { get; set; }

        /// <summary>
        /// 最后登录时间, null未登录过
        /// </summary>
        public System.DateTime? LastLoginDateTime { get; set; }

        /// <summary>
        /// 累计登录次数
        /// </summary>
        public System.Int32 LoginCount { get; set; }

        /// <summary>
        /// 最后修改密码的时间
        /// </summary>
        public DateTime ModifyPasswordDateTime { get; set; }

        /// <summary>
        /// 后台用户信息表状态枚举
        /// <para>-1: 离职停用, 0: 冻结, 1: 正常</para>
        /// </summary>
        public enum AdminUserStatus : sbyte
        {
            /// <summary>
            /// 离职 (停用, 类似删除)
            /// </summary>
            Dimission = -1,

            /// <summary>
            /// 冻结, 无法登录
            /// </summary>
            Freeze = 0,

            /// <summary>
            /// 正常
            /// </summary>
            Normal = 1
        }
    }
}