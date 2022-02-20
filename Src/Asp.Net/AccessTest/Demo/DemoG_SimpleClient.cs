using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class DemoG_SimpleClient
    {
        public static void Init()
        {
            Console.WriteLine("");
            Console.WriteLine("#### SimpleClient Start ####");

            var order = new OrderDal();
            order.GetList();
            order.GetById(1);
            order.MyTest();
            Console.WriteLine("#### SimpleClient End ####");
        }
        public class OrderDal:Repository<Order>
        {
            public void MyTest() 
            {
                base.CommQuery("1=1");
                base.ChangeRepository<Repository<OrderItem>>().CommQuery("1=1");
            }
        }
        public class Repository<T> : SimpleClient<T> where T : class, new()
        {
            public Repository(ISqlSugarClient context = null) : base(context)//注意这里要有默认值等于null
            {
                if (context == null)
                {
                    var db = new SqlSugarClient(new ConnectionConfig()
                    {
                        DbType = SqlSugar.DbType.SqlServer,
                        InitKeyType = InitKeyType.Attribute,
                        IsAutoCloseConnection = true,
                        ConnectionString = Config.ConnectionString
                    });
                    base.Context = db;
                    db.Aop.OnLogExecuting = (s, p) =>
                    {
                        Console.WriteLine(s);
                    };
                }
            }

            /// <summary>
            /// 扩展方法，自带方法不能满足的时候可以添加新方法
            /// </summary>
            /// <returns></returns>
            public List<T> CommQuery(string sql)
            {
                //base.Context.Queryable<T>().ToList();可以拿到SqlSugarClient 做复杂操作
                return base.Context.Queryable<T>().Where(sql).ToList();
            }

        }
    }
}
