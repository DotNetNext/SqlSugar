using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public partial class NewUnitTest
    {
        [SugarTable("UnitUser")]
        public class User2
        {
            [SugarColumn(IsPrimaryKey = true)]
            public int Id { set; get; }
            public string Name { set; get; }
            public int DepartmentId { set; get; }
        }
        [SugarTable("UnitOrder")]
        public class Order2
        {
            [SugarColumn(IsPrimaryKey = true)]
            public int Id { set; get; }
            public string Name { set; get; }
            public int UserId { set; get; }
        }
        [SugarTable("UnitDepartment")]
        public class Department2
        {
            [SugarColumn(IsPrimaryKey = true)]
            public int Id { set; get; }
            public string Name { set; get; }
        }
        public static async Task QueryableAsync2()
        {
            DataBaseInitialize();
            var context = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = "Data Source=.;Initial Catalog=test2222;User id=sa;Password=haosql;pooling=true;min pool size = 2;max pool size=100;",
                DbType = DbType.SqlServer,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute,
            });
            //调式代码 用来打印SQL 
            context.Aop.OnLogExecuting = (sql, pars) =>
            {
                Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "开始\r\n" + sql + "\r\n" + context.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value)));
            };
            context.Aop.OnLogExecuted = (sql, pars) =>
            {
                Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "结束\r\n" + sql + "\r\n" + context.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value)));
            };
            var recordCount = new RefAsync<int>();
            //var result = await context.Queryable<Order, User, Department>((o, u, d) => new JoinQueryInfos
            var result = await context.Queryable<Order2, User2>((o, u) => new JoinQueryInfos
               (
                   JoinType.Left, o.UserId == u.Id
               //,JoinType.Left, u.DepartmentId == d.Id
               ))
                .AS<Order2>("dbo.UnitOrder")
                .AS<User2>("test1111.dbo.UnitUser")
                //.AS<Department>("test1.dbo.Department")
                .Select(o => new User2() { Id = o.Id, Name = o.Name })
                .ToPageListAsync(1, 10, recordCount);

            Console.ReadLine();
        }
        public static void DataBaseInitialize()
        {
            //数据库1
            var context1 = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = "Data Source=.;Initial Catalog=test1111;User id=sa;Password=haosql;pooling=true;min pool size = 2;max pool size=100;",
                DbType = DbType.SqlServer,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute,
            });
            context1.DbMaintenance.CreateDatabase();
            context1.CodeFirst.InitTables(typeof(User2));
            context1.CodeFirst.InitTables(typeof(Department2));
            if (context1.Queryable<User2>().Where(t => t.Id == 1).Count() == 0)
                context1.Insertable(new User2() { Id = 1, Name = "TestUser", DepartmentId = 1 }).ExecuteCommand();
            if (context1.Queryable<Department2>().Where(t => t.Id == 1).Count() == 0)
                context1.Insertable(new Department2() { Id = 1, Name = "TestDepartment" }).ExecuteCommand();
            //数据库2
            var context2 = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = "Data Source=.;Initial Catalog=test2222;User id=sa;Password=haosql;pooling=true;min pool size = 2;max pool size=100;",
                DbType = DbType.SqlServer,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute,
            });
            context2.DbMaintenance.CreateDatabase();
            context2.CodeFirst.InitTables(typeof(Order2));
            if (context1.Queryable<User2>().Where(t => t.Id == 1).Count() == 0)
                context2.Insertable(new Order2() { Id = 1, Name = "Order", UserId = 1 }).ExecuteCommand();
        }
    }
}
