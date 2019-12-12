using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib;
using Dapper.Contrib.Extensions;

namespace PerformanceTest.TestItems
{
    public class TestInsert
    {
        public void Init(OrmType type)
        {
            Database.SetInitializer<EFContext>(null);
            Console.WriteLine("测试插入1条");
            var eachCount = 100;

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
                    case OrmType.EF:
                        EF(eachCount);
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
                    conn.Insertable(GetData()).ExecuteCommand();
                }
            });


            //删除插入数据
            DeleteAddData();
        }

        private static void Dapper(int eachCount)
        {
            GC.Collect();//回收资源
            System.Threading.Thread.Sleep(1);//休息1秒

            PerHelper.Execute(eachCount, "Dapper", () =>
            {
                using (SqlConnection conn = new SqlConnection(Config.connectionString))
                {
                    conn.Insert(GetData());
                }
            });

            //删除插入数据
            DeleteAddData();
        }

        private static void EF(int eachCount)
        {
            GC.Collect();//回收资源
            System.Threading.Thread.Sleep(1);//休息1秒

            PerHelper.Execute(eachCount, "EF", () =>
            {
                using (EFContext conn = new EFContext(Config.connectionString))
                {
                    conn.Set<Test>().Add(GetData());
                    conn.SaveChanges();
                }
            });

            //删除插入数据
            DeleteAddData();
        }

        private static void DeleteAddData()
        {
            var count=Config.GetSugarConn().Deleteable<Test>().Where(it => it.F_String == "test").ExecuteCommand();
            Console.WriteLine("删除：刚插入" + count + "条");
        }

        private static Test GetData()
        {
            return new Test()
            {
                F_Bool = true,
                F_Byte = 0,
                F_DateTime = DateTime.Now,
                F_Decimal = 1,
                F_Double = 11,
                F_Float = 11,
                F_Guid = Guid.Empty,
                F_String = "test",
                F_Int16 = 1,
                F_Int32 = 1,
                F_Int64 = 1

            };
        }
    }
}
