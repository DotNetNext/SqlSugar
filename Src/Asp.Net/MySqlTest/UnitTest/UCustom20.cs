using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class UCustom20
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;
            db.DbMaintenance.TruncateTable<Order>();
            db.Insertable(new Order() { Id = 1, Name = "jack", CreateTime = DateTime.Now, CustomId = 1 }).ExecuteCommand();
           
            var test8 = db.Queryable<Order>()
                .LeftJoin<Order>((x, y) => x.Id == y.Id)
               .Take(2)
                .Select((x, y) => new TestDTO
                {
                    SubOne = new TestSubDTO { NameOne = false, NameTwo = x.Name }
                })
               .ToList();

            if (test8.First().SubOne.NameOne != false || test8.First().SubOne.NameTwo != "jack")
            {
                throw new Exception("unit error");
            }
        }

        public class TestDTO
        {
            public TestSubDTO SubOne { get; set; }

            public TestSubDTO SubTwo { get; set; }
        }

        public class TestSubDTO
        {
            public bool NameOne { get; set; }

            public string NameTwo { get; set; }
        }
    }
}
