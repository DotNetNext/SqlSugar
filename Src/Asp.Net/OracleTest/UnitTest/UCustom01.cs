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
