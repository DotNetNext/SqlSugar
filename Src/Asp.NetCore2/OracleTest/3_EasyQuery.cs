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
        /// <summary>
        /// 初始化方法，包含各种查询操作的演示
        /// Initialization method containing demonstrations of various query operations
        /// </summary>
        public static void Init()
        {
            // 创建表并插入一条记录
            // Create table and insert a record
            CreateTable();

            // 查询所有学生信息
            // Query all student records
            GetAllStudents();

            // 查询学生总数
            // Get the total count of students
            GetStudentCount();

            // 按条件查询学生信息
            // Query student records based on conditions
            GetStudentsByCondition();

            // 模糊查询学生信息（名字包含"jack"的学生）
            // Fuzzy search for student records (students with names containing "jack")
            GetStudentsByName("jack");

            // 根据学生ID查询单个学生
            // Query a single student by student ID
            GetStudentById(1);

            // 获取Student03表中的最大Id
            // Get the maximum ID from the Student03 table
            GetMaxStudentId();

            // 简单排序（按照Id降序排序）
            // Simple sorting (sorting by Id in descending order)
            GetStudentsOrderedByIdDesc();

            // 查询学生姓名列表
            // Query the list of student names
            GetStudentNames();
        }

        /// <summary>
        /// 创建表并插入一条记录
        /// Create table and insert a record
        /// </summary>
        private static void CreateTable()
        {
            SqlSugarClient db = DbHelper.GetNewDb();
            db.CodeFirst.InitTables<Student03>();
            db.Insertable(new Student03() { Name = "name" + SnowFlakeSingle.Instance.NextId() })
                .ExecuteReturnSnowflakeId();
        }

        /// <summary>
        /// 查询所有学生信息
        /// Query all student records
        /// </summary>
        private static void GetAllStudents()
        {
            SqlSugarClient db = DbHelper.GetNewDb();
            var students = db.Queryable<Student03>().ToList();
            // 处理查询结果
            // Process the query results
        }

        /// <summary>
        /// 查询学生总数
        /// Get the total count of students
        /// </summary>
        private static void GetStudentCount()
        {
            SqlSugarClient db = DbHelper.GetNewDb();
            var count = db.Queryable<Student03>().Count();
            // 处理查询结果
            // Process the query results
        }

        /// <summary>
        /// 按条件查询学生信息
        /// Query student records based on conditions
        /// </summary>
        private static void GetStudentsByCondition()
        {
            SqlSugarClient db = DbHelper.GetNewDb();

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
             
        }

        /// <summary>
        /// 模糊查询学生信息
        /// Fuzzy search for student records
        /// </summary>
        private static void GetStudentsByName(string keyword)
        {
            SqlSugarClient db = DbHelper.GetNewDb();
            var students = db.Queryable<Student03>().Where(it => it.Name.Contains(keyword)).ToList();
            // 处理查询结果
            // Process the query results
        }

        /// <summary>
        /// 根据学生ID查询单个学生
        /// Query a single student by student ID
        /// </summary>
        private static void GetStudentById(int id)
        {
            SqlSugarClient db = DbHelper.GetNewDb();
            var student = db.Queryable<Student03>().Single(it => it.Id == id);
            // 处理查询结果
            // Process the query results
        }

        /// <summary>
        /// 获取Student03表中的最大Id
        /// Get the maximum ID from the Student03 table
        /// </summary>
        private static void GetMaxStudentId()
        {
            SqlSugarClient db = DbHelper.GetNewDb();
            var maxId = db.Queryable<Student03>().Max(it => it.Id);
            // 处理查询结果
            // Process the query results
        }

        /// <summary>
        /// 简单排序（按照Id降序排序）
        /// Simple sorting (sorting by Id in descending order)
        /// </summary>
        private static void GetStudentsOrderedByIdDesc()
        {
            SqlSugarClient db = DbHelper.GetNewDb();
            var students = db.Queryable<Student03>().OrderBy(sc => sc.Id, OrderByType.Desc).ToList();
            // 处理查询结果
            // Process the query results
        }

        /// <summary>
        /// 查询学生姓名列表
        /// Query the list of student names
        /// </summary>
        private static void GetStudentNames()
        {
            SqlSugarClient db = DbHelper.GetNewDb();
            var studentNames = db.Queryable<Student03>().Select(it => it.Name).ToList();
            // 处理查询结果
            // Process the query results
        }

        public class Student03
        {
            [SugarColumn(IsPrimaryKey = true)]
            public long Id { get; set; }
            public string Name { get; set; }
        }
    }
}