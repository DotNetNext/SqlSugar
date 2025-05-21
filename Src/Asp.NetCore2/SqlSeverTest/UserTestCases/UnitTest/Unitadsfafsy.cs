
using System;



using System.Collections.Generic;



using System.Data;



using System.Linq;



using System.Reflection;



using SqlSugar;



namespace OrmTest 

{



    public class Unitadfasdys



    {



       public  static void Init() 
        { 

            var db = NewUnitTest.Db;  
            //建表 



            db.CodeFirst.InitTables<Test>();



            //清空表



            db.DbMaintenance.TruncateTable<Test>();



            var insertdata = new List<Test>() { new Test() { Name = " 漫画", Type = "书本" }, new Test() { Name = "间谍过家家", Type = "漫画" },

                new Test() { Name = "红楼梦", Type = "古籍" } };

            //插入测试数据



            var result = db.Insertable(insertdata).ExecuteCommand();//用例代码



            var data = db.Queryable<Test>().Where(i => i.Type == "书本" && SqlFunc.Subqueryable<Test>().Where(s => s.Name == i.Type).NotAny()).Select(i => new



            {



                Name = i.Name,



                Type = i.Type,



                TypeData = SqlFunc.Subqueryable<Test>().Where(b => b.Type == i.Name).ToList()



            }).ToList();



            if (data.First().TypeData.Any())
            {
                throw new Exception("unit error");
            }


             



        }



        [SugarTable("UnitTestdafas")]



        public class Test



        {







            [SugarColumn(ColumnDataType = "nvarchar(50)", ColumnName = "name")]



            public string? Name { get; set; }







            [SugarColumn(ColumnDataType = "nvarchar(50)", ColumnName = "Type")]



            public string? Type { get; set; }







        }











    }

}
