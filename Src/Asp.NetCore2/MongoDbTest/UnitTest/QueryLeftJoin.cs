using MongoDB.Bson;
using SqlSugar.MongoDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbTest 
{
    internal class QueryLeftJoin
    {
        internal static void Init()
        {
            var db = DbHelper.GetNewDb();
            db.DbMaintenance.TruncateTable<School, School>(); 
            // 添加学校数据
            var school = new School { Name = "TestSchool" };
            var ids=db.Insertable(school).ExecuteReturnPkList<string>(); 
            // 添加学生数据，SchoolId 关联学校
            var student = new Student { Name = "TestStudent", SchoolId = ids.Last() }; 
            db.Insertable(student).ExecuteCommand();
            // 添加学生数据，SchoolId 关联学校并且没有学校
            var student2 = new Student { Name = "TestStudentNoSchool", SchoolId =
               ObjectId.GenerateNewId().ToString()
            }; 
            db.Insertable(student2).ExecuteCommand();


            if (db.Queryable<Student>().First(it => it.SchoolId == ids.Last()).Name != "TestStudent") Cases.ThrowUnitError();
            db.Updateable(db.Queryable<Student>().First(it => it.SchoolId == ids.Last())).ExecuteCommand();
            if (db.Queryable<Student>().First(it => it.SchoolId == ids.Last()).Name != "TestStudent") Cases.ThrowUnitError();

           var adoTest= db.Ado.GetDataTable(@"aggregate UnitStudent123131 [
              {
                $lookup: {
                  from: ""UnitSchool123131"",
                  localField: ""SchoolId"",
                  foreignField: ""_id"",
                  as: ""y""
                }
              },
              {
                $unwind: {
                  path: ""$y"",
                  preserveNullAndEmptyArrays: true
                }
              },
              {
                $project: {
                  _id: 0,
                  StudentName: ""$Name"",
                      SchoolName: {
                  $ifNull: [""$y.Name"", null]  
                }
                }
              }
            ]");

            var list = db.Queryable<Student>()
                .LeftJoin<School>((x, y) => x.SchoolId == y.Id)
                .Select((x, y) => new
                {
                    StudentName = x.Name,
                    SchoolName = y.Name
                }).ToList();

            if (list.Count != 3)  Cases.ThrowUnitError();
            if (list.Any(s=>s.SchoolName== "TestSchool") ==false) Cases.ThrowUnitError();
            if (list.Any(s => s.StudentName == "jack") == false) Cases.ThrowUnitError();

            var list2 = db.Queryable<Student>()
               .InnerJoin<School>((x, y) => x.SchoolId == y.Id)
               .Select((x, y) => new
               {
                   StudentName = x.Name,
                   SchoolName = y.Name
               }).ToList(); 
            if (list2.Count != 1) Cases.ThrowUnitError();
            if (list2.Any(s => s.SchoolName == "TestSchool") == false) Cases.ThrowUnitError();

            var list3 = db.Queryable<Student>()
            .LeftJoin<School>((x, y) => x.SchoolId == y.Id)
            .Where((x,y)=>x.Name=="jack")
            .Select((x, y) => new
            {
                StudentName = x.Name,
                SchoolName = y.Name
            }).ToList();
            if (list3.Any(s => s.SchoolName ==null&&s.StudentName=="jack")==false) Cases.ThrowUnitError();

            var list4 = db.Queryable<Student>()
                 .LeftJoin<School>((x, y) => x.SchoolId == y.Id)
                 .Where((x, y) => x.Name == "jack"||y.Name== "TestSchool")
                 .Select((x, y) => new
                 {
                     StudentName = x.Name,
                     SchoolName = y.Name
                 }).ToList();
            if(list4.Count!=2) Cases.ThrowUnitError();
            if (list4.Any(s=>s.SchoolName== "TestSchool") ==false) Cases.ThrowUnitError();
            if (list4.Any(s => s.SchoolName == null||s.StudentName== "jack") == false) Cases.ThrowUnitError();

             
            db.Insertable(new Student { Name = "A", SchoolId = ids.Last() }).ExecuteCommand();
            var list5 = db.Queryable<Student>()
            .LeftJoin<School>((x, y) => x.SchoolId == y.Id) 
            .OrderByDescending((x,y)=>x.Name)
            .Select((x, y) => new
            {
                StudentName = x.Name,
                SchoolName = y.Name
            }).ToList();
            if (list5.Count != 4) Cases.ThrowUnitError();
            if (list5.Last().StudentName!= "A") Cases.ThrowUnitError();
            if (list5.First().StudentName != "jack") Cases.ThrowUnitError();


            var list6 = db.Queryable<Student>()
           .LeftJoin<School>((x, y) => x.SchoolId == y.Id)
           .OrderBy((x, y) => x.Name)
               .Select((x, y) => new
              {
                  StudentName = x.Name,
                  SchoolName = y.Name
              }).ToList();
            if (list6.Count != 4) Cases.ThrowUnitError();
            if (list6.Last().StudentName != "jack") Cases.ThrowUnitError();
            if (list6.First().StudentName != "A") Cases.ThrowUnitError();


            var list7 = db.Queryable<Student>()
            .LeftJoin<School>((x, y) => x.SchoolId == y.Id)
            .OrderBy((x, y) => x.Name)
            .Select((x, y) => new
            {
                StudentName = x.Name,
                SchoolName = y.Name
            }).ToPageList(1,2);
            if (list7.Count != 2) Cases.ThrowUnitError();
            if (list7.First().StudentName != "A") Cases.ThrowUnitError();

            var count = 0;
            var list8 = db.Queryable<Student>()
            .LeftJoin<School>((x, y) => x.SchoolId == y.Id)
            .OrderBy((x, y) => x.Name)
            .Select((x, y) => new
            {
                StudentName = x.Name,
                SchoolName = y.Name
            }).ToPageList(1, 2,ref count);
            if(count!=4||list8.Count!=2||list8.First().StudentName!="A") Cases.ThrowUnitError();

            var count2 = 0;
            var list9 = db.Queryable<Student>()
            .LeftJoin<School>((x, y) => x.SchoolId == y.Id)
            .Where(x=>x.Name!="jack")
            .OrderBy((x, y) => x.Name)
            .Select((x, y) => new
            {
                StudentName = x.Name,
                SchoolName = y.Name
            }).ToPageList(1, 2, ref count2);
            if (count2 != 3 || list9.Count != 2 || list9.First().StudentName != "A") Cases.ThrowUnitError();

            var list10 = db.Queryable<Student>()
             .LeftJoin<School>((x, y) => x.SchoolId == y.Id)
             .LeftJoin<School>((x, y, z) => x.SchoolId == z.Id)
             .Where((x, y) =>y.Name == "TestSchool")
             .Select((x, y,z) => new
             {
                 StudentName = x.Name,
                 SchoolName = y.Name,
                 SchoolName2=z.Name
             }).ToList();
            if(list10.First().SchoolName!=list10.First().SchoolName2) Cases.ThrowUnitError();

            var list11 = db.Queryable<Student>()
            .LeftJoin<School>((x, y) => y.Id== x.SchoolId )
            .LeftJoin<School>((x, y, z) => x.SchoolId == z.Id)
            .Where((x, y) => y.Name == "TestSchool")
            .Select((x, y, z) => new
            {
                StudentName = x.Name,
                SchoolName = y.Name,
                SchoolName2 = z.Name
            }).ToList();
            if (list11.First().SchoolName != list11.First().SchoolName2) Cases.ThrowUnitError();
        }
        [SqlSugar.SugarTable("UnitStudent123131")]
        public class Student : MongoDbBase
        {
            public string Name { get; set; }
            [SqlSugar.SugarColumn(ColumnDataType =nameof(ObjectId))]
            public string SchoolId { get; set; }
        }
        [SqlSugar.SugarTable("UnitSchool123131")]
        public class School : MongoDbBase
        {
            public string Name { get; set; }
        }
    }
}
