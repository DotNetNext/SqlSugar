using Newtonsoft.Json.Linq;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class UJsonFunc
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<UnitJsonTest22>();
            db.DbMaintenance.TruncateTable<UnitJsonTest22>();


            db.Insertable(new UnitJsonTest22() { A = JObject.Parse(db.Utilities.SerializeObject(new { Id = 1 }))}).ExecuteCommand();


            db.Insertable(new UnitJsonTest22() { A = JObject.Parse(db.Utilities.SerializeObject(new { Id = 2,hh=new string[] { "1","2"},zz=new { b=111} })) }).ExecuteCommand();

            db.Insertable(new UnitJsonTest22() { A = JObject.Parse(db.Utilities.SerializeObject(new { Id = 2, zz = new { b = 222 } })) }).ExecuteCommand();
            


            var list = db.Queryable<UnitJsonTest22>()
                .Where(x =>Convert.ToInt32(SqlFunc.JsonField(x.A,"zz","b"))  == 111)
                .ToList();

            if (list.Count != 1) { throw new Exception("unit error"); }

            var list2 = db.Queryable<UnitJsonTest22>()
                  .Where(x => Convert.ToInt32(SqlFunc.JsonField(x.A, "Id")) == 1)
                  .ToList();

            if (list2.Count != 1) { throw new Exception("unit error"); }


            var list3 = db.Queryable<UnitJsonTest22>().Select(it=>new { x = SqlFunc.JsonParse(it.A)}).ToList();


            db.CodeFirst.InitTables<UnitJsonTest221>();
            db.Insertable(new UnitJsonTest221() { A = JArray.Parse("[1,2]") }).ExecuteCommand();
            var list4 = db.Queryable<UnitJsonTest221>().Select(it => new { x = SqlFunc.JsonArrayLength(it.A) }).ToList();

            if (list4.First().x != 2) { throw new Exception("unit error"); }

            var list5 = db.Queryable<UnitJsonTest22>().Select(it => new { x = SqlFunc.JsonContainsFieldName(it.A,"id") }).ToList();
            if (list5.First().x != false) { throw new Exception("unit error"); }

            var list6 = db.Queryable<UnitJsonTest22>().Select(it => new { x = SqlFunc.JsonContainsFieldName(it.A, "Id") }).ToList();
            if (list6.First().x != true) { throw new Exception("unit error"); }


            var list7 = db.Queryable<UnitJsonTest22>()
           .Where(x =>  SqlFunc.JsonField(x.A, "hh","{int:1}")  == "2")
           .ToList();

            var list8= db.Queryable<UnitJsonTest22>()
             .Where(x => SqlFunc.JsonLike(x.A,"hh"))
             .ToList();

            db.DbMaintenance.DropTable<UnitJsonTest22>();
        }
        public class UnitJsonTest22 
        {
            [SqlSugar.SugarColumn(IsJson =true)]
            public JObject A { get; set; }
        }
        public class UnitJsonTest221
        {
            [SqlSugar.SugarColumn(IsJson = true)]
            public JArray A { get; set; }
        }
    }
}
