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
            //db.DbMaintenance.DropTable("TESTFAST11");
            //db.DbMaintenance.TruncateTable<TestFAST11>();
            db.CodeFirst.InitTables<TestFAST11>();
            //db.Insertable<TestFAST11>(new List<TestFAST11>() {
            //  new TestFAST11(){  Date=DateTime.Now, Id=Guid.NewGuid()+"", Sex=1 , }
            //}).UseOracle().ExecuteBulkCopy();
            //db.Fastest<TestFAST11>().BulkCopy(new List<TestFAST11>() {
            //  new TestFAST11(){  Date=DateTime.Now, Id=Guid.NewGuid()+"", Sex=1 , }
            //});
        
            //db.Updateable(data).ExecuteCommand();
            for (int i = 0; i < 2; i++)
            {
                db.Fastest<TestFAST11>().BulkCopy(new List<TestFAST11> { new TestFAST11() { Date = DateTime.Now, Id = Guid.NewGuid() + "", Sex = 1 } });
                var x = db.Queryable<TestFAST11>().ToList();
                var updaterows = db.Fastest<TestFAST11>().BulkUpdate(x); 
            }
        }
    }
}
