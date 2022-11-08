using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace OrmTest
{
    public class Unit001
    {

        public static void Init()
        {
           Get().GetAwaiter().GetResult();
        }
        public static async Task<object> Get()
        {
            var db = NewUnitTest.Db;
            db.DbMaintenance.CreateDatabase();

            db.CurrentConnectionConfig.MoreSettings = new ConnMoreSettings()
            {
                 PgSqlIsAutoToLowerCodeFirst = false,
                 PgSqlIsAutoToLower=false
            };

            //建表 
            if (!db.DbMaintenance.IsAnyTable(nameof(RoleEntity), false))
            {
                db.CodeFirst.InitTables<RoleEntity>();
            }
            if (!db.DbMaintenance.IsAnyTable(nameof(ManagerRoleRelationEntity), false))
            {
                db.CodeFirst.InitTables<ManagerRoleRelationEntity>();
            }
            if (!db.DbMaintenance.IsAnyTable(nameof(ManagerEntity), false))
            {
                db.CodeFirst.InitTables<ManagerEntity>();
            }
            if (!db.DbMaintenance.IsAnyTable(nameof(RoleManagerGroupRelationEntity), false))
            {
                db.CodeFirst.InitTables<RoleManagerGroupRelationEntity>();
            }
            if (!db.DbMaintenance.IsAnyTable(nameof(ManagerManagerGroupRelationEntity), false))
            {
                db.CodeFirst.InitTables<ManagerManagerGroupRelationEntity>();
            }
            RefAsync<int> total = 0;

            //用例代码 
            var roleList = await db.Queryable<RoleEntity>()
                  .Where(item => item.RemoveMark == 0)
                  .Select(item => new GetRoleOutput()
                  {
                      RoleID = item.RoleID,
                      RoleName = item.RoleName,
                      NumberOfAssociatedAcount = SqlFunc.Subqueryable<ManagerRoleRelationEntity>().Where(it => it.RoleID == item.RoleID).Count()

                      + SqlFunc.Subqueryable<ManagerManagerGroupRelationEntity>()
                      .LeftJoin<ManagerEntity>((mmgMap, manger) => manger.ManagerID == mmgMap.ManagerID && manger.EnabledMark == 1 && manger.RemoveMark == 0)
                      .InnerJoin<RoleManagerGroupRelationEntity>((mmgMap, rmgMap) => mmgMap.ManagerGroupID == rmgMap.ManagerGroupID && rmgMap.RoleID == item.RoleID)
                      .Where(mmgMap => SqlFunc.Subqueryable<ManagerRoleRelationEntity>().Where(it => it.RoleID == item.RoleID && it.ManagerID == mmgMap.ManagerID).NotAny())

                      .Select(mmgMap => SqlFunc.AggregateDistinctCount(mmgMap.ManagerID))
                      ,
                      CreateTime = item.CreateTime,
                      ModifyTime = item.ModifyTime,
                      EnabledMark = item.EnabledMark,

                  }
                  )
                  .ToPageListAsync(2, 2, total);


            return roleList;
        }
        #region 用例实体
        //用例实体

        [SugarTable("sys_manager_managergroup_relation")]
        public class ManagerManagerGroupRelationEntity
        {
            /// <summary>
            /// 
            /// </summary>
            public ManagerManagerGroupRelationEntity()
            {
            }

            /// <summary>
            /// 
            /// </summary>
            [SugarColumn(IsPrimaryKey = true)]
            public System.String ManagerManagerGroupRelationID { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String ManagerID { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String ManagerGroupID { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.Int32? SortCode { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.Int32? RemoveMark { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.Int32? EnabledMark { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String Description { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.DateTime? CreateTime { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String CreateUserID { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String CreateUserName { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.DateTime? ModifyTime { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String ModifyUserID { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String ModifyUserName { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String DeleteUserID { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String DeleteUserName { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.DateTime? DeleteTime { get; set; }


            public void Create()
            {
                this.ManagerManagerGroupRelationID = "";
                this.CreateTime = DateTime.Now;
                this.RemoveMark = 0;
                this.EnabledMark = 1;

            }
        }


        [SugarTable("sys_role_managergroup_relation")]
        public class RoleManagerGroupRelationEntity
        {
            /// <summary>
            /// 
            /// </summary>
            public RoleManagerGroupRelationEntity()
            {
            }

            /// <summary>
            /// 
            /// </summary>
            [SugarColumn(IsPrimaryKey = true)]
            public System.String RoleManagerGroupRelationID { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String RoleID { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String ManagerGroupID { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.Int32? SortCode { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.Int32? RemoveMark { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.Int32? EnabledMark { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String Description { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.DateTime? CreateTime { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String CreateUserID { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String CreateUserName { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.DateTime? ModifyTime { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String ModifyUserID { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String ModifyUserName { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String DeleteUserID { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String DeleteUserName { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.DateTime? DeleteTime { get; set; }
        }


        [SugarTable("sys_role")]
        public class RoleEntity
        {
            /// <summary>
            /// 
            /// </summary>
            public RoleEntity()
            {
            }

            [SugarColumn(IsIgnore = true)]
            public int NumberOfAssociatedAcount { get; set; }

            /// <summary>
            /// 
            /// </summary>
            [SugarColumn(IsPrimaryKey = true)]
            public System.String RoleID { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String RoleCategory { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String RoleName { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String EnCode { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.Boolean? IsSys { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.Int32? SortCode { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.Int32? RemoveMark { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.Int32? EnabledMark { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String Description { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.DateTime CreateTime { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String CreateUserID { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String CreateUserName { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.DateTime? ModifyTime { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String ModifyUserID { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String ModifyUserName { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String DeleteUserID { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String DeleteUserName { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.DateTime? DeleteTime { get; set; }



            public void Create()
            {
                this.RoleID = "";
                this.CreateTime = DateTime.Now;
                this.RemoveMark = 0;
                this.EnabledMark = 1;

            }

            public void Modified()
            {
                this.ModifyTime = DateTime.Now;
            }

        }

        [SugarTable("sys_manager_role_relation")]
        public class ManagerRoleRelationEntity
        {

            /// <summary>
            /// 
            /// </summary>
            [SugarColumn(IsPrimaryKey = true)]
            public System.String ManagerRoleRelationID { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String ManagerID { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String RoleID { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.Int32? SortCode { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.Int32? RemoveMark { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.Int32? EnabledMark { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String Description { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.DateTime? CreateTime { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String CreateUserID { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String CreateUserName { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.DateTime? ModifyTime { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String ModifyUserID { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String ModifyUserName { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String DeleteUserID { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String DeleteUserName { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.DateTime? DeleteTime { get; set; }
        }


        [SugarTable("sys_manager")]
        public class ManagerEntity
        {

            [SugarColumn(IsIgnore = true)]
            public List<string> OrganizationIDs { get; set; }







            [SugarColumn(IsIgnore = true)]
            public List<RoleEntity> Roles { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public ManagerEntity()
            {
            }

            /// <summary>
            /// 
            /// </summary>
            [SugarColumn(IsPrimaryKey = true)]
            public System.String ManagerID { get; set; }


            /// <summary>
            /// 
            /// </summary>
            public System.String Encode { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String Account { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String Password { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String SecretKey { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String RealName { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String NickName { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.Int32 LoginCount { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String AvatarImage { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String QuickQuery { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String SimpleSpelling { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String Gender { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.Boolean? IsSys { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.DateTime? Birthday { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String Mobile { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String Telephone { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String Email { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String OICQNO { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String WeChat { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String MSN { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.Int32? SecurityLevel { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String OpenID { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String Question { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String AnswerQuestion { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.Int32? CheckOnLine { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.DateTime? AllowStartTime { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.DateTime? AllowEndTime { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.DateTime? LockStartDate { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.DateTime? LockEndDate { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.DateTime? LastLoginTime { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.Int32? SortCode { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.Int32? EnabledMark { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String Description { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.DateTime CreateTime { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String CreateUserID { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String CreateUserName { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.DateTime? ModifyTime { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String ModifyUserID { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String ModifyUserName { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String DeleteUserID { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.String DeleteUserName { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.DateTime? DeleteTime { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public System.Int32? RemoveMark { get; set; }

            public void Create()
            {
                this.ManagerID = "";
                this.CreateTime = DateTime.Now;
                this.RemoveMark = 0;
                //this.EnabledMark = 1;

            }


            public void Modify(string id)
            {
                this.ManagerID = id;
                this.ModifyTime = DateTime.Now;
                this.QuickQuery = this.RealName ?? "";
                this.SimpleSpelling = this.RealName;
                this.SortCode = this.SortCode ?? 99;
                this.RemoveMark = 0;
                //this.EnabledMark = 1;
                this.IsSys = false;
                this.LockEndDate = DateTime.Now;
                this.DeleteTime = DateTime.MinValue;
            }
        }



        #region DTO
        //DTO
        public class GetRoleOutput
        {

            public int NumberOfAssociatedAcount { get; set; }

            public string RoleID { get; set; }


            /// <summary>
            /// 
            /// </summary>
            public string RoleName { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string Encode { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public bool? IsSys { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public int? SortCode { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public int? RemoveMark { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public int? EnabledMark { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string Description { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public DateTime? CreateTime { get; set; }
            public DateTime? ModifyTime { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string CreateUserName { get; set; }

            public List<GetRoleNavigationModel> GetRoleNavigationList { get; set; }
        }



        public class GetRoleNavigationModel
        {
            /// <summary>
            /// 导航栏ID
            /// </summary>
            public string NavigationID { get; set; }
            public string RoleNavigationRelationID { get; set; }

            public List<GetRoleFunctionPermissionModel> GetRoleFunctionPermissionList { get; set; }

        }

        public class GetRoleFunctionPermissionModel
        {

            public string FunctionAuthID { get; set; }


        }

        #endregion

        #endregion
    }


}
