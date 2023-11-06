using OrmTest.UnitTest.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest 
{
    public class UCustom07
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;
            db.CurrentConnectionConfig.MoreSettings = new ConnMoreSettings() {
                IsWithNoLockQuery = true
        };
            var query6 = db.Queryable<Order>().LeftJoin(db.Queryable<OrderItem>(),(m, i) => m.Id == i.OrderId)
         .ToList();


        }
        public class Unit06 
        {
            public string Name { get; set; }
            public string Company { get; set; }
            public string Work { get; set; }
        }
        public class UnitPeople
        {
            public string Name { get; set; }
            public UnitJobClass Job { get; set; }
        }

        public class UnitJobClass
        {
            public string Company { get; set; }
            public string Work { get; set; }
        }
    }
}
