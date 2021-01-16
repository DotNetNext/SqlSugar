using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class Demo5_SqlQueryable
    {
        public static void Init()
        {
            Console.WriteLine("");
            Console.WriteLine("#### SqlQueryable Start ####");
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                DbType = DbType.Sqlite,
                ConnectionString = Config.ConnectionString,
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true
            });

            int total = 0;
            var list = db.SqlQueryable<Order>("select * from `order`").ToPageList(1, 2, ref total);


            //by expression
            var list2 = db.SqlQueryable<Order>("select * from `order`").Where(it => it.Id == 1).ToPageList(1, 2);
            //by sql
            var list3 = db.SqlQueryable<Order>("select * from `order`").Where("id=@id", new { id = 1 }).ToPageList(1, 2);

            Console.WriteLine("#### SqlQueryable End ####");
        }
    }
}
