using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest 
{
    public class DemoN_SplitTable
    {
        public static void Init()
        {
            Console.WriteLine("");
            Console.WriteLine("#### CodeFirst Start ####");
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                DbType = DbType.SqlServer,
                ConnectionString = Config.ConnectionString,
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true
            });
            db.Aop.OnLogExecuted = (s, p) =>
            {
                Console.WriteLine(s);
                Console.WriteLine(string.Join(",", p?.Select(it => it.ParameterName + ":" + it.Value)));
            };

            //初始化分表
            db.CodeFirst.SplitTables().InitTables<OrderSpliteTest>();

            Console.WriteLine();

            //根据最近3个表进行查询
            var list=db.Queryable<OrderSpliteTest>()
                .SplitTable(DateTime.Now.Date.AddYears(-1),DateTime.Now)
                .ToList();

            var first = db.Queryable<OrderSpliteTest>()
          .SplitTable(DateTime.MaxValue, DateTime.Now)
          .First();//no table


            Console.WriteLine();

            //根据时间选出的表进行查询
            var list2 = db.Queryable<OrderSpliteTest>().SplitTable(tabs => tabs.Where(it=> it.Date>=DateTime.Now.AddYears(-2))).ToList();

            Console.WriteLine();

            //删除数据只在最近3张表执行操作
            var x = db.Deleteable<OrderSpliteTest>().Where(it=>it.Pk==Guid.NewGuid()).SplitTable(tabs => tabs.Take(3)).ExecuteCommand();
           
            Console.WriteLine();

            var tableName = db.SplitHelper<OrderSpliteTest>().GetTableName(DateTime.Now.AddDays(-111));

            var listNull= db.Queryable<OrderSpliteTest>().SplitTable(ta => ta.InTableNames(tableName)).ToList();

            var tableName2 = db.SplitHelper(new OrderSpliteTest() {  Time=DateTime.Now}).GetTableNames();
            var tableName3 = db.SplitHelper(new List<OrderSpliteTest> {
              new OrderSpliteTest() { Time = DateTime.Now },
              new OrderSpliteTest() { Time = DateTime.Now },
              new OrderSpliteTest() { Time = DateTime.Now.AddMonths(-10) }
            }).GetTableNames();
            var x2 = db.Updateable<OrderSpliteTest>()
                .SetColumns(it=>it.Name=="a")
                .Where(it => it.Pk == Guid.NewGuid())
                .SplitTable(tabs => tabs.InTableNames(tableName2))
                .ExecuteCommand();

            Console.WriteLine();

            //按日分表 
            var x3 = db.Insertable(new OrderSpliteTest() { Name="A",Time=DateTime.Now  }).SplitTable().ExecuteCommand();
            var x33 = db.Insertable(new OrderSpliteTest() { Name = "A" ,Time=DateTime.Now.AddDays(-11)}).SplitTable().ExecuteCommand();
            Console.WriteLine();
            ////强制分表类型
            var x4 = db.Insertable(new OrderSpliteTest() { Name = "A" ,Time=DateTime.Now.AddDays(-1) }).SplitTable().ExecuteCommand();

            //分表支持BulkCopy
            db.Fastest<OrderSpliteTest>().SplitTable().BulkCopy(new List<OrderSpliteTest> {
              new OrderSpliteTest() { Pk=Guid.NewGuid(),Name ="a", Time = DateTime.Now },
              new OrderSpliteTest() {Pk=Guid.NewGuid(),Name ="a", Time = DateTime.Now },
              new OrderSpliteTest() {Pk=Guid.NewGuid(),Name ="a", Time = DateTime.Now.AddMonths(-10) }
            });
            var updateList = db.Queryable<OrderSpliteTest>().SplitTable(x1 => x1).Take(10).ToList();
            db.Updateable(updateList).IgnoreColumns(it=>it.Name).SplitTable().ExecuteCommand();


            db.Deleteable(updateList).SplitTable().ExecuteCommand();

            db.Fastest<OrderSpliteTest>().SplitTable().BulkUpdate(db.Queryable<OrderSpliteTest>().SplitTable(it=>it).ToList());
            db.Fastest<OrderSpliteTest>().SplitTable().BulkUpdate(db.Queryable<OrderSpliteTest>().SplitTable(it => it).ToList(),new string[] { "pk"},new string[] { "name"});
            Console.WriteLine("#### CodeFirst end ####");
        }

        [SplitTable(SplitType.Day)]
        [SqlSugar.SugarTable("Taxxx0101_{year}{month}{day}")]
        public class OrderSpliteTest 
        {
            [SugarColumn(IsPrimaryKey =true)]
            public  Guid  Pk{ get; set; }
            public string Name { get; set; }
            [SugarColumn(IsNullable =true)]
            [SplitField]
            public DateTime Time { get; set; }
        }
    }
}
