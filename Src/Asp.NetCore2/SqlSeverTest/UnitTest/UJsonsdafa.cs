using System;
using System.Collections.Generic;
using System.Text;
using SqlSugar;
namespace OrmTest
{
    public class UJsonsdafa
    {
        public static void Init()
        {


            SqlSugarClient db = NewUnitTest.Db;
            if (!db.DbMaintenance.IsAnyTable("t1", false))
            {
                db.CodeFirst.InitTables<t1>();
                db.CodeFirst.InitTables<t2>();
                var result1 = db.Insertable(new t1 { id = 1, f1 = "f1" }).ExecuteCommand();
                var result2 = db.Insertable(new t2
                {
                    id = 1,
                    json1 = new field_t2_json1 { a = "a", b = "b", c = new field_t2_f1_c() { x = "x", y = "y" } },
                    json2 = null
                }).ExecuteCommand();
            }

            var list0 = db.Queryable<t1>().InnerJoin<t2>((m, j) => m.id == j.id).Where((m) => m.id == 1).Select((m, j) => new { m, j.json1, j.json2 }).ToList();
            Console.WriteLine(string.Format("{0}:{1}", list0[0].json1.a, list0[0].json1.c.x));  //可以得到json1的值
        }
        public class t1
        {
            [SugarColumn(IsPrimaryKey = true)]
            public int id { get; set; }
            public string f1 { get; set; }
        }
        public class t2
        {
            [SugarColumn(IsPrimaryKey = true)]
            public int id { get; set; }
            [SugarColumn(IsJson = true, IsNullable = true)]
            public field_t2_json1 json1 { get; set; }
            [SugarColumn(IsJson = true, IsNullable = true)]
            public Dictionary<string, string> json2 { get; set; }
        }
        public class field_t2_json1
        {
            public string a { get; set; }
            public string b { get; set; }
            [SugarColumn(IsJson = true, IsNullable = true)]
            public field_t2_f1_c c { get; set; }
        }
        public class field_t2_f1_c
        {
            public string x { get; set; }
            public string y { get; set; }
        }
    }
}
