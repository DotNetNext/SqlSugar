using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    internal class _6_NavQuery
    {
        /// <summary>
        /// Initialize navigation query examples.
        /// 初始化导航查询示例。
        /// </summary>
        public static void Init()
        {
            var db = DbHelper.GetNewDb();

            // Initialize database table structures.
            // 初始化数据库表结构。
            InitializeDatabase(db);

            // One-to-One navigation query test.
            // 一对一导航查询测试。
            OneToOneTest(db);

            // One-to-Many navigation query test.
            // 一对多导航查询测试。
            OneToManyTest(db);

            // Many-to-Many navigation query test.
            // 多对多导航查询测试。
            ManyToManyTest(db);
        }

        /// <summary>
        /// Test many-to-many navigation queries.
        /// 测试多对多导航查询。
        /// </summary>
        private static void ManyToManyTest(SqlSugarClient db)
        {
            // Many-to-many navigation query, query table A and include its BList.
            // 多对多导航查询，查询A表，并包含其BList。
            var list4 = db.Queryable<A>().Includes(it => it.BList).ToList();

            // Many-to-many navigation query with filtered BList while preserving the original A records, regardless of the filter on BList.
            // 使用过滤条件的多对多导航查询，在过滤BList的同时保持A表的原始记录，不受BList过滤条件的影响。
            var list5 = db.Queryable<A>().Includes(it => it.BList.Where(s => s.BId == 1).ToList()).ToList();

            // Many-to-many navigation query with filtered A records while preserving the original BList, regardless of the filter on A records.
            // 使用过滤条件的多对多导航查询，在过滤A表的同时保持BList的原始记录，不受A表过滤条件的影响。
            var list6 = db.Queryable<A>().Includes(it => it.BList)
                          .Where(it =>it.BList.Any(s => s.BId == 1)).ToList();

            // Many-to-many navigation query with filtered BList and filtered A records, but not preserving the original A and B records.
            // 使用过滤条件的多对多导航查询，在A表中过滤BList并过滤A记录，但不保持A表和B表的原始记录。
            var list7 = db.Queryable<A>()
                        .Includes(it => it.BList.Where(s => s.BId == 1).ToList())
                        .Where(it => it.BList.Any(s => s.BId == 1)).ToList();
        }

        /// <summary>
        /// Test one-to-many navigation queries.
        /// 测试一对多导航查询。
        /// </summary>
        private static void OneToManyTest(SqlSugarClient db)
        {
            // One-to-many navigation query, query table Student and include its Books.
            // 一对多导航查询，查询Student表，并包含其Books。
            var list4 = db.Queryable<Student>().Includes(it => it.Books).ToList();

            // One-to-many navigation query with filtered Books while preserving the original Student records, regardless of the filter on Books.
            // 使用过滤条件的一对多导航查询，在过滤Books的同时保持Student表的原始记录，不受Books过滤条件的影响。
            var list5 = db.Queryable<Student>().Includes(it => it.Books.Where(s => s.BookId == 1).ToList()).ToList();

            // One-to-many navigation query with filtered Student records while preserving the original Books, regardless of the filter on Student records.
            // 使用过滤条件的一对多导航查询，在过滤Student表的同时保持Books的原始记录，不受Student表过滤条件的影响。
            var list6 = db.Queryable<Student>().Includes(it => it.Books)
                            .Where(it => it.Books.Any(s => s.BookId == 1)).ToList();

            // One-to-many navigation query with filtered Books and filtered Student records, but not preserving the original Student and Books records.
            // 使用过滤条件的一对多导航查询，在Student表中过滤Books并过滤Student记录，但不保持Student表和Books表的原始记录。
            var list7 = db.Queryable<Student>()
                          .Includes(it => it.Books.Where(s => s.BookId == 1).ToList())
                          .Where(it =>  it.Books.Any(s => s.BookId == 1)).ToList();
        }

        /// <summary>
        /// Test one-to-one navigation queries.
        /// 测试一对一导航查询。
        /// </summary>
        private static void OneToOneTest(SqlSugarClient db)
        {
            // One-to-one navigation query with condition, query table Student and include its associated School with specific SchoolId.
            // 带条件的一对一导航查询，查询Student表，并包含其关联的School表，条件为特定的SchoolId。
            var list = db.Queryable<Student>()
                .Where(it => it.School.SchoolId == 1)
                .ToList();

            // Inner join navigation query, query table Student and include its associated School.
            // 内连接导航查询，查询Student表，并包含其关联的School表。
            var list2 = db.Queryable<Student>().IncludeInnerJoin(it => it.School)
                .ToList();

            // Includes navigation query, query table Student and include its associated School.
            // 包含导航查询，查询Student表，并包含其关联的School表。
            var list3 = db.Queryable<Student>().Includes(it => it.School).ToList();
        }

        /// <summary>
        /// Initialize database tables and insert sample data for navigation query examples.
        /// 初始化导航查询示例的数据库表并插入示例数据。
        /// </summary>
        private static void InitializeDatabase(SqlSugarClient db)
        {
            // Initialize and truncate tables for Student, School, and Book entities.
            // 初始化并清空Student、School和Book表。
            db.CodeFirst.InitTables<Student, School, Book>();
            db.DbMaintenance.TruncateTable<Student, School, Book>();

            // Sample data for Student, School, and Book entities.
            // Student、School和Book表的示例数据。
            var students = new List<Student>
            {
                new Student
                {
                    Name = "Student 1",
                    SexCode = "M",
                    School = new School { SchoolName = "School 1" },
                    Books = new List<Book>
                    {
                        new Book { Name = "Book 1" },
                        new Book { Name = "Book 2" }
                    }
                },
                new Student
                {
                    Name = "Student 2",
                    SexCode = "F",
                    School = new School { SchoolName = "School 2" },
                    Books = new List<Book>
                    {
                        new Book { Name = "Book 3" }
                    }
                }
            };

            // Insert sample data for Student, School, and Book entities using navigation properties.
            // 使用导航属性插入Student、School和Book表的示例数据。
            db.InsertNav(students)
                .Include(it => it.School)
                .Include(it => it.Books).ExecuteCommand();

            // Initialize and truncate tables for A, B, and ABMapping entities.
            // 初始化并清空A、B和ABMapping表。
            db.CodeFirst.InitTables<A, B, ABMapping>();
            db.DbMaintenance.TruncateTable<A, B, ABMapping>();

            // Sample data for A, B, and ABMapping entities.
            // A、B和ABMapping表的示例数据。
            List<A> a1 = new List<A> { new A() { Name = "A1" }, new A() { Name = "A2" } };
            B b1 = new B { Name = "B1" };
            B b2 = new B { Name = "B2" };
            a1[0].BList = new List<B> { b1, b2 };

            // Insert sample data for A, B, and ABMapping entities using navigation properties.
            // 使用导航属性插入A、B和ABMapping表的示例数据。
            db.InsertNav(a1).Include(x => x.BList).ExecuteCommand();
        }

        /// <summary>
        /// Student entity representing the Student table in the database.
        /// 表示数据库中Student表的Student实体类。
        /// </summary>
        [SugarTable("Student06")]
        public class Student
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int StudentId { get; set; }
            public string Name { get; set; }
            public string SexCode { get; set; }
            public int SchoolId { get; set; }

            // One-to-One navigation property to School entity.
            // 与School实体的一对一导航属性。
            [Navigate(NavigateType.OneToOne, nameof(SchoolId))]
            public School School { get; set; }

            // One-to-Many navigation property to Book entities.
            // 与Book实体的一对多导航属性。
            [Navigate(NavigateType.OneToMany, nameof(Book.StudentId))]
            public List<Book> Books { get; set; }
        }

        /// <summary>
        /// School entity representing the School table in the database.
        /// 表示数据库中School表的School实体类。
        /// </summary>
        [SugarTable("School06")]
        public class School
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int SchoolId { get; set; }
            public string SchoolName { get; set; }
        }

        /// <summary>
        /// Book entity representing the Book table in the database.
        /// 表示数据库中Book表的Book实体类。
        /// </summary>
        [SugarTable("Book06")]
        public class Book
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int BookId { get; set; }
            public string Name { get; set; }
            public int StudentId { get; set; }
        }

        /// <summary>
        /// A entity representing the A table in the database for many-to-many relationship.
        /// 表示多对多关系中数据库中A表的A实体类。
        /// </summary>
        [SugarTable("A06")]
        public class A
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int AId { get; set; }
            public string Name { get; set; }

            // Many-to-Many navigation property to B entities using ABMapping table.
            // 与B实体的多对多导航属性，使用ABMapping表。
            [Navigate(typeof(ABMapping), nameof(ABMapping.AId), nameof(ABMapping.BId))]
            public List<B> BList { get; set; }
        }

        /// <summary>
        /// B entity representing the B table in the database for many-to-many relationship.
        /// 表示多对多关系中数据库中B表的B实体类。
        /// </summary>
        [SugarTable("B06")]
        public class B
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int BId { get; set; }
            public string Name { get; set; }

            // Many-to-Many navigation property to A entities using ABMapping table.
            // 与A实体的多对多导航属性，使用ABMapping表。
            [Navigate(typeof(ABMapping), nameof(ABMapping.BId), nameof(ABMapping.AId))]
            public List<A> AList { get; set; }
        }

        /// <summary>
        /// ABMapping entity representing the intermediate table for many-to-many relationship between A and B entities.
        /// 表示A和B实体之间多对多关系的中间表的ABMapping实体类。
        /// </summary>
        [SugarTable("ABMapping06")]
        public class ABMapping
        {
            [SugarColumn(IsPrimaryKey = true)]
            public int AId { get; set; }
            [SugarColumn(IsPrimaryKey = true)]
            public int BId { get; set; }
        }
    }
}