using SqlSugar;
using SqlSugar.MongoDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbTest 
{
    public class QueryWhere
    {
        public static void Init()
        {
            //创建DB
            var db = DBHelper.DbHelper.GetNewDb();

            //初始化数据
            InitializeStudentData(db);

            //null类型测试
            ValidateStudentData(db);

            //函数
            FilterStudentsByFunc(db);

            //根据bool过滤
            FilterStudentsByBool(db);
        }

        private static void FilterStudentsByBool(SqlSugar.SqlSugarClient db)
        {
            //bool类型测试

            var list1 = db.Queryable<Student>().Where(it => it.Bool == true).ToList();
            //var list2 = db.Queryable<Student>().Where(it => it.Bool).ToList();
            //var list3 = db.Queryable<Student>().Where(it => !it.Bool).ToList();
        }

        private static void FilterStudentsByFunc(SqlSugar.SqlSugarClient db)
        {
            var list = db.Queryable<Student>().Where(it => it.Name.Contains("ck")).ToList();
            if (!list.First().Name.Contains("ck")) Cases.ThrowUnitError();

            var list2 = db.Queryable<Student>().Where(it => it.Name.StartsWith("ck")).ToList();
            if (list2.Any()) Cases.ThrowUnitError();

            var list3 = db.Queryable<Student>().Where(it => it.Name.StartsWith("ja")).ToList();
            if (!list3.Any()) Cases.ThrowUnitError();

            var list4 = db.Queryable<Student>().Where(it => it.Name.EndsWith("ck")).ToList();
            if (!list4.Any()) Cases.ThrowUnitError();

            var list5 = db.Queryable<Student>().Where(it =>it.CreateDateTime==DateTime.Now.Date).ToList();
            var list6 = db.Queryable<Student>().Where(it => it.CreateDateTime == DateTime.Now.AddDays(1)).ToList();
        }

        private static void ValidateStudentData(SqlSugar.SqlSugarClient db)
        {
            var list = db.Queryable<Student>().ToList();
            if (list.First() is { } first && (first.BoolNull != null || first.SchoolIdNull != null)) Cases.ThrowUnitError();
            if (list.Last() is { } last && (last.BoolNull != true || last.SchoolIdNull != 4)) Cases.ThrowUnitError();
        }

        private static void InitializeStudentData(SqlSugar.SqlSugarClient db)
        {
            db.CodeFirst.InitTables<Student>();
            db.DbMaintenance.TruncateTable<Student>();
            db.Insertable(new Student() { Name = "jack",CreateDateTime=DateTime.Now.AddDays(-10), Bool = true, SchoolId = 2 }).ExecuteCommand();
            db.Insertable(new Student() { Name = "tom_null", CreateDateTime = DateTime.Now.AddDays(-110), Bool = false, BoolNull = true, SchoolId = 3, SchoolIdNull = 4 }).ExecuteCommand();
        }

        [SqlSugar.SugarTable("UnitStudent1ssss23s131")]
        public class Student : MongoDbBase
        {
            public string Name { get; set; }

            public bool Bool { get; set; }
            public bool? BoolNull { get; set; }

            public int SchoolId { get; set; }
            public int? SchoolIdNull { get; set; }

            public DateTime CreateDateTime { get; set; }
        }
    }
}
