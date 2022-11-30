using Microsoft.SqlServer.Server;
using Newtonsoft.Json.Linq;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
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

            db.CodeFirst.InitTables<UnitJsonArray>();
            db.DbMaintenance.TruncateTable<UnitJsonArray>();
            db.Insertable(new UnitJsonArray() { a = new int[] { 1, 2, 3 }, b = new string[] { "a", "b" } }).ExecuteCommand();
            db.Insertable(new UnitJsonArray() { a = new int[] { 5 }, b = new string[] { "c", "d" } }).ExecuteCommand();
            var isBool=db.Queryable<UnitJsonArray>().Any(it => SqlFunc.JsonArrayAny(it.a, 1));
            var isBool2 = db.Queryable<UnitJsonArray>().Any(it => SqlFunc.JsonArrayAny(it.a, 4));

            var isBool1 = db.Queryable<UnitJsonArray>().Any(it => SqlFunc.JsonArrayAny(it.b, "a"));
            var isBool22 = db.Queryable<UnitJsonArray>().Any(it => SqlFunc.JsonArrayAny(it.b, "e"));

            if (isBool == false || isBool2 == true || isBool1 == false || isBool22 == true) 
            {
                throw new Exception("unit test");
            }
            db.CodeFirst.InitTables<UnitJsonTest2222>();
            db.Insertable(new UnitJsonTest2222() {
                A =JArray.Parse(db.Utilities.SerializeObject( new List<Order>() { new Order() { Id = 1, Name = "a" } }.ToList()))
            }).ExecuteCommand();
            var isAny=db.Queryable<UnitJsonTest2222>().Any(it => SqlFunc.JsonListObjectAny(it.A, "Name", "a"));
            var isAny2 = db.Queryable<UnitJsonTest2222>().Any(it => SqlFunc.JsonListObjectAny(it.A, "Name", "b"));

            var isAny21 = db.Queryable<UnitJsonTest2222>().Any(it => SqlFunc.JsonListObjectAny(it.A, "Id", 1));
            var isAny22 = db.Queryable<UnitJsonTest2222>().Any(it => SqlFunc.JsonListObjectAny(it.A, "Id", 2));

            if (isAny == false || isAny21 == false || isAny2 == true || isAny22 == true) 
            {
                throw new Exception("unit test");
            }
        }

        public class UnitJsonArray
        {
            [SqlSugar.SugarColumn(IsJson =true)]
            public int[] a { get; set; }
            [SqlSugar.SugarColumn(IsJson = true)]
            public string[] b { get; set; }
        }
    public class UnitJsonTest2222
    {
        [SqlSugar.SugarColumn(IsJson = true)]
        public JArray A { get; set; }
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
