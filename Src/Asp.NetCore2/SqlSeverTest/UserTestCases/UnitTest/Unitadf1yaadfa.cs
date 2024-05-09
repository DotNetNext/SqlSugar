using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    internal class Unitadf1yaadfa
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<UnitOrderfafaadfa>();
            db.CodeFirst.InitTables<UnitOrderItemafdaa>();
            db.Queryable<UnitOrderItemafdaa>()
              .Where(it => it.Order.CreateTime.Value.Date>DateTime.Now&& it.Order.CreateTime.Value.Date > DateTime.Now)
              .ToList();
        }
       
        public class UnitOrderfafaadfa
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }

            public string Name { get; set; }
            public decimal Price { get; set; }
            [SugarColumn(IsNullable = true)]
            public DateTime? CreateTime { get; set; }
            [SugarColumn(IsNullable = true)]
            public int CustomId { get; set; } 
        }
     
        public class UnitOrderItemafdaa
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int ItemId { get; set; }
            public int OrderId { get; set; }
            public decimal? Price { get; set; }
            [SqlSugar.SugarColumn(IsNullable = true)]
            public DateTime? CreateTime { get; set; }
            [Navigate(NavigateType.OneToOne, nameof(OrderId))]
            public UnitOrderfafaadfa Order { get; set; }
        }
    }
}
