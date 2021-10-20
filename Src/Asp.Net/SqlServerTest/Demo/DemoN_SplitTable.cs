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
            db.CodeFirst.SplitTables().InitTables<OrderSpliteTest>();
            db.Queryable<OrderSpliteTest>().SplitTable(it => it.Take(3)).ToList();
            Console.WriteLine("#### CodeFirst end ####");
        }

        [SqlSugar.SugarTable("Order{year}{month}{day}")]
        public class OrderSpliteTest 
        {
            [SugarColumn(IsPrimaryKey =true)]
            public  Guid  Pk{ get; set; }
            public string Name { get; set; }
        }
    }
}
