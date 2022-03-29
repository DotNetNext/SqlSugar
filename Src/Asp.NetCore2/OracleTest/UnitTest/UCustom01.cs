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

            db.CodeFirst.InitTables<UnitDate>();
            db.Insertable(new List<UnitDate>() {
             new UnitDate(){   date1=DateTime.Now,date2=DateTime.Now} 
            }).ExecuteCommand();
            var list=db.Queryable<UnitDate>().ToList();
        }

        public class UnitDate 
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
