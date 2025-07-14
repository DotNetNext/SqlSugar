using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    internal class Unitdfayssf
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Order>();
            var list=db.Queryable<Order>()
              .Where(x => x.CreateTime > DateTime.Now.AddDays(-6)
                                          && (x.Status & OrderStatus.Aborted) == 0
                                          && (x.Status & OrderStatus.Executed) == 0)
              .ToList();
        }
        [SugarTable("Unitsdfadsfydsly")]

        public class Order
        {
            [SugarColumn(IsPrimaryKey = true)]
            public int Id { get; set; }
            public DateTime CreateTime { get; set; }
            public OrderStatus Status { get; set; }
        }
        public enum OrderStatus
        {
            Aborted = 0x04,
            Executed = 0x40
        }

        public class TestDateTime
        {
            [SugarColumn(IsPrimaryKey = true)]
            public int Id { get; set; }
            public DateTime CreateTime { get; set; }
            public DateTime? UpdateTime { get; set; }
        }
    }

}
