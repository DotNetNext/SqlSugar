using OrmTest.UnitTest.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class UCustom010
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Unitasfa1sadfa>();
            var list = db.Queryable<Unitasfa1sadfa>()
               .Select(x => new { x = Convert.ToBoolean(x.ItemId),
                   x1 = SqlFunc.ToBool(x.ItemId)
               })
               .ToList();
        }
        public class Unitasfa1sadfa
        {
            [SugarColumn(IsPrimaryKey = true)]
            public Guid Id { get; set; }
            [SugarColumn(IsNullable = true)]
            public bool ItemId { get; set; }
 
        }

     
    }
}
