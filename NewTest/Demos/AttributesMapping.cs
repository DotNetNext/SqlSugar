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
            using (var db = DBManager.GetInstance())
            {

                //查询
                var list = db.Queryable<TestStudent>()
                    .Where(it=>it.className.Contains("杰")).OrderBy(it=>it.classSchoolId).ToList();
                var list2 = db.Queryable<TestStudent>()
                    .JoinTable<TestSchool>((s1,s2)=>s1.classSchoolId==s2.classId).Select("s1.name,s2.name as schname")
                    .OrderBy<TestSchool>((s1,s2)=>s1.classId).ToDynamic();

                //添加
                TestStudent s = new TestStudent();
                s.className = "属性名";
                s.classSchoolId = 1;
                db.Insert(s);

            }
        }

        /// <summary>
        /// 属性只作为初始化映射，SetMappingTables和SetMappingColumns可以覆盖
        /// </summary>
        [SugarMapping(TableName = "Student")]
        public class TestStudent
        {

            [SugarMapping(ColumnName = "id")]
            public int classId { get; set; }

             [SugarMapping(ColumnName = "name")]
            public string className { get; set; }

              [SugarMapping(ColumnName = "sch_id")]
             public int classSchoolId { get; set; }
        }

        /// <summary>
        /// 属性只作为初始化映射，SetMappingTables和SetMappingColumns可以覆盖
        /// </summary>
        [SugarMapping(TableName = "School")]
        public class TestSchool
        {

            [SugarMapping(ColumnName = "id")]
            public int classId { get; set; }

            [SugarMapping(ColumnName = "name")]
            public string className { get; set; }
        }
    }

    public class DBManager
    {

        public static SqlSugarClient GetInstance()
        {
            var db = new SqlSugarClient(SugarDao.ConnectionString);
            db.IsEnableAttributeMapping = true;//启用属性映射
            db.IsIgnoreErrorColumns = true;//忽略非数据库列



            db.IsEnableLogEvent = true;//启用日志事件
            db.LogEventStarting = (sql, par) => { Console.WriteLine(sql + " " + par + "\r\n"); };


            return db;
        }
    }
}
