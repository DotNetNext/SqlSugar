using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class UnitWeek
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;

            for (int i = 0; i < 20; i++)
            {
                var date = DateTime.Now.AddDays(i);
                var id = db.Insertable(new Order()
                {
                    CreateTime = date,
                    CustomId = 1,
                    Name = "a",
                    Price = 1
                })
               .ExecuteReturnIdentity();

                var data1 = db.Queryable<Order>()
                    .In(id).Select(it => it.CreateTime.DayOfWeek).Single();
                Console.WriteLine(db.Queryable<Order>()
                    .In(id).Select(it => it.CreateTime).Single());
                if (data1 != date.DayOfWeek)
                {
                    throw new Exception("DayOfWeek error");
                }
                var result = db.Queryable<Order>()
                    .In(id)
                .Select(st => new
                {
                    SchoolName0 = -st.Id,
                    SchoolName1 = 0 - SqlFunc.Subqueryable<Order>().Where(s => s.Id == st.Id).Select(s => SqlFunc.AggregateSum(s.Id)),
                    SchoolName2 = -SqlFunc.Subqueryable<Order>().Where(s => s.Id == st.Id).Select(s => s.Id),
                })
                .ToList();
                if (result.First().SchoolName0 != -id || result.First().SchoolName1 != -id || result.First().SchoolName2 != -id)
                {
                    throw new Exception("unit error");
                }
            }
        }
    }
}
