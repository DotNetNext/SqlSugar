using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Net.Mime;
    using System.Threading.Tasks;
    using SqlSugar;


    public static class Unitafdssfasydsfsf
    {
        public static void Init()
        {
            Async().GetAwaiter().GetResult();
        }
        public static async Task Async()
        {
            await Task.CompletedTask;
            var db = NewUnitTest.Db;

            //建表 
            db.CodeFirst.InitTables<Students>();
            db.CodeFirst.InitTables<StudentsSub>();
            db.CodeFirst.InitTables<StudentsSubInfo>();
            db.DbMaintenance.TruncateTable<Students, StudentsSub, StudentsSubInfo>();

            var data = new List<Students>()
        {
            new Students() { Id = 1, Name = "张三" },
            new Students() { Id = 2, Name = "张三2", ParentId = 1 },
            new Students() { Id = 3, Name = "张三3", ParentId = 1 },
            new Students() { Id = 4, Name = "张三4", ParentId = 1 },
        };

            await db.Storageable(data).ExecuteCommandAsync();

            var data2 = new List<StudentsSub>()
        {
            new StudentsSub() { Id = 1, StudentId = 1 },
            new StudentsSub() { Id = 2, StudentId = 2 },
            new StudentsSub() { Id = 3, StudentId = 3 },
            new StudentsSub() { Id = 4, StudentId = 4 },
        };

            await db.Storageable(data2).ExecuteCommandAsync();
            var data3 = new List<StudentsSubInfo>()
        {
            new StudentsSubInfo() { Id = 1, StudentSubId = 1 },
            new StudentsSubInfo() { Id = 2, StudentSubId = 2 },
            new StudentsSubInfo() { Id = 3, StudentSubId = 3 },
            new StudentsSubInfo() { Id = 4, StudentSubId = 4 },
        };

            await db.Storageable(data3).ExecuteCommandAsync();

            // var flag = true;
            //
            // // var query = await db.Queryable<Students>()
            // //    .IncludesIf(flag, d => d.Child)
            // //    .ToListAsync();

            var list=await db.Queryable<Students>() 
               .IncludesAllSecondLayer(s => s.Subs)
               .ToListAsync();
            var list2 = await db.Queryable<Students>()
               .Includes(it=>it.Subs,it=>it.Infos) 
              .ToListAsync();

            if (db.Utilities.SerializeObject(list) != db.Utilities.SerializeObject(list2))
                throw new Exception("unit error");
        }

        private static ISugarQueryable<T> IncludesIf<T, TReturn>(this ISugarQueryable<T> queryable, bool condition,
            Expression<Func<T, TReturn>> expression) where T : class
        {
            return condition ? queryable.Includes(expression) : queryable;
        }
        [SugarTable("UnitStudentssdfs477")]
        public class Students
        {
            [SugarColumn(IsPrimaryKey = true)]
            public int Id { get; set; }

            public string Name { get; set; }

            public long ParentId { get; set; }

            [Navigate(NavigateType.OneToMany, nameof(StudentsSub.StudentId))]
            public List<StudentsSub> Subs { get; set; }
        }
        [SugarTable("StudentsSubssdfs477")]
        public class StudentsSub
        {
            [SugarColumn(IsPrimaryKey = true)]
            public int Id { get; set; }

            public long StudentId { get; set; }

            [SugarColumn(IsNullable = true)]
            public string SubName { get; set; }

            [Navigate(NavigateType.OneToMany, nameof(StudentsSubInfo.StudentSubId))]
            public List<StudentsSubInfo> Infos { get; set; }
        }
        [SugarTable("StudentsSubInfossdfs477")]
        public class StudentsSubInfo
        {
            [SugarColumn(IsPrimaryKey = true)]
            public int Id { get; set; }

            public int StudentSubId { get; set; }

            [SugarColumn(IsNullable = true)]
            public string Address { get; set; }
        }
    }
}
