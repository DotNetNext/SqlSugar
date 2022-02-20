using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class DemoM_UnitOfWork
    {
        public static void Init()
        {
            Console.WriteLine("");
            Console.WriteLine("#### DemoM_UnitOfWork ####");


            DbContext.Db.UseTran(() =>
            {

                var id = DbContext.CustomDal.InsertReturnIdentity(new Custom() { Id = 1, Name = "guid" });
                var id2 = DbContext.OrderDal.InsertReturnIdentity(new Order() { Name = "guid2", Price = 0, CreateTime = DateTime.Now, CustomId = 1 });
                throw new Exception("");
            },
            e =>
            {
               //throw e;
            });
            Console.WriteLine("");
            Console.WriteLine("#### Saveable End ####");
        }
        public class DbContext
        {
            public static SqlSugarScope Db = new SqlSugarScope(new ConnectionConfig()
            {
                DbType = SqlSugar.DbType.SqlServer,
                ConnectionString = Config.ConnectionString,
                IsAutoCloseConnection = true
            }, db => {
                //单例参数配置，所有上下文生效
                db.Aop.OnLogExecuting = (s, p) =>
                {
                    Console.WriteLine(s);
                };
            });

            public static DbSet<Order> OrderDal => new DbSet<Order>();
            public static DbSet<Custom> CustomDal => new DbSet<Custom>();


            public class DbSet<T> : SimpleClient<T> where T : class, new()
            {
                public DbSet(ISqlSugarClient context = null) : base(context)//需要有构造参数
                {
                    base.Context = DbContext.Db;
                }

                /// <summary>
                /// 扩展方法，自带方法不能满足的时候可以添加新方法
                /// </summary>
                /// <returns></returns>
                public List<T> CommQuery(string json)
                {
                    //base.Context.Queryable<T>().ToList();可以拿到SqlSugarClient 做复杂操作
                    return null;
                }

            }
        }

    }
}
