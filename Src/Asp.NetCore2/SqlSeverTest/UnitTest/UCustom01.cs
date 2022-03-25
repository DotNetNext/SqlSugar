using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class UCustom01
    {

        public static void Init()
        {
            var db = new SqlSugarScope(new SqlSugar.ConnectionConfig()
            {
                ConnectionString = Config.ConnectionString,
                DbType = DbType.SqlServer,
                IsAutoCloseConnection = true
            });
            db.Aop.OnLogExecuted = (s, p) =>
            {
                Console.WriteLine(s);
            };
            //建表 
            if (!db.DbMaintenance.IsAnyTable("User_Test001", false))
            {
                db.CodeFirst.InitTables<User_Test001>();
            }
            if (!db.DbMaintenance.IsAnyTable("UserRole_Test001", false))
            {
                db.CodeFirst.InitTables<UserRole_Test001>();
            }

            //用例代码 
            var result = db.Queryable<User_Test001, UserRole_Test001>((u, ur) => new object[] {

                            JoinType.Left,u.ID==ur.UserID



                        }).Select((u, ur) => new

                        {

                            customName = SqlFunc.Subqueryable<User_Test001>().Where(s => s.UserName == u.UserName).Select(s => s.UserName+"")



                        }).ToPageList(1, 10);

        }
        [SugarTable("unitUser_Test001")]
        public class User_Test001
        {

            public int ID { get; set; }
            public string UserName { get; set; }
        }
        [SugarTable("unitUserRole_Test001")]
        public class UserRole_Test001
        {

            public int ID { get; set; }
            public int UserID { get; set; }
        }
    }
}
