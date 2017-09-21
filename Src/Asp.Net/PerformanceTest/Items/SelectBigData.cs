using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Dapper;
using SqlSugar;
using Dapper.Contrib.Extensions;

namespace PerformanceTest.Items
{
    public class SelectBigData  
    {
        /// <summary>
        /// 测试一次读取100万条数据的速度
        /// </summary>
        public void Init()
        {
            Console.WriteLine("测试一次读取100万条数据的速度");
            var eachCount = 1000;

            /*******************车轮战是性能评估最准确的一种方式***********************/
            for (int i = 0; i < 10; i++)
            {

                //dapper
               Dapper(eachCount);

                //sqlSugar
              SqlSugar(eachCount); 
            }
 
        }

        private static void SqlSugar(int eachCount)
        {
            GC.Collect();//回收资源
            System.Threading.Thread.Sleep(1);//休息2秒

            PerHelper.Execute(eachCount, "SqlSugar", () =>
            {
                using (SqlSugarClient conn = new SqlSugarClient(new ConnectionConfig() { InitKeyType=InitKeyType.SystemTable, ConnectionString= PubConst.connectionString, DbType=DbType.SqlServer }))
                {
                 //  var list = conn.Ado.SqlQuery<Test>("select * from test where id=1");
                   var list2 = conn.Queryable<Test>().InSingle(1);
                }
            });
        }

        private static void Dapper(int eachCount)
        {
            GC.Collect();//回收资源
            System.Threading.Thread.Sleep(1);//休息2秒

            //正试比拼
            PerHelper.Execute(eachCount, "Dapper", () =>
            {
                using (SqlConnection conn = new SqlConnection(PubConst.connectionString))
                {
                  var list = conn.Get<Test>(1);
                }
            });
        }
    }
}
