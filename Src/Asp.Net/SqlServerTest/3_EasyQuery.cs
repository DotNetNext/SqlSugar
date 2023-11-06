using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace OrmTest 
{
    internal class _3_EasyQuery
    {
      

        public static void Init() 
        {

            CreateTable();
            GetAllStudents();
            GetStudentCount();
            GetStudentsByCondition();
            GetStudentsByName("jack");
            GetStudentById(1);
            GetMaxStudentId();
            GetStudentsOrderedByIdDesc();
            GetStudentNames();
        }

        private static void CreateTable()
        {
            SqlSugarClient db = DbHelper.GetNewDb();
            db.CodeFirst.InitTables<Student03>();
            db.Insertable(new Student03() { Name = "name" + SnowFlakeSingle.Instance.NextId() })
                .ExecuteCommand();
        }

        // 查询所有学生信息
        // Query all student records
        public  static List<Student03> GetAllStudents()
        {
            SqlSugarClient db = DbHelper.GetNewDb();
            return db.Queryable<Student03>().ToList();
        }

        // 查询学生总数
        // Get the total count of students
        public static int GetStudentCount()
        {
            SqlSugarClient db = DbHelper.GetNewDb();
            return db.Queryable<Student03>().Count();
        }

        // 按条件查询学生信息
        // Query student records based on conditions
        public static List<Student03> GetStudentsByCondition()
        {
            SqlSugarClient db = DbHelper.GetNewDb();

            // 查询Id为1的学生
            // Query students with Id equal to 1
            var studentsWithId1 = db.Queryable<Student03>().Where(it => it.Id == 1).ToList();

            // 查询name字段不为null的学生
            // Query students where the 'name' field is not null
            var studentsWithNameNotNull = db.Queryable<Student03>().Where(it => it.Name != null).ToList();

            // 查询name字段为null的学生
            // Query students where the 'name' field is null
            var studentsWithNameNull = db.Queryable<Student03>().Where(it => it.Name == null).ToList();

            // 查询name字段不为空的学生
            // Query students where the 'name' field is not empty
            var studentsWithNameNotEmpty = db.Queryable<Student03>().Where(it => it.Name != "").ToList();

            // 多条件查询
            // Query students with multiple conditions
            var studentsWithMultipleConditions = db.Queryable<Student03>().Where(it => it.Id > 10 && it.Name == "a").ToList();

            // 动态OR查询
            // Dynamic OR query
            var exp = Expressionable.Create<Student03>();
            exp.OrIF(true, it => it.Id == 1);
            exp.Or(it => it.Name.Contains("jack"));
            var studentsWithDynamicOr = db.Queryable<Student03>().Where(exp.ToExpression()).ToList();

            return studentsWithDynamicOr;
        }

        // 模糊查询
        // Fuzzy search
        public static List<Student03> GetStudentsByName(string keyword)
        {
            SqlSugarClient db = DbHelper.GetNewDb();
            return db.Queryable<Student03>().Where(it => it.Name.Contains(keyword)).ToList();
        }

        // 根据主键查询单个学生
        // Query a single student by primary key
        public static Student03 GetStudentById(int id)
        {
            SqlSugarClient db = DbHelper.GetNewDb();
            return db.Queryable<Student03>().Single(it => it.Id == id);
        }

        // 获取订单表中的最大Id
        // Get the maximum Id from the Student03 table
        public static int GetMaxStudentId()
        {
            SqlSugarClient db = DbHelper.GetNewDb();
            return db.Queryable<Student03>().Max(it => it.Id);
        }

        // 简单排序
        // Simple sorting
        public static List<Student03> GetStudentsOrderedByIdDesc()
        {
            SqlSugarClient db = DbHelper.GetNewDb();
            return db.Queryable<Student03>().OrderBy(sc => sc.Id, OrderByType.Desc).ToList();
        }

        // 查询一列
        // Query a single column
        public static List<string> GetStudentNames()
        {
            SqlSugarClient db = DbHelper.GetNewDb();
            return db.Queryable<Student03>().Select(it => it.Name).ToList();
        }



        public class Student03
        {
            [SugarColumn(IsPrimaryKey =true,IsIdentity =true)]
            public int Id { get; set; }
            public string Name { get; set; }
        }
         
    }
}
