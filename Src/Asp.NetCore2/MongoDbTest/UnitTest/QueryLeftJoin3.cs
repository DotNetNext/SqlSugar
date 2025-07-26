using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SqlSugar.MongoDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbTest 
{
    internal class QueryLeftJoin3
    {
        internal static void Init()
        {
            var db = DbHelper.GetNewDb();
            db.DbMaintenance.TruncateTable<Student, School>(); 
            // 添加学校数据
            var school = new School { Name = "TestSchool" };
            var ids=db.Insertable(school).ExecuteReturnPkList<string>(); 
            // 添加学生数据，SchoolId 关联学校
            var student = new Student { Name = "TestStudent",Json=new Json() {  SchoolId=ids.Last() } }; 
            db.Insertable(student).ExecuteCommand();
 
 
            var list = db.Queryable<Student>()
                .LeftJoin<School>((x, y) => x.Json.SchoolId == y.Id)
                .Select((x, y) => new
                {
                    StudentName = x.Name,
                    SchoolName = y.Name
                }).ToList();

            if (list.First().SchoolName != "TestSchool") Cases.ThrowUnitError();
             
        }
        [SqlSugar.SugarTable("UnitStudentsfyyd1")]
        public class Student : MongoDbBase
        {
            public string Name { get; set; }

            [SqlSugar.SugarColumn(IsJson =true)]
            public Json Json { get; set; }

		}
        public class Json 
        {
			[BsonRepresentation(BsonType.ObjectId)]
			[SqlSugar.SugarColumn(ColumnDataType = nameof(ObjectId))]
			public string SchoolId { get; set; }
		}
        [SqlSugar.SugarTable("UnitSchool123131")]
        public class School : MongoDbBase
        {
            public string Name { get; set; }
        }
    }
}
