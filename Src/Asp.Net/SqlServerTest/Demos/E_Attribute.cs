using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlSugar;
namespace OrmTest.Demo
{
    public class AttributeDemo : DemoBase
    {
        public static void Init()
        {
            var db = GetInstance();
            AttributeTest a = new AttributeTest()
            {
                Name = "attr"
            };
            db.Insertable(a).AS("student").ExecuteCommand();
            var list = db.Queryable<AttributeTest>().AS("student").ToList();
            var list2 = db.Queryable<AttributeTest>().AS("student").Select(it => new AttributeTest() { Aid = it.Aid + 1,CreateTime=DateTime.Now,Name=it.Name }).ToList();
            var s = new AttributeTest2() { Aid = 1,AName="a", CreateTime=DateTime.Now };
            var count = db.Updateable(s).UpdateColumns(it=>new { it.CreateTime,it.AName }).Where(it=>it.Aid==100).ExecuteCommand();
        }

        public class AttributeTest
        {
            [SugarColumn(ColumnName = "Id")]
            public int Aid { get; set; }
            public string Name { get; set; }
            [SugarColumn(IsOnlyIgnoreInsert = true)]
            public DateTime CreateTime { get; set; }
        }
        [SugarTable("student")]
        public class AttributeTest2
        {
            [SugarColumn(ColumnName = "Id")]
            public int Aid { get; set; }
            [SugarColumn(ColumnName = "Name")]
            public string AName { get; set; }
            [SugarColumn(IsOnlyIgnoreInsert = true)]
            public DateTime CreateTime { get; set; }
        }
    }
}
