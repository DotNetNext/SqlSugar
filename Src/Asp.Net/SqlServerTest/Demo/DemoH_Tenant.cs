using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest 
{
    public class DemoH_Tenant
    {
        public static void Init()
        {
            Console.WriteLine("");
            Console.WriteLine("#### SimpleClient Start ####");
            new C1Service().Test();
            Console.WriteLine("#### SimpleClient End ####");
        }
        public class C1Service : Repository<C1Table>
        {
            public void Test() 
            {
                base.GetList(); 
                base.ChangeRepository<C2Service>().GetList();
            }
        }
        public class C2Service : Repository<C2Table>
        {

        }


        public class Repository<T> : SimpleClient<T> where T : class, new()
        {
            public Repository(ISqlSugarClient context = null) : base(context)//注意这里要有默认值等于null
            {
                if (context == null)
                {
                    var db = new SqlSugarClient(new List<ConnectionConfig> {
                                                        new ConnectionConfig()
                                                    {
                                                        ConfigId=1,
                                                        DbType = SqlSugar.DbType.SqlServer,
                                                        IsAutoCloseConnection = true,
                                                        ConnectionString = Config.ConnectionString
                                                    },
                                                        new ConnectionConfig()
                                                    {
                                                        ConfigId="2",
                                                        DbType = SqlSugar.DbType.SqlServer,
                                                        IsAutoCloseConnection = true,
                                                        ConnectionString = Config.ConnectionString2
                                                    }
                    });
                    var configId = typeof(T).GetCustomAttribute<TenantAttribute>().configId;
                    base.Context = db.GetConnection(configId);
                    base.Context.CodeFirst.InitTables<T>();
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

        [TenantAttribute("1")]
        public class C1Table 
        {
           public string Id { get; set; }
        }
        [TenantAttribute("2")]
        public class C2Table
        {
            public string Id { get; set; }
        }
    }
}