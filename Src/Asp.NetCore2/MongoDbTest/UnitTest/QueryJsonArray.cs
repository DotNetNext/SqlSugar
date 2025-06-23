using SqlSugar.MongoDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbTest
{
    public class QueryJsonArray
    {
        internal static void Init()
        {
            var db = DBHelper.DbHelper.GetNewDb();
            db.CodeFirst.InitTables<Student>();
            db.DbMaintenance.TruncateTable<Student>();
            db.Insertable(new Student() { Age = 1, Name = "tom", SchoolId = "a", Book = new List<Book>() { new Book() { CreateTime = DateTime.Now, Price = 21 } } }).ExecuteCommand();
            var data1=db.Queryable<Student>().ToList();
            if (data1.First().Book.Count != 1)  Cases.ThrowUnitError();
            if (data1.First().Book.First().Price != 21) Cases.ThrowUnitError();
        }

        [SqlSugar.SugarTable("UnitStudentdfsds3zzz1")]
        public class Student : MongoDbBase
        {
            public string Name { get; set; }

            public string SchoolId { get; set; }

            public int Age { get; set; }

            public DateTime CreateDateTime { get; set; }

            [SqlSugar.SugarColumn(IsJson = true)]
            public List<Book> Book { get; set; }
        }

        public class Book
        {
            public decimal Price { get; set; }
            public DateTime CreateTime { get; set; }
        }
    }

}
