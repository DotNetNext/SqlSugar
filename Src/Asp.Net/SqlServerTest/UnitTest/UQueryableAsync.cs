using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace OrmTest
{
    public partial class NewUnitTest
    {
        public static void QueryableAsync()
        {
            q1();
            q2();
            q3();
        }

        private static void q1()
        {
            RefAsync<int> total = 0;
            var count = Db.Queryable<Order>().Count();
            Task t = Db.Queryable<Order>().ToPageListAsync(1, 2, total);
            t.Wait();
            UValidate.Check(count, total.Value, "QueryableAsync");
        }
        private static void q2()
        {
            RefAsync<int> total = 0;
            var count = Db.Queryable<Order>().Count();
            Task t = Db.Queryable<Order>().ToDataTablePageAsync(1, 2, total);
            t.Wait();
            UValidate.Check(count, total.Value, "QueryableAsync");
        }
        private static void q3()
        {
            RefAsync<int> total = 0;
            var count = Db.Queryable<Order>().Count();
            Task t = Db.Queryable<Order>().ToJsonPageAsync(1, 2, total);
            t.Wait();
            UValidate.Check(count, total.Value, "QueryableAsync");
        }
    }
}
