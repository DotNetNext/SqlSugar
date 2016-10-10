using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using SqlSugar;
using Dapper;
using Dapper.Contrib;
using Dapper.Contrib.Extensions;
using Models.PkDapper;
namespace PkDapper.Demos
{
    public class WarmUp
    {
        public WarmUp()
        {
            Console.WriteLine("开启预热");
            //预热处理
            for (int i = 0; i < 2; i++)
            {
                using (SqlConnection conn = new SqlConnection(PubConst.connectionString))
                {
                    var list = conn.QueryFirst<Test>("select top 1 * from Test");
                }

                using (SqlSugarClient conn = new SqlSugarClient(PubConst.connectionString))
                {
                    var list = conn.Queryable<Test>().Where(it => 1 == 2).ToList();
                }
            }
            Console.WriteLine("预热完毕");
            Console.WriteLine("----------------比赛开始-------------------");
        }
    }
}
