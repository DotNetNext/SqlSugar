using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrmTest;

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
            }
        }
    }
}
