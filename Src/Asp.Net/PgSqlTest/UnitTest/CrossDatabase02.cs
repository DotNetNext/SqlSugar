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
                new ConnectionConfig(){ConfigId="A",DbType=DbType.PostgreSQL,ConnectionString=Config.ConnectionString,IsAutoCloseConnection=true},
                new ConnectionConfig(){ConfigId="B",DbType=DbType.PostgreSQL,DbLinkName="public",ConnectionString=Config.ConnectionString,IsAutoCloseConnection=true  },
               
            });

            db.Aop.OnLogExecuting = (x, y) => Console.WriteLine(UtilMethods.GetNativeSql(x,y));

            db.GetConnection("A").CodeFirst.InitTables<OptRole>();
            db.GetConnection("B").CodeFirst.InitTables<Role>(); 

            db.GetConnection("A").DbMaintenance.TruncateTable<OptRole>();
            db.GetConnection("B").DbMaintenance.TruncateTable<Role>(); 

         
            var x3 = db.QueryableWithAttr<OptRole>()
                .LeftJoin<Role>((x1, y1) => x1.roleId == y1.id).ToList();
 
        }
       

        /// <summary>
        /// 描述：
        /// 作者：synjones
        /// 时间：2022-04-20 21:30:28
        /// </summary>
        [SugarTable("unit_role18")]
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
        [SugarTable("unit_operator_role8")]
        [Tenant("A")]
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
            public int  roleId { get; set; }

        }
    }
}
