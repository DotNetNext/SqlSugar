using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NewTest.Dao;
using Models;
using System.Data.SqlClient;
using SqlSugar;
namespace NewTest.Demos
{
    //通过属性的方法设置别名表和别名字段
    public class AttributesMapping : IDemos
    {

        public void Init()
        {
            Console.WriteLine("启动AttributesMapping.Init");
            using (var db = SugarDao.GetInstance())
            {
                db.IsIgnoreErrorColumns = true;//忽略非数据库列

                var list = db.Queryable<TestStudent>().ToList();

            }
        }

        [SugarMapping(TableName = "Student")]
        public class TestStudent
        {

            [SugarMapping(ColumnName = "id")]
            public int classId { get; set; }


            public string name { get; set; }
        }
    }
}
