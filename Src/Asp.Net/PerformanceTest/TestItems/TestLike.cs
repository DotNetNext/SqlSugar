using Dapper;
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
    public class TestLike
    {
        public void Init(OrmType type)
        {
            Database.SetInitializer<EFContext>(null);
            Console.WriteLine("测试SQL查询的速度");
            var eachCount = 1;

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
                    case OrmType.FREE:
                        Free(eachCount);
                        break;
                }
            }
            Console.Write("总计：" + (DateTime.Now - beginDate).TotalMilliseconds / 1000.0);
        }


        private static void Free(int eachCount)
        {
            GC.Collect();//回收资源
            System.Threading.Thread.Sleep(1);//休息1秒
            IFreeSql fsql = new FreeSql.FreeSqlBuilder()
.UseConnectionString(FreeSql.DataType.SqlServer, Config.connectionString)
.UseAutoSyncStructure(false) //自动同步实体结构到数据库
.Build();
            PerHelper.Execute(eachCount, "Free like ", () =>
            {
                var x = "abc";
             var list2 = fsql.Queryable<Test>().Where(it => it.F_String.Contains(x)).ToList();
            });
        }

        private static void SqlSugar(int eachCount)
        {
            GC.Collect();//回收资源
            System.Threading.Thread.Sleep(1);//休息1秒

            PerHelper.Execute(eachCount, "SqlSugar like ", () =>
            {
                SqlSugarClient conn = Config.GetSugarConn();
                var list2 = conn.Queryable<Test>().Where(it => it.F_String.Contains("abc")).ToList(); 
            });
        }

        private static void Dapper(int eachCount)
        {
            throw new Exception("未实现");
        }

        private static void EF(int eachCount)
        {
            throw new Exception("未实现");
        }
    }
}
