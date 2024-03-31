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

            var list = db.Queryable<Order>().Take(2).ToList();
            Console.WriteLine(db.Queryable<Order>().Count());
            var i = db.RestApi().BulkCopy(list);
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
            Demo1(db);
            Demo2(db);
        }

        private static void Demo1(SqlSugarClient db)
        {
            db.CodeFirst.InitTables<BulkCopyTest>();
            List<BulkCopyTest> bulkCopyTests = new List<BulkCopyTest>();
            bulkCopyTests.Add(new BulkCopyTest() { Id = SnowFlakeSingle.Instance.NextId(), DateTime = DateTime.Now });
            bulkCopyTests.Add(new BulkCopyTest() { Id = SnowFlakeSingle.Instance.NextId(), DateTime = DateTime.Now });
            var rows = db.RestApi().BulkCopy(bulkCopyTests);
            var list2 = db.Queryable<BulkCopyTest>().Take(2).ToList();
        }
        private static void Demo2(SqlSugarClient db)
        {
            db.CodeFirst.InitTables<BulkCopyTest2>();
            List<BulkCopyTest2> bulkCopyTests = new List<BulkCopyTest2>();
            bulkCopyTests.Add(new BulkCopyTest2() { Id = SnowFlakeSingle.Instance.NextId(), DateTime = DateTime.Now });
            bulkCopyTests.Add(new BulkCopyTest2() { Id = SnowFlakeSingle.Instance.NextId(), DateTime = DateTime.Now });
            var rows = db.RestApi().BulkCopy(bulkCopyTests);
            var list2 = db.Queryable<BulkCopyTest2>().Take(2).ToList();
        }
        public class BulkCopyTest 
        {
            [SugarColumn(IsPrimaryKey =true)]
            public long Id { get; set; }
            public DateTime DateTime { get; set; }
            [SugarColumn(IsIgnore =true)]
            public string aaa { get; }
        }
        public class BulkCopyTest2
        {
            [SugarColumn(IsPrimaryKey = true)]
            public long Id { get; set; }
            [SugarColumn(ColumnName ="dt")]
            public DateTime DateTime { get; set; }
            [SugarColumn(IsIgnore = true)]
            public string aaa { get; }
        }
    }
}
