using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class UCustom01 
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;
            var x = "aaaaaaaaadssadsssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssaaa" +
              "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            db.CodeFirst.InitTables<UnitClob>();
            db.Insertable(new List<UnitClob>() {
             new UnitClob(){  Clob="aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"},
              new UnitClob(){ 
                  Clob=x+x+x+x+x+x+x+x}
            }).ExecuteCommand();

            db.CodeFirst.InitTables<UnitDate1>();
            db.Insertable(new UnitDate1() { date1 = DateTime.Now.AddDays(10), date2 = DateTime.Now.AddDays(10) }
            ).ExecuteCommand();
            var list=db.Queryable<UnitDate1>().ToList();
            string p1 = "p1";
            db.Queryable<Order>().Where(x11 => x11.Name + "a" == x11.Name).ToList();
            db.Queryable<Order>().Where(x11 => x11.Name == x11.Name + "a").ToList();
            db.Queryable<Order>().Where(x11 => "a" + x11.Name + p1 == x11.Name).ToList();
            db.Queryable<Order>().Where(x11 => x11.Name == "a" + x11.Name + p1).ToList();
            db.Queryable<Order>().Where(x11 => SqlFunc.ToString("a" + p1 + x11.Name) == x11.Name).ToList();
            db.Updateable<Order>()
                .SetColumns(x1 => x1.Name == x1.Name + "a")
                .Where(z => z.Id == 1)
                .ExecuteCommand();
            db.Updateable<Order>()
              .SetColumns(x1 => new Order() { Name = x1.Name + "a" })
              .Where(z => z.Id == 1)
              .ExecuteCommand();
        }

        public class UnitDate1
        {
            [SugarColumn(ColumnDataType = "date")]
            public DateTime date1 { get; set; }
            [SugarColumn(ColumnDataType = "timestamp")]
            public DateTime date2 { get; set; }
        }
        public class UnitClob 
        {
            [SugarColumn(IsPrimaryKey =true)]
            public Guid Id { get; set; }

            [SugarColumn(ColumnDataType ="clob", IsNullable =true)]
            public string Clob { get; set; }
        }
     
    }
}
