using MongoDB.Bson;
using MongoDB.Bson.IO;
using SqlSugar.MongoDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbTest 
{
    public class QueryLeftJoin2
    {
        public static void Init()
        {
            var db = DbHelper.GetNewDb();
            db.DbMaintenance.TruncateTable<City,School,Student>();

            // 添加 City 测试数据
            var city1 = new City { Name = "北京" };
            var city2 = new City { Name = "上海" };
            db.Insertable(city1).ExecuteCommandIdentityIntoEntity();
            db.Insertable(city2).ExecuteCommandIdentityIntoEntity();

            // 添加 School 测试数据
            var school1 = new School { Name = "清华大学", CityId = city1.Id };
            var school2 = new School { Name = "复旦大学", CityId = city2.Id };
            db.Insertable(school1).ExecuteCommandIdentityIntoEntity();
            db.Insertable(school2).ExecuteCommandIdentityIntoEntity();

            // 添加 Student 测试数据
            var student1 = new Student { Name = "张三", SchoolId = school1.Id };
            var student2 = new Student { Name = "李四", SchoolId = school2.Id };
            db.Insertable(student1).ExecuteCommandIdentityIntoEntity();
            db.Insertable(student2).ExecuteCommandIdentityIntoEntity();

            var students = db.Queryable<Student>().ToDataTable();
            var schools = db.Queryable<School>().ToDataTable();
            var cities = db.Queryable<City>().ToDataTable();

            var dt=db.Queryable<Student>()
                .LeftJoin<School>((s, sc) => s.SchoolId == sc.Id)
                .LeftJoin<City>((s, sc, city) => sc.CityId == city.Id)
                .Select((s, sc, city) => new
                {
                    studentName=s.Name,
                    schoolName=sc.Name,
                    cityName=city.Name
                }).ToList();
            if (dt.First().schoolName != "清华大学" || dt.First().studentName != "张三" || dt.First().cityName != "北京") Cases.ThrowUnitError();

           var dt2 = db.Queryable<Student>()
            .LeftJoin<School>((s, sc) => s.SchoolId == sc.Id)
            .Select((s, sc) => sc).ToList();
            if (dt2.First().Name != "清华大学") Cases.ThrowUnitError();

            var dt3 = db.Queryable<Student>()
            .LeftJoin<School>((s, sc) => s.SchoolId == sc.Id)
            .Select((s, sc) => s).ToList();
            if (dt3.First().Name != "张三") Cases.ThrowUnitError();

            var dt5 = db.Queryable<Student>()
        .LeftJoin<School>((s, sc) => s.SchoolId == sc.Id )
        .Select((s, sc) => new {
            studentName = s.Name,
            schoolName = sc.Name
        }).ToList();
            //var dt4 = db.Queryable<Student>()
            //.LeftJoin<School>((s, sc) => s.SchoolId.ToLower() == sc.Id)
            //.Select((s,sc)=>new { 
            //    studentName=s.Name,
            //    schoolName=sc.Name
            //}).ToList();
            //if (dt3.First().Name != "张三") Cases.ThrowUnitError();
        }
        [SqlSugar.SugarTable("UnitStudentdu2s31")]
        public class Student : MongoDbBase
        {
            public string Name { get; set; }
            [SqlSugar.SugarColumn(ColumnDataType = nameof(ObjectId))]
            public string SchoolId { get; set; }
        }
        [SqlSugar.SugarTable("UnitSchool1lki131")]
        public class School : MongoDbBase
        {
            public string Name { get; set; }
            [SqlSugar.SugarColumn(ColumnDataType = nameof(ObjectId))]
            public string CityId { get; set; }
        }
        [SqlSugar.SugarTable("Cityd1ki131")]
        public class City : MongoDbBase
        {
            public string Name { get; set; }
        }
    }


}
