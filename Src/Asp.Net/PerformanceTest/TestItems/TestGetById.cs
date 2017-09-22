using Dapper.Contrib.Extensions;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceTest.TestItems
{
    public class TestGetById
    {

        public void Init(OrmType type)
        {
            Database.SetInitializer<EFContext>(null);
            Console.WriteLine("测试一次读取1条数据的速度");
            var eachCount = 1000;

            var beginDate = DateTime.Now;
            for (int i = 0; i < 20; i++)
            {
                switch (type)
                {
                    case OrmType.SqlSugar:
                        SqlSugar(eachCount);
                        break;
                    case OrmType.Dapper:
                        Dapper(eachCount);
                        break;
                    case OrmType.EF:
                        EF(eachCount);
                        break;
                    default:
                        break;
                }
            }

            Console.Write("总计："+(DateTime.Now-beginDate).TotalMilliseconds/1000.0);
        }

        private static void SqlSugar(int eachCount)
        {
            GC.Collect();//回收资源
            System.Threading.Thread.Sleep(1);//休息1秒

            PerHelper.Execute(eachCount, "SqlSugar", () =>
            {
                using (SqlSugarClient conn = Config.GetSugarConn())
                {
                    var list2 = conn.Queryable<Test>().InSingle(1);
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
                     var list = conn.Get<Test>(1);
                }
            });
        }


        private static void EF(int eachCount)
        {
            GC.Collect();//回收资源
            System.Threading.Thread.Sleep(1);//休息1秒

            PerHelper.Execute(eachCount, "EF", () =>
            {
                using (EFContext conn = new EFContext(Config.connectionString))
                {
                    var list = conn.TestList.AsNoTracking().Single(it=>it.Id==1);
                }
            });
        }
    }
}
