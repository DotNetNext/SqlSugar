using OrmTest.UnitTest.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class UCustom017
    {

        public static void Init()
        {
            var db = NewUnitTest.Db;
            db.Queryable<Order>()
                .Where(it => it.Id==SqlFunc.Subqueryable<Order>().Where(z=>z.Id==it.Id).GroupBy(z=>z.Id).Select(x=>x.Id) )
                .ToList();
            db.Queryable<Order>()
             .Where(it => it.Id != SqlFunc.Subqueryable<Order>().Where(z => z.Id == it.Id).GroupBy(z => z.Id).Select(x => x.Id))
             .ToList();
        }

    }
}
