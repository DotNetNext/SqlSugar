using Dapper.Contrib.Extensions;
using PerformanceTest.Models;
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
    public class TestOneToMany
    {

        public void Init(OrmType type)
        {
            Database.SetInitializer<EFContext>(null);
            Console.WriteLine("测试一次读取1条数据的速度");
            var eachCount = 20;

            var beginDate = DateTime.Now;
            for (int i = 0; i < 20; i++)
            {
                switch (type)
                {
                    case OrmType.SqlSugar:
                        SqlSugar(eachCount);
                        break;
                    //case OrmType.Dapper:
                    //    Dapper(eachCount);
                    //    break;
                    //case OrmType.EF:
                    //    EF(eachCount);
                    //    break;
                    case OrmType.FREE:
                        Free(eachCount);
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
              
                    var list2 = conn.Queryable<PerformanceTest.Models2.Group>()
                    .Mapper(it => it.AUsers, it => it.AUsers.First().AGroupId).Take(20).ToList();
                   
                }
            });
        }
        private static void Free(int eachCount)
        {
            GC.Collect();//回收资源
            System.Threading.Thread.Sleep(1);//休息1秒
            IFreeSql fsql = new FreeSql.FreeSqlBuilder()
.UseConnectionString(FreeSql.DataType.SqlServer, Config.connectionString)
.UseAutoSyncStructure(false) //自动同步实体结构到数据库
.Build();
            PerHelper.Execute(eachCount, "free", () =>
            {

                var list2 = fsql.Queryable<Group>().IncludeMany(it => it.AUsers).Take(20).ToList();
                //用.First(it=>it.Id==1)报错
            });
        }
        private static void Dapper(int eachCount)
        {
         
        }


        private static void EF(int eachCount)
        {
            
        }
    }
}
