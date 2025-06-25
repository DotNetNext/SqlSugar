using SqlSugar.MongoDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbTest
{
    public class QueryJson
    {
        internal static void Init()
        {
            var db =  DbHelper.GetNewDb();
            db.CodeFirst.InitTables<Student>();
            db.DbMaintenance.TruncateTable<Student>();
            db.Insertable(new Book() { Price = 1, CreateTime = DateTime.Now }).ExecuteCommand();
            var data1=db.Queryable<Book>().Where(it => it.Price == 1).ToList();
            db.Insertable(new Student() { Age = 1, Name = "tom", SchoolId = "a", Book = new Book() { CreateTime = DateTime.Now, Price = 1 } }).ExecuteCommand();
            db.Insertable(new Student() { Age = 1, Name = "tom2", SchoolId = "a2", Book = new Book() { CreateTime = DateTime.Now, Price = 2 } }).ExecuteCommand();
            var data2 = db.Queryable<Student>().Where(it => it.Book.Price == 1).ToList();
            if (data2.Count != 1) Cases.ThrowUnitError();
            if (data2.First().Book.Price != 1) Cases.ThrowUnitError();
            data2.First().Book.Price = 100;
            db.Updateable(data2).ExecuteCommand();
            var data3 = db.Queryable<Student>().Where(it => it.Book.Price == 100).ToList();
            if (data3.Count != 1) Cases.ThrowUnitError();
            if (data2.First().Book.Price != 100) Cases.ThrowUnitError();
        }

        [SqlSugar.SugarTable("UnitStudentdfsds3zzz1")]
        public class Student : MongoDbBase
        {
            public string Name { get; set; }

            public string SchoolId { get; set; }

            public int Age { get; set; }

            public DateTime CreateDateTime { get; set; }

            [SqlSugar.SugarColumn(IsJson = true)]
            public Book Book { get; set; }
        }

        public class Book
        {
            public decimal Price { get; set; }
            public DateTime CreateTime { get; set; }
        }
    }

}
