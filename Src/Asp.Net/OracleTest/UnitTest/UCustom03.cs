using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace OrmTest
{
    public class UCustom03
    {
       public  static void Init()
        {
            var db = NewUnitTest.Db;
            db.Aop.OnLogExecuting = (sql, pars) =>
            {
                Console.WriteLine($"SqlSugar[Info]：{UtilMethods.GetSqlString(DbType.SqlServer, sql, pars) }");

            };
            //建表 
            if (!db.DbMaintenance.IsAnyTable("Test0011", false))
            {
                db.CodeFirst.InitTables<Test001111>();
            }


            //用例代码 
            var dt = DateTime.Now.Date;
            var result = db.Insertable(new Test001111() { id = dt }).ExecuteCommand();//用例代码
            var res = db.Queryable<Test001111>().WhereClass(new Test001111() { id = dt }).ToList();
            //Console.WriteLine(result);
            //Console.WriteLine("用例跑完");
            //Console.ReadKey();
        }
        //建类
        public class Test001111
        {
            [SqlSugar.SugarColumn(SqlParameterDbType = System.Data.DbType.Date)]
            public DateTime? id { get; set; }
        }
    }
}
