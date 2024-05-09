using SqlSugar;
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
            Test(new UnitbModel());

           var sql=db.Queryable<Order>().Where(it => SqlFunc.FullTextContains(new string[] {it.Name, it.Name }, "a")).ToSql();
        }

        public static void Test(UnitbModel model)
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<UnitbModel>();
            var xx = db.Queryable<UnitbModel>()
             .Where(it=>it.isOk==!model.isOk)
             .ToList();
        }
        
        public class UnitbModel 
        {
            public bool isOk { get; set; }
        }

    }
}
