using System;
using System.Collections.Generic;
using System.Text;

namespace OrmTest
{
    internal class UnitBool
    { 
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            var xx=db.Queryable<Order>()
                .Select(it => new
                {   id=it.Id,
                    x=it.Id==1
                }).ToList();
        }

    }
}
