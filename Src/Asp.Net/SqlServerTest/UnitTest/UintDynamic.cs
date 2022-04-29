using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace OrmTest
{
    public class UintDynamic
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;
            var list=db.Queryable<StudentA>()
                .Includes(it => it.Books.Where(z=>z.BookId==1)
                                        .MappingField(z=>z.studenId,()=>it.StudentId)
                                        .MappingField(z => z.BookId, () => it.StudentId).ToList())
                .ToList();
        }

        public class StudentA
        {
            [SugarColumn(IsPrimaryKey = true)]
            public int StudentId { get; set; }
            public string Name { get; set; }
            [SugarColumn(IsNullable = true)]
            public int? SchoolId { get; set; }
            [Navigate(NavigateType.Dynamic,null)]
            public SchoolA SchoolA { get; set; }
            [Navigate(NavigateType.Dynamic, null)]
            public List<BookA> Books { get; set; }

        }
        public class SchoolA
        {
            [SugarColumn(IsPrimaryKey = true)]
            public int SchoolId { get; set; }
            [SugarColumn(ColumnName = "SchoolName")]
            public string School_Name { get; set; }
     
        }

        public class BookA
        {
            [SugarColumn(IsPrimaryKey = true)]
            public int BookId { get; set; }
            [SugarColumn(ColumnName = "Name")]
            public string Names { get; set; }
            public int studenId { get; set; }
        }
    }
}
