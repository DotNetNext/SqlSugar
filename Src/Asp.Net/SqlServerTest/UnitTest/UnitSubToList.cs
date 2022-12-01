using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace OrmTest 
{
    public class UnitSubToList
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            var test47 = db.Queryable<Order>().Select(it => new
            {
                disCount = SqlFunc.Subqueryable<Order>().Where(s=>s.Id==it.Id).ToList()
            })
            .ToList();
        }
    }
}
