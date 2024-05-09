using SqlSugar; 
using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Text; 
using System.Threading.Tasks; 

namespace OrmTest
{
    internal class _a5_GridSave
    {
        public static void Init()
        {
            // Get a new database connection
            // 获取一个新的数据库连接
            SqlSugarClient db = DbHelper.GetNewDb();

            // Initialize tables using CodeFirst
            // 使用 CodeFirst 初始化表
            db.CodeFirst.InitTables<Student>();

            // Clear table data
            // 清空表数据
            db.DbMaintenance.TruncateTable<Student>();

            // Insert two student records
            // 插入两条学生记录
            db.Insertable(new List<Student>() {
               new Student() {Name= "jack",CreateTime=DateTime.Now},
               new Student() {Name= "tom",CreateTime=DateTime.Now}
            }).ExecuteReturnIdentity();

            // Query all student records
            // 查询所有学生记录
            List<Student> getAll = db.Queryable<Student>().ToList();


             
            // Enable entity tracking for the list 'getAll'
            // 启用对列表 'getAll' 的实体跟踪
            db.Tracking(getAll); 




            // Remove the first record
            // 移除第一条记录
            getAll.RemoveAt(0);

            // Modify the name of the last record
            // 修改最后一条记录的姓名
            getAll[getAll.Count - 1].Name += "_Update";

            // Add a new record
            // 添加新记录
            getAll.Add(new Student { Name = "NewRecord" });





            // Execute GridSave operation
            // 执行 GridSave 操作
            db.GridSave(getAll).ExecuteCommand();

            // Query all students again
            // 再次查询所有学生
            var list = db.Queryable<Student>().ToList();
        }

        // Define the entity class 定义实体类
        [SugarTable("SaveTable_a5")]
        public class Student
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }
            public string Name { get; set; }
            public DateTime CreateTime { get; set; }
        }
    }
}