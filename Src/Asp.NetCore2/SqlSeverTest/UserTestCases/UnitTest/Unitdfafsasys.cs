using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class Unitsdfadysssdf 

    { 

       public static void Init()
             
        { 

            var db = NewUnitTest.Db;  

            //建表 



            db.CodeFirst.InitTables<Test>();



            ////清空表



            db.DbMaintenance.TruncateTable<Test>();



            var insertdata = new List<Test>() { new Test() { Name = "漫画", Type = "书本" }, new Test() { Name = "间谍过家家", Type = "漫画" },

                new Test() { Name = "红楼梦", Type = "古籍" } };

            //插入测试数据



            var result = db.Insertable(insertdata).ExecuteCommand();//用例代码



            var data = db.Queryable<Test>().Where(i => i.Type == "书本" && SqlFunc.Subqueryable<Test>().Where(s => s.Name == i.Type).NotAny()).Select(i => new TestName

            {

                Name = i.Name,

                Type = i.Type,

                TypeData = SqlFunc.Subqueryable<Test>().InnerJoin<Test>((a, b) => a.Type == b.Name && i.Type == b.Type).ToList((a, b) => new TestName

                {

                    Name = a.Name,

                    NewName = string.IsNullOrEmpty(i.Name) ? "测试名" : "cc",

                    Type = a.Type,

                    NewType = string.IsNullOrEmpty(i.Type) ? "测试类型" : "dd"

                }, true)



            }).ToList();
            if (data.First().TypeData.First().NewName != "cc") 
            {
                throw new Exception("unit error");
            }
            Console.WriteLine("用例跑完");  
        }



        [SugarTable("UnitTestdassfa")]



        public class Test



        {







            [SugarColumn(ColumnDataType = "nvarchar(50)", ColumnName = "name")]



            public string? Name { get; set; }







            [SugarColumn(ColumnDataType = "nvarchar(50)", ColumnName = "Type")]



            public string? Type { get; set; }







        }





        public class TestName
        {

            public string? Name { get; set; }

            public string? NewName { get; set; }

            public string? Type { get; set; }

            public string? NewType { get; set; }

            public List<TestName> TypeData { get; set; }

        }





    }



} 