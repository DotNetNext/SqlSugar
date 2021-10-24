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

            var x = db.Deleteable<OrderSpliteTest>().Where(it=>it.Pk==Guid.NewGuid()).SplitTable(tabs => tabs.Take(3)).ExecuteCommand();
            Console.WriteLine("#### CodeFirst end ####");
        }

        [SqlSugar.SugarTable("Taxxx0101_{year}{month}{day}")]
        public class OrderSpliteTest 
        {
            [SugarColumn(IsPrimaryKey =true)]
            public  Guid  Pk{ get; set; }
            public string Name { get; set; }
        }
    }
}
