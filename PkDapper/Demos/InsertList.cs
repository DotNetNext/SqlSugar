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
    public class InsertList : IDemos
    {
 
        public void Init()
        {
            Console.WriteLine("测试插入1000条记录的集合");

            var eachCount = 10;

            /*******************车轮战是性能评估最准确的一种方式***********************/
            for (int i = 0; i < 10; i++)
            {
                //清除
                DeleteAddDatas();

                //dapper
                Dapper(eachCount);


                //清除
                DeleteAddDatas();

                //sqlSugar
                SqlSugar(eachCount);

            }
            Console.WriteLine("SqlSugar批量插入性能，秒杀Dapper一条街。(Dapper并没有优化过)");
        }

        private static void DeleteAddDatas()
        {
            using (SqlSugarClient conn = new SqlSugarClient(PubConst.connectionString))
            {
                conn.Delete<Test>(it => it.F_String == "Test");
            }
        }
        private static List<Test> GetList
        {
            get
            {
                List<Test> list = new List<Test>();
                for (int i = 0; i < 1000; i++)
                {
                    Test t = new Test()
                    {
                        F_Int32 = 1,
                        F_String = "Test",
                        F_Float = 1,
                        F_DateTime = DateTime.Now,
                        F_Byte = 1,
                        F_Bool = true
                    };
                    list.Add(t);
                }
                return list;
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
                    var list = conn.SqlBulkCopy(GetList);
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
                    var list = conn.Insert(GetList);
                }
            });
        }
    }
}
