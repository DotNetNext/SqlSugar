using OrmTest;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest 
{
    public class TestFAST111
    {
 
        public int Sex { get; set; }
        public DateTime Date { get; set; }

        [SqlSugar.SugarColumn(IsPrimaryKey =true)]
        public string Id { get; set; }
        [SqlSugar.SugarColumn(IsNullable  = true)]
        public long X { get; set; }
        [SqlSugar.SugarColumn(IsNullable = true,IsJson =true,ColumnDataType ="varchar(500)")]
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
                DbType = DbType.MySql,
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
            db.CodeFirst.InitTables<TestFAST111>();
            db.Fastest<TestFAST111>().BulkCopy(new List<TestFAST111>() { 
              new TestFAST111(){  Date=DateTime.Now, Id=Guid.NewGuid()+"", Sex=1 , X=111,json=new string[]{ "x"} }
            });
            var data = new List<TestFAST111>() {
              new TestFAST111(){   Date=DateTime.Now, Id=Guid.NewGuid()+"", Sex=2 , X=112,json=new string[]{ "x"} }
            };
            //db.Updateable(data).ExecuteCommand();
            db.Fastest<TestFAST111>().BulkUpdate(data);
            var x = db.Queryable<TestFAST111>().ToList();
 
        }
    }
}
