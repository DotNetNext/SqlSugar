using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Dapper;
using SqlSugar;
using Dapper.Contrib.Extensions;

namespace PerformanceTest.TestItems
{
    public class TestGetAll  
    {
        public void Init(OrmType type)
        {
            Console.WriteLine("测试一次读取100万条数据的速度");
            var eachCount = 1;

            var beginDate = DateTime.Now;
            for (int i = 0; i < 10; i++)
            {
                switch (type)
                {
                    case OrmType.SqlSugar:
                        SqlSugar(eachCount);
                        break;
                    case OrmType.Dapper:
                        Dapper(eachCount);
                        break;
                    default:
                        break;
                }
    
            }
            Console.Write("总计：" + (DateTime.Now - beginDate).TotalMilliseconds / 1000.0);
        }

        private static void SqlSugar(int eachCount)
        {
            GC.Collect();//回收资源
            System.Threading.Thread.Sleep(1);//休息1秒

            PerHelper.Execute(eachCount, "SqlSugar", () =>
            {
                using (SqlSugarClient conn = Config.GetSugarConn())
                {
                    var list2 = conn.Queryable<Test>().ToList();
                }
            });
        }


        private static void Dapper(int eachCount)
        {
            GC.Collect();//回收资源
            System.Threading.Thread.Sleep(1);//休息1秒

            PerHelper.Execute(eachCount, "Dapper", () =>
            {
                using (SqlConnection conn = new SqlConnection(Config.connectionString))
                {
                  var list = conn.GetAll<Test>();
                }
            });
        }
    }
}
