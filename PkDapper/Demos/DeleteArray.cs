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
    public class DeleteArray : IDemos
    {

        public void Init()
        {
            Console.WriteLine("测试删除1000条的集合");

            var eachCount = 10;

            /*******************车轮战是性能评估最准确的一种方式***********************/
            for (int i = 0; i < 10; i++)
            {

                //dapper
                Dapper(eachCount);

                //sqlSugar
                SqlSugar(eachCount);

            }

        }

        /// <summary>
        /// 查询出刚插入的1000条数据
        /// </summary>
        /// <returns></returns>
        private static List<Test> GetDeleteList()
        {
            using (SqlSugarClient conn = new SqlSugarClient(PubConst.connectionString))
            {
                conn.Delete<Test>(it => it.F_String == "Test");
                //插入1000条
                conn.SqlBulkCopy(GetList);
                //查询出插入的1000条
                var list = conn.Queryable<Test>().Where(it => it.F_String == "Test").ToList();
                return list;
            }
        }
        private static List<Test> GetList
        {
            get
            {
                List<Test> list = new List<Test>();
                for (int i = 1; i < 1000; i++)
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
                    var ids = GetDeleteList().Select(it => it.Id).ToArray();
                    var list = conn.Delete<Test,int>(ids);
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
                    var delList = GetDeleteList();
                    var list = conn.Delete(delList);
                }
            });
        }
    }
}
