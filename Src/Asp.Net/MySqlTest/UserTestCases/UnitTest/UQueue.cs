using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest
{
    public partial class NewUnitTest
    {
        public static void Queue()
        {
            test1();
            test2();
        }
        private static void test2()
        {
            var db = Db;
            db.AddQueue("select 11");
            db.Queryable<Order>().Where(it => false).AddQueue();
            db.AddQueue("select 12");
            var list = db.SaveQueuesAsync<int, Order, int>();
            list.Wait();
            UValidate.Check(list.Result.Item1[0], "11", "Queue");
            UValidate.Check(list.Result.Item2.Count(), 0, "Queue");
            UValidate.Check(list.Result.Item3[0], "12", "Queue");
        }

        private static void test1()
        {
            var db = Db;
            db.AddQueue("select 11");
            db.Queryable<Order>().Where(it => false).AddQueue();
            db.AddQueue("select 12");
            var list = db.SaveQueues<int, Order, int>();
            UValidate.Check(list.Item1[0], "11", "Queue");
            UValidate.Check(list.Item2.Count(), 0, "Queue");
            UValidate.Check(list.Item3[0], "12", "Queue");
        }
    }
}
