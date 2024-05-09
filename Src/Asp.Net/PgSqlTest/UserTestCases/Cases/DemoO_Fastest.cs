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
        [SqlSugar.SugarColumn(IsArray =true, ColumnDataType ="text []")]
        public string[] Array { get; set; }

        public int Sex { get; set; }
        public DateTime Date { get; set; }

        [SqlSugar.SugarColumn(IsPrimaryKey =true)]
        public string Id { get; set; }
        [SqlSugar.SugarColumn(IsNullable  = true)]
        public long X { get; set; }
        [SqlSugar.SugarColumn(IsNullable = true,IsJson =true,ColumnDataType ="json")]
        public string [] json { get; set; }
    }
 
    public class DemoO_Fastest
    {
        public static void Init()
        {
            Console.WriteLine("");
            Console.WriteLine("#### Insertable Start ####");

            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                DbType = DbType.PostgreSQL,
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
            db.Fastest<TestFAST11>().BulkCopy(new List<TestFAST11>() { 
              new TestFAST11(){ Array=new string[]{ "2"}, Date=DateTime.Now, Id=Guid.NewGuid()+"", Sex=1 , X=11,json=new string[]{ "x"} }
            });
            var data = new List<TestFAST11>() {
              new TestFAST11(){ Array=new string[]{ "2"}, Date=DateTime.Now, Id=Guid.NewGuid()+"", Sex=1 , X=11,json=new string[]{ "x"} }
            };
            //db.Updateable(data).ExecuteCommand();
            db.Fastest<TestFAST11>().BulkUpdate(data);
            var x = db.Queryable<TestFAST11>().ToList();
 
        }
    }
}
