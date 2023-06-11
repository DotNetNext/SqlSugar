using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest 
{
    internal class UQueryableByObject
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;
            var type = typeof(Order);
            var data=db.QueryableByObject(type)
                .Where("id=1").ToList();
            var data2=db.QueryableByObject(type)
                .Where("id=@id",new {id=1 }).ToList();
            var data3 = db.QueryableByObject(type)
                .OrderBy("id").Where("id=@id", new { id = 1 }).ToList();
            var data4 = db.QueryableByObject(type)
                .GroupBy("id").Having("count(name)>0").Select("Id").ToList();
        }
    }
}
