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
        }

        public class AttributeTest
        {
            [SugarColumn(ColumnName = "Id")]
            public int Aid { get; set; }
            public string Name { get; set; }
            [SugarColumn(IsOnlyIgnoreInsert = true)]
            public int CreateTime { get; set; }
        }
    }
}
