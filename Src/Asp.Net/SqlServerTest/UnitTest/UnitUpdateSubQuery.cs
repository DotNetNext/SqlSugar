using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace OrmTest
{
    public class UnitUpdateSubQuery
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;

            //建表 
            if (!db.DbMaintenance.IsAnyTable("ClassEntity", false))
            {
                db.CodeFirst.InitTables<ClassEntity>();
            }
            if (!db.DbMaintenance.IsAnyTable("ClassStudentEntity", false))
            {
                db.CodeFirst.InitTables<ClassStudentEntity>();
            }
            if (!db.DbMaintenance.IsAnyTable("StudentEntity", false))
            {
                db.CodeFirst.InitTables<StudentEntity>();
            }
            db.DbMaintenance.TruncateTable("ClassEntity");
            db.DbMaintenance.TruncateTable("ClassStudentEntity");
            db.DbMaintenance.TruncateTable("StudentEntity");

            //插入数据
            var @class = new ClassEntity
            {
                StudentQty = 0
            };
            db.Insertable(@class).ExecuteCommand();
            var classStudentList = new List<ClassStudentEntity>
            {
                new ClassStudentEntity{ ClassID = 1, StudentID = 1}
            };
            db.Insertable(classStudentList).ExecuteCommand();
            var studentList = new List<StudentEntity>
            {
                new StudentEntity{ Name = "张三" }
            };
            db.Insertable(studentList).ExecuteCommand();
            var result = new SimpleClient<ClassEntity>(db).AsQueryable().Select(t => new ClassEntity
            {
                StudentQty = SqlFunc.Subqueryable<ClassStudentEntity>()
            .InnerJoin<StudentEntity>((tt, tt1) => tt.StudentID == tt1.StudentID)
            .Where(tt => tt.ClassID == t.ClassID)
            .Count()
            }).ToList();
            //用例代码
            var result2 = new SimpleClient<ClassEntity>(db).Update(t => new ClassEntity
            {
                StudentQty = SqlFunc.Subqueryable<ClassStudentEntity>()
                    .InnerJoin<StudentEntity>((tt, tt1) => tt.StudentID == tt1.StudentID)
                    .Where(tt => tt.ClassID == t.ClassID)
                    .Count()
            }, tt => tt.ClassID == 1);
            db.DbMaintenance.DropTable("ClassEntity");
            db.DbMaintenance.DropTable("ClassStudentEntity");
            db.DbMaintenance.DropTable("StudentEntity");
            Console.WriteLine(result);
            Console.WriteLine("用例跑完");
       
        }

        internal class ClassEntity
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int ClassID { get; set; }

            public int StudentQty { get; set; }
        }
        internal class ClassStudentEntity
        {
            [SugarColumn(IsPrimaryKey = true)]
            public int ClassID { get; set; }

            [SugarColumn(IsPrimaryKey = true)]
            public int StudentID { get; set; }
        }
        internal class StudentEntity
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int StudentID { get; set; }

            public string Name { get; set; }
        }
    }
}
