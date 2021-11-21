using OrmTest;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest 
{
    public class TestFAST11
    {
  

        public int Sex { get; set; }
        public DateTime Date { get; set; }

        [SqlSugar.SugarColumn(IsPrimaryKey =true)]
        public string Id { get; set; }
  
    }
 
    public class DemoO_Fastest
    {
        public static void Init()
        {
            Console.WriteLine("");
            Console.WriteLine("#### Insertable Start ####");

            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                DbType = DbType.Oracle,
                ConnectionString = Config.ConnectionString,
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true,
                AopEvents = new AopEvents
                {
                    OnLogExecuting = (sql, p) =>
                    {
                        Console.WriteLine(sql);
                        Console.WriteLine(string.Join(",", p?.Select(it => it.ParameterName + ":" + it.Value)));
                    }
                }
            });
        
            db.CodeFirst.InitTables<TestFAST11>();
            //db.Insertable<TestFAST11>(new List<TestFAST11>() {
            //  new TestFAST11(){  Date=DateTime.Now, Id=Guid.NewGuid()+"", Sex=1 , }
            //}).UseOracle().ExecuteBulkCopy();
            //db.Fastest<TestFAST11>().BulkCopy(new List<TestFAST11>() {
            //  new TestFAST11(){  Date=DateTime.Now, Id=Guid.NewGuid()+"", Sex=1 , }
            //});
            var data = new List<TestFAST11>() {
              new TestFAST11(){  Date=DateTime.Now, Id=Guid.NewGuid()+"", Sex=1  }
            };
            //db.Updateable(data).ExecuteCommand();
            db.Fastest<TestFAST11>().BulkCopy(data);
            var x = db.Queryable<TestFAST11>().ToList();
 
        }
    }
}
