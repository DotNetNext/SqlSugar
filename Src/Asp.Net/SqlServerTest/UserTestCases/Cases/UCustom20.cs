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
            var test1 = db.Queryable<Order>()
                 .ToList(z => new
                 {
                     name1 = new { z.Id, z.Name, ZId = 100 }
                 }).First();

            if (test1.name1.Id != 1 || test1.name1.Name != "jack" || test1.name1.ZId != 100)
            {
                throw new Exception("unit error");
            }

            var test2 = db.Queryable<Order>()
           .ToList(z => new
           {
               name1 = new { z.Id, z.Name, ZId = z.Id.ToString() }
           }).First();

            if (test2.name1.Id != 1 || test2.name1.Name != "jack" || test2.name1.ZId != "1")
            {
                throw new Exception("unit error");
            }

            var test3 = db.Queryable<Order>()
            .Take(2)
             .Select(i => new TestDTO
             {
                 SubOne = new TestSubDTO { NameOne = "a1", NameTwo = i.Id.ToString() },
                 //   SubTwo = new TestSubDTO { NameOne = i.Name, NameTwo = i.Name }
             })
             .First();

            if (test3.SubOne.NameOne != "a1" || test3.SubOne.NameTwo != "1")
            {
                throw new Exception("unit error");
            }

            var test4 = db.Queryable<Order>()
            .Take(2)
             .Select(i => new TestDTO
             {
                 SubOne = new TestSubDTO { NameOne = "a1", NameTwo = i.Name },
                 //SubTwo = new TestSubDTO { NameOne = i.Name, NameTwo = i.Name }
             })
            .ToList().First();
            if (test4.SubOne.NameOne != "a1" || test4.SubOne.NameTwo != "jack")
            {
                throw new Exception("unit error");
            }

            var test5 = db.Queryable<Order>()
           .Take(2)
            .Select(i => new TestDTO
            {
                SubOne = new TestSubDTO { NameOne = i.Name, NameTwo = i.Name },
                //SubTwo = new TestSubDTO { NameOne = i.Name, NameTwo = i.Name }
            })
           .ToList().First();
            if (test5.SubOne.NameOne != "jack" || test5.SubOne.NameTwo != "jack")
            {
                throw new Exception("unit error");
            }

            var test6 = db.Queryable<Order>()
            .Take(2)
             .Select(i => new TestDTO
             {
                 SubOne = new TestSubDTO { NameOne = i.Name+"1", NameTwo = i.Name+"2" },
                 SubTwo = new TestSubDTO { NameOne = i.Name+"3", NameTwo = i.Name+"4" }
             })
            .ToList().First();
            if (test6.SubOne.NameOne != "jack1" || test6.SubOne.NameTwo != "jack2"||
                test6.SubTwo.NameOne != "jack3" || test6.SubTwo.NameTwo != "jack4")
            {
                throw new Exception("unit error");
            }
            var list = db.Queryable<Order>().ToList();
            var test7 = db.Queryable<Order>()
           .LeftJoin<Order>((x,y)=>x.Id==y.Id)
          .Take(2)
           .Select((x,y) => new TestDTO
           {
               SubOne = new TestSubDTO { NameOne = x.Name, NameTwo = x.Name },
               SubTwo = new TestSubDTO { NameOne = x.Name, NameTwo = x.Name }
           })
          .ToList() ;

            if (test7.First().SubTwo.NameTwo != "jack"|| test7.First().SubTwo.NameOne != "jack") 
            {
                throw new Exception("unit error");
            }
            if (test7.First().SubOne.NameTwo != "jack" || test7.First().SubOne.NameOne != "jack")
            {
                throw new Exception("unit error");
            }

            var test8 = db.Queryable<Order>()
                .LeftJoin<Order>((x, y) => x.Id == y.Id)
               .Take(2)
                .Select((x, y) => new TestDTO
                {
                    SubOne = new TestSubDTO { NameOne = "a", NameTwo =x.Name } 
                })
               .ToList();

            if (test8.First().SubOne.NameOne != "a" || test8.First().SubOne.NameTwo != "jack")
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
            public string NameOne { get; set; }

            public string NameTwo { get; set; }
        }
    }
}
