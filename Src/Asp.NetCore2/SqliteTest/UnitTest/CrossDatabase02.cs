using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlSugar;
namespace OrmTest
{
    public class CrossDatabase02
    {
        public static void Init()
        {

            var db = new SqlSugarClient(new List<ConnectionConfig>()
            {
                new ConnectionConfig(){ConfigId="A",DbType=DbType.Sqlite,ConnectionString="DataSource=/A_DB.sqlite",IsAutoCloseConnection=true},
                new ConnectionConfig(){ConfigId="B",DbType=DbType.Sqlite,ConnectionString="DataSource=/B_DB.sqlite",IsAutoCloseConnection=true  },
                new ConnectionConfig(){ConfigId="AB",DbType=DbType.Sqlite,ConnectionString="DataSource=/AB_DB.sqlite",IsAutoCloseConnection=true  }
            });

            db.GetConnection("A").CodeFirst.InitTables<OperatorInfo>();
            db.GetConnection("B").CodeFirst.InitTables<Role>();
            db.GetConnection("AB").CodeFirst.InitTables<OptRole>();

            db.GetConnection("A").DbMaintenance.TruncateTable<OperatorInfo>();
            db.GetConnection("B").DbMaintenance.TruncateTable<Role>();
            db.GetConnection("AB").DbMaintenance.TruncateTable<OptRole>();

            db.GetConnection("A").Insertable(new OperatorInfo() {  id=10, realname="A"}).ExecuteCommand();
            db.GetConnection("B").Insertable(new Role() {  id=101, name="B"}).ExecuteCommand();
            db.GetConnection("AB").Insertable(new OptRole() {  id=1, operId=10, roleId=101}).ExecuteCommand();

             var x=db.QueryableWithAttr<OperatorInfo>()
                //.CrossQueryWithAttr()
                .Includes(z => z.Roles).ToList();

            if (x.First().Roles.Count == 0) 
            {
                throw new Exception("unit error");
            }
        }
        /// <summary>
        /// 描述：
        /// 作者：synjones
        /// 时间：2022-04-20 21:30:28
        /// </summary>
        [SugarTable("unit_operatorinfo")]
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
            [Navigate(typeof(OptRole), nameof(OptRole.operId), nameof(OptRole.roleId))]//名字换
            public List<Role> Roles { get; set; }
        }

        /// <summary>
        /// 描述：
        /// 作者：synjones
        /// 时间：2022-04-20 21:30:28
        /// </summary>
        [SugarTable("unit_role1")]
        [Tenant("B")]
        public partial class Role
        {
            /// <summary>
            /// 角色
            /// </summary>
            [SugarColumn(IsPrimaryKey = true )]
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
        [SugarTable("unit_operator_role")]
        [Tenant("AB")]
        public partial class OptRole
        {
            /// <summary>
            /// 
            /// </summary>
            [SugarColumn(IsPrimaryKey = true)]
            public int id { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public int operId { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public int roleId { get; set; }


        }
    }
}
