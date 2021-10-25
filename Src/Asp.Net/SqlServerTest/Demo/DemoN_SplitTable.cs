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
            };

            db.CodeFirst.SplitTables().InitTables<OrderSpliteTest>();
            var list=db.Queryable<OrderSpliteTest>().SplitTable(tabs => tabs.Take(3)).ToList();

            var list2 = db.Queryable<OrderSpliteTest>().SplitTable(tabs => tabs.Where(it=> it.Date>=DateTime.Now.AddYears(-2))).ToList();

            var x = db.Deleteable<OrderSpliteTest>().Where(it=>it.Pk==Guid.NewGuid()).SplitTable(tabs => tabs.Take(3)).ExecuteCommand();

            var x2 = db.Updateable<OrderSpliteTest>()
                .SetColumns(it=>it.Name=="a")
                .Where(it => it.Pk == Guid.NewGuid())
                .SplitTable(tabs => tabs.Take(3))
                .ExecuteCommand();

            var x3 = db.Insertable(new OrderSpliteTest() { Name="A" }).SplitTable(SplitType.Day).ExecuteCommand();
            var x4 = db.Insertable(new OrderSpliteTest() { Name = "A" }).SplitTable(SplitType.Day,it=>it.Time).ExecuteCommand();
            Console.WriteLine("#### CodeFirst end ####");
        }

        [SqlSugar.SugarTable("Taxxx0101_{year}{month}{day}")]
        public class OrderSpliteTest 
        {
            [SugarColumn(IsPrimaryKey =true)]
            public  Guid  Pk{ get; set; }
            public string Name { get; set; }
            [SugarColumn(IsNullable =true)]
            public DateTime Time { get; set; }
        }
    }
}
