using Dapper;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceTest.TestItems
{
    public class TestSql
    {
        public void Init(OrmType type)
        {
            Console.WriteLine("测试SQL查询的速度");
            var eachCount = 3000;

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
                }
            }

        }

        private static void SqlSugar(int eachCount)
        {
            GC.Collect();//回收资源
            System.Threading.Thread.Sleep(1);//休息1秒

            PerHelper.Execute(eachCount, "SqlSugar", () =>
            {
                using (SqlSugarClient conn = new SqlSugarClient(new ConnectionConfig() { InitKeyType = InitKeyType.SystemTable, ConnectionString = Config.connectionString, DbType = DbType.SqlServer }))
                {
                    var list2 = conn.Ado.SqlQuery<Test>("select top 10 * from Test"); ;
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
                    var list = conn.Query<Test>("select top 10 * from Test");
                }
            });
        }
    }
}
