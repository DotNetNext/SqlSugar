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
            var db = DbHelper.GetNewDb();

            //初始化数据
            InitializeStudentData(db);

            //类型测试
            ValidateStudentData(db);

            //函数
            FilterStudentsByFunc(db);

            //根据bool过滤
            FilterStudentsByBool(db);

            //更多
            InsertAndValidateStudent(db);
        }

        private static void InsertAndValidateStudent(SqlSugarClient db)
        {
            db.Insertable(new Student() { Name = "", Bool = true, CreateDateTime = DateTime.Now }).ExecuteCommand();
            var list = db.Queryable<Student>().Where(it => string.IsNullOrEmpty(it.Name)).ToList();
            if (!string.IsNullOrEmpty(list.First().Name)) Cases.ThrowUnitError();
            var pks=db.Insertable(new Student() { Name = "jack", Bool = true, CreateDateTime = DateTime.Now }).ExecuteReturnPkList<string>();
            var data=db.Queryable<Student>().Where(it => it.Id == pks.Last()).Select(it => it.Name).Single();
            var data2 = db.Queryable<Student>().Where(it => it.Id == pks.Last()).Select(it => it.Name).First();
            if (data!="jack"||data2!="jack") Cases.ThrowUnitError();
            var data3 = db.Queryable<Student>().Where(it => it.Name.Equals("jack")).Select(it => it.Name).First();
            if (data3 != "jack") Cases.ThrowUnitError();
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

            var list5 = db.Queryable<Student>().Where(it => it.CreateDateTime == DateTime.Now.Date).ToList();
            var list6 = db.Queryable<Student>().Where(it => it.CreateDateTime == DateTime.Now.AddDays(1)).ToList();

            var dt = DateTime.Now.AddDays(-10).Date;
            var list70 = db.Queryable<Student>().ToList().Where(it => it.CreateDateTime.Date == dt).ToList();
            var list71 = db.Queryable<Student>().Where(it => it.CreateDateTime.Date == dt).ToList();

            if (list71.Count != list70.Count) Cases.ThrowUnitError();

            var ids = list71.Select(it => it.Id).ToList();
            var list72 = db.Queryable<Student>().Where(it => ids.Contains(it.Id)).ToList();
            if (list72.Count != ids.Count) Cases.ThrowUnitError();

            var ids2 = db.Queryable<Student>().Select(it => it.Id).ToList();
            var list73 = db.Queryable<Student>().Where(it => ids2.Contains(it.Id)).ToList();
            if (list73.Count != ids2.Count) Cases.ThrowUnitError();


            string[] ids3 = db.Queryable<Student>().Select(it => it.Id).ToArray();
            var list74 = db.Queryable<Student>().Where(it => ids3.Contains(it.Id)).ToList();
            if (list74.Count != ids2.Count) Cases.ThrowUnitError();

            var list75 = db.Queryable<Student>().Where(it => SqlFunc.Between(it.SchoolId, 1, 2)).ToList();
            if (list75.Count != 1 || list75.First().SchoolId != 2) Cases.ThrowUnitError();

            var list76 = db.Queryable<Student>().Select(it => SqlFunc.UNIX_TIMESTAMP(it.CreateDateTime)).ToList();

            var list77 = db.Queryable<Student>()
                .Where(it=>"ac"== it.Name.Substring(1, it.SchoolId))
                .Select(it => new { it.SchoolId,  name =it.Name.Substring(1, it.SchoolId) }).ToList();
        }

        private static void ValidateStudentData(SqlSugar.SqlSugarClient db)
        {
            var list = db.Queryable<Student>().ToList();
            if (list.First() is { } first && (first.BoolNull != null || first.SchoolIdNull != null)) Cases.ThrowUnitError();
            if (list.Last() is { } last && (last.BoolNull != true || last.SchoolIdNull != 4)) Cases.ThrowUnitError();

            var list3 = db.Queryable<Student>().Where(it=>it.SchoolId>2).ToList();
            if(list3.Any(s=>s.SchoolId<=2)) Cases.ThrowUnitError(); 
        }

        private static void InitializeStudentData(SqlSugar.SqlSugarClient db)
        {
            db.CodeFirst.InitTables<Student>();
            db.DbMaintenance.TruncateTable<Student>();
            var dt = DateTime.Now.AddDays(-10);
            db.Insertable(new Student() { Name = "jack",CreateDateTime= dt, Bool = true, SchoolId = 2 }).ExecuteCommand();
            db.Insertable(new Student() { Name = "tom_null", CreateDateTime = DateTime.Now.AddDays(-110), Bool = false, BoolNull = true, SchoolId = 3, SchoolIdNull = 4 }).ExecuteCommand();
            var x1=db.Queryable<Student>().ToList();
            var x2 = db.Queryable<Student>().Where(it=>it.Bool==true).ToList();
            var x3=db.Queryable<Student>().Where(it => it.Bool).ToList();
            if (x3.Count != x2.Count) Cases.ThrowUnitError();
            if (x3.First().Id != x2.First().Id) Cases.ThrowUnitError();
            if (x3.First().Bool!=true) Cases.ThrowUnitError();
            var x4 = db.Queryable<Student>().Where(it => it.Bool&&it.Bool).ToList();
            if (x4.First().Id!=x3.First().Id||x4.Count!=x2.Count) Cases.ThrowUnitError();
            var x5 = db.Queryable<Student>().Where(it =>  it.SchoolIdNull==null && it.Bool).ToList();
            if (x5.First().SchoolIdNull!=null||x5.First().Bool!=true) Cases.ThrowUnitError();
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
