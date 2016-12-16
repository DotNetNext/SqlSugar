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
    public class InsertItem : IDemos
    {

        public void Init()
        {
            Console.WriteLine("测试插入单条记录");

            var eachCount = 1000;

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
        }

        private static void DeleteAddDatas()
        {
            using (SqlSugarClient conn = new SqlSugarClient(PubConst.connectionString))
            {
                conn.Delete<Test>(it => it.F_String == "Test");
            }
        }
        private static Test GetItem
        {
            get
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
                return t;
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
                    var list = conn.Insert(GetItem);
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
                    var list = conn.Insert(GetItem);
                }
            });
        }
    }
}
