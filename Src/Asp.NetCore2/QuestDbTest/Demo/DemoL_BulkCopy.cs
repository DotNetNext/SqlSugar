using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    internal class DemoL_BulkCopy
    {
        public static void Init() 
        {
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                DbType = DbType.QuestDB,
                ConnectionString = Config.ConnectionString3,
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true
            });

            var list=db.Queryable<Order>().Take(2).ToList();
            Console.WriteLine(db.Queryable<Order>().Count());
            var i=db.RestApi().BulkCopy(list);
            Console.WriteLine(db.Queryable<Order>().Count());
            Console.WriteLine("--2---");
            var i2 = db.RestApi().BulkCopy(list.First());
            Console.WriteLine(db.Queryable<Order>().Count());
            Console.WriteLine("--1---");
            var i3 = db.RestApi().BulkCopyAsync(list).GetAwaiter().GetResult();
            Console.WriteLine(db.Queryable<Order>().Count());
            Console.WriteLine("--2---");
            var i4 = db.RestApi().BulkCopyAsync(list.First()).GetAwaiter().GetResult();
            Console.WriteLine(db.Queryable<Order>().Count());
            Console.WriteLine("--1---");
        }
    }
}
