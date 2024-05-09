using SqlSugar;
using System.Collections.Generic;

namespace Test.Model
{
    [SugarTable("StudentA01")]
    public class Student
    {
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true, IsIdentity = true)]
        public int? Id { get; set; }

        [SugarColumn(ColumnName = "Name")]
        public string Name { get; set; }

        [SugarColumn(ColumnName = "Age")]
        public string  Age { get; set; }

        [Navigate(NavigateType.OneToMany, nameof(Book.PId))]
        public List<Book> Books { get; set; }

        [Navigate(NavigateType.OneToMany, nameof(School.PId))]
        public List<School> Schools { get; set; }
    }
}
