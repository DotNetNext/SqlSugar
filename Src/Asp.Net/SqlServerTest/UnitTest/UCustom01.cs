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
                Console.WriteLine(UtilMethods.GetSqlString(DbType.SqlServer,s,p));
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
            db.CodeFirst.InitTables<BoolTest12313>();
            var x1 = db.Queryable<BoolTest12313>().Where(it => it.IsDeleted && SqlFunc.HasValue(it.name) ).ToList();
            var x2 = db.Queryable<BoolTest12313>().Where(it => it.IsDeleted && SqlFunc.HasValue(it.name)==true).ToList();
            var x3 = db.Queryable<BoolTest12313>().Where(it =>true  && SqlFunc.HasValue(it.name) == true).ToList();
            var x4 = db.Queryable<BoolTest12313>().Where(it =>SqlFunc.HasValue(it.name) == true).ToList();
            var x5 = db.Queryable<BoolTest12313>().Where(it => SqlFunc.HasValue(it.name) ).ToList();
            var x6 = db.Queryable<BoolTest12313>().Select(it => new  { x=SqlFunc.HasValue(it.name)}).ToList();
            var x7 = db.Queryable<BoolTest12313>().Select(it => new { x = it.IsDeleted?  "a" : "b"  }).ToList();
            var x8 = db.Queryable<BoolTest12313>().Select(it => new { x = SqlFunc.IIF(it.IsDeleted,"a","b") }).ToList();
            db.CurrentConnectionConfig.MoreSettings = new ConnMoreSettings()
            { 
                 IsWithNoLockQuery=true
            };
            var sql=db.Queryable<Order>().LeftJoin<Order>((z, y) => z.Id == y.Id).ToSql().Key;
            if (!sql.Contains(SqlWith.NoLock)) { throw new Exception("unit error"); }
            var sql2=db.SqlQueryable<Order>("select * from [Order]").With(SqlWith.Null).LeftJoin<OrderItem>((z, y) => z.Id == y.OrderId).ToSql().Key;
            if (sql2.Contains(SqlWith.NoLock)) { throw new Exception("unit error"); }
            db.DbMaintenance.DropTable("BoolTest12313");

            string p1 = null;
            var sql3=db.Queryable<Order>().Where(x => x.Name == p1).ToSql();
            if (!sql3.Key.Contains("IS  NULL ")) { throw new Exception("unit error"); };
            db.Queryable<Order>().Where(x => x.Name == p1).First();
            var sql31 = db.Queryable<Order>().Where(x => x.Name != p1).ToSql();
            if (!sql31.Key.Contains(" IS NOT  NULL ")) { throw new Exception("unit error"); };
            db.Queryable<Order>().Where(x => x.Name != p1).First();
            p1 = "";
            var sql4 = db.Queryable<Order>().Where(x => x.Name == p1).ToSql();
            if (!sql4.Key.Contains("@")) { throw new Exception("unit error"); };
            var sql41 = db.Queryable<Order>().Where(x => x.Name != p1).ToSql();
            if (!sql41.Key.Contains("@")) { throw new Exception("unit error"); };

            db.Queryable<Order>().Where(x11 => x11.Name + "a" == x11.Name).ToList();
            db.Queryable<Order>().Where(x11 => x11.Name == x11.Name + "a").ToList();
            db.Queryable<Order>().Where(x11 => "a"+x11.Name+ p1 == x11.Name).ToList();
            db.Queryable<Order>().Where(x11 => x11.Name == "a" + x11.Name + p1).ToList();
            db.Queryable<Order>().Where(x11 => SqlFunc.ToString("a"+p1+x11.Name)==x11.Name).ToList();
            db.Updateable<Order>()
                .SetColumns(x => x.Name == x.Name + "a")
                .Where(z => z.Id == 1)
                .ExecuteCommand();
            db.Updateable<Order>()
              .SetColumns(x => new Order() { Name = x.Name + "a" })
              .Where(z => z.Id == 1)
              .ExecuteCommand();
            var ids = new string[] { "a", "c" };
            for (int i = 0; i < 2; i++)
            {
                db.Queryable<Order>().Where(z => ids[i] == z.Name).ToList();
                db.Queryable<Order>().Select(z => ids[i]).ToList();
            }
        }

        public class BoolTest12313
        {
            public string name { get; set; }
            public bool IsDeleted { get; set; }
            public int xx { get; set; }
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
