using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    internal class Unitatadffaa1
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;

            db.CodeFirst.InitTables<Test001>();
            db.CodeFirst.InitTables<Test002>();
            db.DbMaintenance.TruncateTable<Test001, 
                Test002>();

            //用例代码 
            var result = db.Insertable(new Test001() { id = 1, name = "ceshi", age = 11, sex = "33311" }).ExecuteCommand();//用例代码

            var result2 = db.Insertable(new Test002() { id = 1, name = "ceshi2", age = 22, sex = "33322" }).ExecuteCommand();//用例代码
            try
            { 

                var data = db.Queryable<Test001>().Where(x => x.id == 1)
                    .Select(x => new
                    {
                        cs = SqlFunc.Subqueryable<Test002>()
                        .ToList(g => new Test003
                        {
                            a = true? "1" : "2"
                        }, true)
                    }).ToList();

                string xx = "1";
                var data2 = db.Queryable<Test001>().Where(x => x.id == 1)
                 .Select(x => new
                 {
                     cs = SqlFunc.Subqueryable<Test002>()
                     .ToList(g => new Test003
                     {
                         a = xx == "1" ? "1" : "2"
                     }, true)
                 }).ToList();

                // 没问题：

                var data3 = db.Queryable<Test001>().Where(x => x.id == 1)
                  .Where(x => x.id == 1)
                 .Select(x => new
                 {
                     cs = SqlFunc.Subqueryable<Test002>()
                     .LeftJoin<Test001>((g, d) => g.id == d.id)
                     .ToList(g => new Test003
                     {
                         a = g.sex
                     }, true)
                 }).ToList(); 

            }
            catch (Exception)
            {
                throw;
            }
        }

        [SugarTable("unitTest001aa")]
        internal class Test001
        {
            [SugarColumn(IsPrimaryKey = true)]
            public int id { get; set; }
            public string name { get; set; }
            public string sex { get; set; }
            public int age { get; set; }
        }
        [SugarTable("unitTest003aa")]
        internal class Test003
        {
            public string a { get; set; }
            public string b { get; set; }
        }
        internal class Test002
        {
            [SugarColumn(IsPrimaryKey = true)]
            public int id { get; set; }
            public string name { get; set; }
            public string sex { get; set; }
            public int age { get; set; }
        }
    }
}
