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
        public static void Init()
        {
            var db = NewUnitTest.Db;

            InitializeDatabase(db);

            OneToOneTest(db);

            OneToManyTest(db);

            ManyToManyTest(db);
        }
        private static void ManyToManyTest(SqlSugarClient db)
        {
            var list4 = db.Queryable<A>().Includes(it => it.BList).ToList();

            var list5 = db.Queryable<A>().Includes(it => it.BList.Where(s => s.BId == 1).ToList()).ToList();

            var list6 = db.Queryable<A>().Includes(it => it.BList)
                        .Where(it => it.BList.Any(s => s.BId == 1)).ToList();

            var list7 = db.Queryable<A>()
                      .Includes(it => it.BList.Where(s => s.BId == 1).ToList())
                      .Where(it => it.BList.Any(s => s.BId == 1)).ToList();
        }

        private static void OneToManyTest(SqlSugarClient db)
        {
            var list4 = db.Queryable<Student>().Includes(it => it.Books).ToList();

            var list5 = db.Queryable<Student>().Includes(it => it.Books.Where(s=>s.BookId==1).ToList()).ToList();

            var list6 = db.Queryable<Student>().Includes(it => it.Books)
                        .Where(it=>it.Books.Any(s=>s.BookId==1)).ToList();

            var list7 = db.Queryable<Student>()
                      .Includes(it => it.Books.Where(s => s.BookId == 1).ToList())
                      .Where(it => it.Books.Any(s => s.BookId == 1)).ToList();
        }

        private static void OneToOneTest(SqlSugarClient db)
        {
            var list = db.Queryable<Student>()
                .Where(it => it.School.SchoolId == 1)
                .ToList();

            var list2 = db.Queryable<Student>().IncludeInnerJoin(it => it.School)
                .ToList();

            var list3 = db.Queryable<Student>().Includes(it => it.School).ToList();
        }

        private static void InitializeDatabase(SqlSugarClient db)
        {
            db.CodeFirst.InitTables<Student, School, Book>();
            db.DbMaintenance.TruncateTable<Student, School, Book>();
            var students = new List<Student>
                                {
                                    new Student {
                                        Name = "Student 1",
                                        SexCode = "M",
                                        School = new School { SchoolName = "School 1" },
                                        Books = new List<Book> {
                                            new Book { Name = "Book 1" },
                                            new Book { Name = "Book 2" }
                                        }
                                    },
                                new Student {
                                    Name = "Student 2",
                                    SexCode = "F",
                                    School =  new School { SchoolName = "School 2" },
                                    Books = new List<Book> {
                                        new Book { Name = "Book 3" }
                                    }
                                }
                               };

            db.InsertNav(students)
                .Include(it => it.School)
                .Include(it => it.Books).ExecuteCommand();



            db.CodeFirst.InitTables<A, B, ABMapping>();
            db.DbMaintenance.TruncateTable<A, B, ABMapping>();
            List<A> a1 = new List<A> { new A() { Name = "A1" }, new A() { Name = "A2" } };
            B b1 = new B { Name = "B1" };
            B b2 = new B { Name = "B2" };
            a1[0].BList = new List<B> { b1,b2 };

            db.InsertNav(a1).Include(x => x.BList).ExecuteCommand();
        }
         
        [SugarTable("Student06")]
        public class Student
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int StudentId { get; set; }
            public string Name { get; set; }
            public string SexCode { get; set; }
            public int SchoolId { get; set; }

            [Navigate(NavigateType.OneToOne, nameof(SchoolId))]
            public School School { get; set; }

            [Navigate(NavigateType.OneToMany, nameof(Book.StudentId))]
            public List<Book> Books { get; set; }
        }
        [SugarTable("School06")]
        public class School
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int SchoolId { get; set; }
            public string SchoolName { get; set; }
        }
        [SugarTable("Book06")]
        public class Book
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int BookId { get; set; }
            public string Name { get; set; }
            public int StudentId { get; set; }
        }
        [SugarTable("A06")]
        public class A
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int AId { get; set; }
            public string Name { get; set; }

            [Navigate(typeof(ABMapping), nameof(ABMapping.AId), nameof(ABMapping.BId))]
            public List<B> BList { get; set; }
        }
        [SugarTable("B06")]
        public class B
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int BId { get; set; }
            public string Name { get; set; }

            [Navigate(typeof(ABMapping), nameof(ABMapping.BId), nameof(ABMapping.AId))]
            public List<A> AList { get; set; }
        }
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
