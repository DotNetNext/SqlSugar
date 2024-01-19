using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
 
    public class Unitadfaasdfaaa
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;

            db.CodeFirst.InitTables<Teacher, ClassRoom, StarClass, TeacherClass, StudentClass>();
            db.DbMaintenance.TruncateTable<Teacher, ClassRoom, StarClass, TeacherClass, StudentClass>();
            db.Insertable(new ClassRoom()
            {
                Name = "a"
            }).ExecuteCommand();
            db.Insertable(new Teacher()
            {
                Name = "b"
            }).ExecuteCommand();
            db.Queryable<Teacher>()
                .Select(s => new TeacherView
                {
                    Id = s.Id,
                    Name = s.Name,
                    ClassRoom = SqlFunc.Subqueryable<ClassRoom>()
                        .LeftJoin<StarClass>((c, sc) => c.Id == sc.ClassId)
                        .Where((c, sc) => SqlFunc.Subqueryable<TeacherClass>().Where(tc => tc.TeacherId == s.Id && tc.ClassRoomId == c.Id).Any())
                        .ToList((c, sc) => new ClassRoomView()
                        {
                            HasStudent = SqlFunc.Subqueryable<StudentClass>().Where(st => st.ClassId == c.Id).Any(),
                        }, true)
                })
                .ToList();
        }

        public class Teacher
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }

            public string Name { get; set; }
        }

        public class TeacherView
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public List<ClassRoomView> ClassRoom { get; set; }
        }

        public class ClassRoom
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }

            public string Name { get; set; }
        }

        public class ClassRoomView
        {
            public int Id { get; set; }

            public string Name { get; set; }
            public bool HasStudent { get; set; }
        }

        public class TeacherClass
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }

            public int TeacherId { get; set; }

            public int ClassRoomId { get; set; }
        }

        public class StarClass
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }

            public int ClassId { get; set; }
        }

        public class Student
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }

            public int ClassId { get; set; }
        }

        public class StudentClass
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }

            public int StudentId { get; set; }

            public int ClassId { get; set; }
        }
    }
 
}
