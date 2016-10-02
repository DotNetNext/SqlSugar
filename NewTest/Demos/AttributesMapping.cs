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
                    .Where(it => it.className.Contains("小")).OrderBy(it => it.classSchoolId).Select<V_Student>(it => new V_Student() { id = it.classId, name = it.className }).ToList();
                var list2 = db.Queryable<TestStudent>()
                    .JoinTable<TestSchool>((s1, s2) => s1.classSchoolId == s2.classId)
                    .OrderBy<TestSchool>((s1, s2) => s1.classId)
                    .Select<TestStudent, TestSchool, V_Student>((s1, s2) => new V_Student() { id = s1.classId, name = s1.className, SchoolName = s2.className }).ToList();

                //添加
                TestStudent s = new TestStudent();
                s.className = "属性名";
                s.classSchoolId = 1;
                var id = db.Insert(s);
                s.classId = id.ObjToInt();

                //更新
                db.Update(s);
                db.Update<TestStudent, int>(s, 100);
                db.Update<TestStudent>(s, it => it.classId == 100);
                db.SqlBulkReplace(new List<TestStudent>() { s });

                //删除
                db.Delete<TestStudent>(it => it.classId == 100);
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

            public int isOk { get; set; }
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

            public int AreaId = 1;
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
