using SqlSugar;
using System;

namespace CacheTest
{
    class Program
    {
        static void Main(string[] args)
        {
            SugarCache cache = new SugarCache();
            cache.Add("a", "1");

            var x = cache.Get<string>("a");
            cache.Add("a2", "11",5);
            var x2 = cache.Get<string>("a2");
            var isa= cache.ContainsKey<string>("a2");
            var allKeys = cache.GetAllKey<string>();
            var testr=cache.GetOrCreate<string>("a33",()=> { return "aaa"; },10);
            cache.Remove<string>("aaaaaaaa");
            cache.Remove<string>("a");
            ICacheService myCache = cache;

            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = "server=.;uid=sa;pwd=sasa;database=SQLSUGAR4XTEST",
                DbType = DbType.SqlServer,
                IsAutoCloseConnection = true,
                MoreSettings=new ConnMoreSettings() { 
                 IsAutoRemoveDataCache = true,
                },
                ConfigureExternalServices = new ConfigureExternalServices()
                {
                    DataInfoCacheService = myCache //配置我们创建的缓存类，具体用法看标题5
                }
            });
            db.Fastest<Order>().BulkCopy(new System.Collections.Generic.List<Order>()
            {
                 new Order(){ CreateTime=DateTime.Now, CustomId=1, Name="a" }
            });
            Console.WriteLine("Hello World!");
        }
    }
}
