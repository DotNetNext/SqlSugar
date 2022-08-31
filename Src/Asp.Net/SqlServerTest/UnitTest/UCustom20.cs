using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest 
{
    public  class UCustom20
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            var list = db.Queryable<Order>()
            .Take(2)
             .Select(i => new TestDTO
             {
                 SubOne = new TestSubDTO { NameOne = i.Name, NameTwo = i.Name },
                 SubTwo = new TestSubDTO { NameOne = i.Name, NameTwo = i.Name }
             })
             .ToList();
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
