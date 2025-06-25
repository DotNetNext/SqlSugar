using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  OrmTest
{
    internal class Unitsfasdyd
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<UserEntity, DeptEntity>();

            db.DbMaintenance.TruncateTable<UserEntity, DeptEntity>();

            var dept = new DeptEntity()
            {
                Id = Guid.NewGuid(),
                DeptName = "研发部",
                DeptCode = "RD",
                Leader = "张三"
            };

            db.Insertable<DeptEntity>(dept).ExecuteCommand();

            var user = new UserEntity()
            {
                Id = Guid.NewGuid(),
                UserName = "admin",
                DeptId = dept.Id, // 关联部门
                EncryPassword = new EncryPasswordValueObject("123qwe"),
            };


            db.Insertable<UserEntity>(user).ExecuteCommand();

            var users =   db.Queryable<UserEntity>()
                //.Includes(x => x.Roles.Where(r=>r.RoleName.StartsWith("管理")).ToList())
                .Includes(x => x.Dept)
                .ToList();

            if (users.First().Dept == null) 
            {
                throw new Exception("error");
            }
        }

        /// <summary>
        /// 角色表
        /// </summary>
        [SugarTable("sys_roledsfadf222")]
        public class RoleEntity
        {
            [SugarColumn(IsPrimaryKey = true)]
            public Guid Id { get; set; }
            /// <summary>
            /// 角色名
            /// </summary>
            public string RoleName { get; set; } = string.Empty;

            /// <summary>
            /// 角色编码 
            ///</summary>
            [SugarColumn(ColumnName = "RoleCode")]
            public string RoleCode { get; set; } = string.Empty;

            /// <summary>
            /// 描述 
            ///</summary>
            [SugarColumn(ColumnName = "Remark")]
            public string  Remark { get; set; }
        }


        /// <summary>
        /// 用户表
        /// </summary>
        [SugarTable("sys_user1231313131")]
        [SugarIndex($"index_{nameof(UserName)}", nameof(UserName), OrderByType.Asc)]
        public class UserEntity
        {
            [SugarColumn(IsPrimaryKey = true)]
            public Guid Id { get; set; }

            /// <summary>
            /// 用户名
            /// </summary>
            public string UserName { get; set; } = string.Empty;


            /// <summary>
            /// 加密密码
            /// </summary>
            [SugarColumn(IsOwnsOne = true)]
            public EncryPasswordValueObject? EncryPassword { get; set; }


            ///// <summary>
            ///// 角色
            ///// </summary>
            //[Navigate(typeof(UserRoleEntity), nameof(UserRoleEntity.UserId), nameof(UserRoleEntity.RoleId))]
            //public List<RoleEntity> Roles { get; set; }


            /// <summary>
            /// 部门id
            /// </summary>
            public Guid? DeptId { get; set; }

            /// <summary>
            /// 部门
            /// </summary>

            [Navigate(NavigateType.OneToOne, nameof(DeptId))]
            public DeptEntity? Dept { get; set; }

        }

        [SugarTable("sys_dept2213s")]
        public class DeptEntity
        {
            [SugarColumn(IsPrimaryKey = true)]
            public Guid Id { get; set; }

            /// <summary>
            /// 部门名称 
            ///</summary>
            public string DeptName { get; set; }
            /// <summary>
            /// 部门编码 
            ///</summary>
            [SugarColumn(ColumnName = "DeptCode")]
            public string DeptCode { get; set; }
            /// <summary>
            /// 负责人 
            ///</summary>
            [SugarColumn(ColumnName = "Leader")]
            public string? Leader { get; set; }
        }

        [SugarTable("sys_userrole111")]
        public class UserRoleEntity
        {
            [SugarColumn(IsPrimaryKey = true)]
            public Guid Id { get; set; }


            /// <summary>
            /// 角色id
            /// </summary>
            public Guid RoleId { get; set; }

            /// <summary>
            /// 用户id
            /// </summary>
            public Guid UserId { get; set; }
        }

        public class EncryPasswordValueObject
        {
            public EncryPasswordValueObject() { }
            public EncryPasswordValueObject(string password)
            {
                this.Password = password;
                this.Salt = DateTime.Now.Ticks.ToString(); // 使用当前时间戳作为盐值
            }

            /// <summary>
            /// 密码
            /// </summary>
            public string Password { get; set; } = string.Empty;

            /// <summary>
            /// 加密盐值
            /// </summary>
            public string Salt { get; set; } = string.Empty;


        }
    }
}
