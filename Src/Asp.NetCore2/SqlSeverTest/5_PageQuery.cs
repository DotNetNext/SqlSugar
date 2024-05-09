using OrmTest;
using SqlSugar;
using System;
using System.Linq;
using System.Threading.Tasks;

public class _5_PageQuery
{
    public static void Init()
    {
    
        int pagenumber = 1;
        int pageSize = 20;
        int totalCount = 0;

        SqlSugarClient db = DbHelper.GetNewDb();

        //建表
        //Create table
        AddTestData(db);

        // 同步分页方法
        // Synchronous pagination method
        SyncPagination(db, pagenumber, pageSize, ref totalCount);

        // 异步分页方法
        // Asynchronous pagination method
        AsyncPagination(db, pagenumber, pageSize) .GetAwaiter() .GetResult();
    }

    public static void AddTestData(SqlSugarClient db)
    {
    
        //建表
        //Create table
        db.CodeFirst.InitTables<School, Student>();

        // 添加学校数据
        // Add school data
        var school1 = new School { Name = "School A" };
        var school2 = new School { Name = "School B" };
        db.Insertable(school1).ExecuteCommand();
        db.Insertable(school2).ExecuteCommand();

        // 添加学生数据
        // Add student data
        var student1 = new Student { SchoolId = school1.Id, Name = "John", CreateTime = DateTime.Now };
        var student2 = new Student { SchoolId = school1.Id, Name = "Alice", CreateTime = DateTime.Now }; 

        db.Insertable(student1).ExecuteCommand();
        db.Insertable(student2).ExecuteCommand();

        Console.WriteLine("Test data added successfully.");
    }

    /// <summary>
    /// 同步分页示例
    /// Synchronous pagination example
    /// </summary>
    /// <param name="db">数据库连接对象 Database connection object</param>
    /// <param name="pagenumber">页码 Page number</param>
    /// <param name="pageSize">每页大小 Page size</param>
    /// <param name="totalCount">总记录数 Total record count</param>
    public static void SyncPagination(SqlSugarClient db, int pagenumber, int pageSize, ref int totalCount)
    {
        // 同步单表分页
        // Synchronous pagination for a single table
        var page = db.Queryable<Student>().ToPageList(pagenumber, pageSize, ref totalCount);

        // 同步多表分页
        // Synchronous pagination for multiple tables
        var list = db.Queryable<Student>().LeftJoin<School>((st, sc) => st.SchoolId == sc.Id)
                   .Select((st, sc) => new { Id = st.Id, Name = st.Name, SchoolName = sc.Name })
                   .ToPageList(pagenumber, pageSize, ref totalCount);

        // offset分页
        // offset pagination
        var sqlServerPage = db.Queryable<Student>().ToOffsetPage(pagenumber, pageSize);
          
    }

    /// <summary>
    /// 异步分页示例
    /// Asynchronous pagination example
    /// </summary>
    /// <param name="db">数据库连接对象 Database connection object</param>
    /// <param name="pagenumber">页码 Page number</param>
    /// <param name="pageSize">每页大小 Page size</param>
    public static async Task AsyncPagination(SqlSugarClient db, int pagenumber, int pageSize)
    {
        RefAsync<int> total = 0;
        // 异步分页
        // Asynchronous pagination
        var orders = await db.Queryable<Student>().ToPageListAsync(pagenumber, pageSize, total);
    }
    [SugarTable("Student05")]
    public class Student
    {
        [SugarColumn(IsIdentity =true,IsPrimaryKey =true)]
        public int Id { get; set; }
        public int SchoolId { get; set; }
        public string Name { get; set; }
        public DateTime CreateTime { get; set; }
    }
    [SugarTable("School05")]
    public class School
    {
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int Id { get; set; }
        public string Name { get; set; }
    } 
}