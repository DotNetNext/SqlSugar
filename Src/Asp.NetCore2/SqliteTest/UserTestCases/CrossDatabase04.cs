using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlSugar;
namespace OrmTest
{
    public class CrossDatabase04
    {
        public static void Init()
        {

            var db = new SqlSugarClient(new List<ConnectionConfig>()
            {
                new ConnectionConfig(){ConfigId="A",DbType=DbType.Sqlite,ConnectionString="DataSource=/A_DB.sqlite",IsAutoCloseConnection=true,
                ConfigureExternalServices=new ConfigureExternalServices(){
                    EntityNameService=(x,y)=>{
                      y.DbTableName=y.DbTableName.ToLower();
                    },
                    EntityService=(x,y)=>{
                               y.DbColumnName=y.DbColumnName?.ToLower();
                    }
                }
                },
                new ConnectionConfig(){ConfigId="B",DbType=DbType.Sqlite,ConnectionString="DataSource=/B_DB.sqlite",IsAutoCloseConnection=true,
                      ConfigureExternalServices=new ConfigureExternalServices(){
                    EntityNameService=(x,y)=>{
                      y.DbTableName=y.DbTableName.ToUpper();
                    },
                    EntityService=(x,y)=>{
                               y.DbColumnName=y.DbColumnName?.ToUpper();
                    }
                }},
                new ConnectionConfig(){ConfigId="AB",DbType=DbType.Sqlite,ConnectionString="DataSource=/AB_DB.sqlite",IsAutoCloseConnection=true  }
            });
            db.GetConnection("A").Aop.OnLogExecuting =
            db.GetConnection("B").Aop.OnLogExecuting =
            db.GetConnection("AB").Aop.OnLogExecuting = (s, p) =>
            {

                Console.WriteLine(s);
            };


            db.GetConnection("A").CodeFirst.InitTables<OperatorInfo>();
            db.GetConnection("B").CodeFirst.InitTables<Role>();
            db.GetConnection("AB").CodeFirst.InitTables<OptRole>();

            db.GetConnection("A").DbMaintenance.TruncateTable<OperatorInfo>();
            db.GetConnection("B").DbMaintenance.TruncateTable<Role>();
            db.GetConnection("AB").DbMaintenance.TruncateTable<OptRole>();

            db.GetConnection("A").Insertable(new OperatorInfo() { id = 10, realname = "A" }).ExecuteCommand();
            db.GetConnection("B").Insertable(new Role() { id = 101, name = "B" }).ExecuteCommand();
            db.GetConnection("AB").Insertable(new OptRole() { Id = 1, OperId = 10, RoleId = 101 }).ExecuteCommand();

            var x = db.GetConnection("A").Queryable<OperatorInfo>()
               .CrossQuery(typeof(Role),"B")
               .CrossQuery(typeof(OptRole), "AB")
               .Includes(z => z.Roles.Where(it=>it.id==1).ToList()).ToList();

           
        }
        /// <summary>
        /// 描述：
        /// 作者：synjones
        /// 时间：2022-04-20 21:30:28
        /// </summary>
        [SugarTable("unit_operatorinfo213")]
        [Tenant("A")]
        public partial class OperatorInfo
        {
            /// <summary>
            /// 主键
            /// </summary>
            [SugarColumn(IsPrimaryKey = true)]
            public int id { get; set; }

            /// <summary>
            /// 姓名
            /// </summary>
            public string realname { get; set; }

            /// <summary>
            /// 多角色
            /// </summary>
            [Navigate(typeof(OptRole), nameof(OptRole.OperId), nameof(OptRole.RoleId))]//名字换
            public List<Role> Roles { get; set; }
        }

        /// <summary>
        /// 描述：
        /// 作者：synjones
        /// 时间：2022-04-20 21:30:28
        /// </summary>
        [SugarTable("unit_role112313")]
        [Tenant("B")]
        public partial class Role
        {
            /// <summary>
            /// 角色
            /// </summary>
            [SugarColumn(IsPrimaryKey = true)]
            public int id { get; set; }

            /// <summary>
            /// 角色名称
            /// </summary>
            public string name { get; set; }

        }


        /// <summary>
        /// 描述：
        /// 作者：synjones
        /// 时间：2022-04-21 14:35:09
        /// </summary>
        [SugarTable("Unit_Operator_Role123131")]
        [Tenant("AB")]
        public partial class OptRole
        {
            /// <summary>
            /// 
            /// </summary>
            [SugarColumn(IsPrimaryKey = true)]
            public int Id { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public int OperId { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public int RoleId { get; set; }


        }
    }
}
