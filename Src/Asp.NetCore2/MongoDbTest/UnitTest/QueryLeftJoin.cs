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

            //var list = db.Queryable<Student>()
            //    .LeftJoin<School>((x, y) => x.SchoolId == y.Id) 
            //    .Select((x,y) => new
            //    {
            //        StudentName= x.Name,
            //        SchoolName= y.Name
            //    }).ToList();
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
