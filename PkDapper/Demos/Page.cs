using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Dapper;
using Dapper.Contrib;
using Dapper.Contrib.Extensions;
using Models.PkDapper;
using SqlSugar;
using SyntacticSugar;
namespace PkDapper.Demos
{
    public class Page : IDemos
    {
 
        public void Init()
        {
            Console.WriteLine("分页");

            var eachCount = 3000;

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
            System.Threading.Thread.Sleep(2000);//休息2秒

            PerHelper.Execute(eachCount, "SqlSugar", () =>
            {
                using (SqlSugarClient conn = new SqlSugarClient(PubConst.connectionString))
                {
                    var list = conn.Queryable<Test>().OrderBy("id").ToPageList(1, 10);
                }
            });
        }

        private static void Dapper(int eachCount)
        {
            GC.Collect();//回收资源
            System.Threading.Thread.Sleep(2000);//休息2秒

            //正试比拼
            PerHelper.Execute(eachCount, "Dapper", () =>
            {
                using (SqlConnection conn = new SqlConnection(PubConst.connectionString))
                {
                    var list = conn.Query<Test>("select * from(select *,row_number() over(order by id) as r from test  ) t where t.r between @b and @e ", new { b = 1, e = 10 });
                }
            });
        }
    }
}
