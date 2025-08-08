using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using SqlSugar.MongoDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbTest
{
    public class QueryJson4
    {
        internal static void Init()
        {
            var db =  DbHelper.GetNewDb();
            db.CodeFirst.InitTables<Student>();
            db.DbMaintenance.TruncateTable<Student>();
            var dt = DateTime.Now;
            db.Insertable(new Student()
            {
                Age = 1,
                Book = new Book()
                {
                    SchoolId = 1,
                    Book2 = new Book() { SchoolId=1,Book2 = new Book() { SchoolId = 2 } }
                }
            }).ExecuteCommand();
            db.Insertable(new Student()
            {
                Age = 1,
                Book = new Book()
                {
                    SchoolId = 222,
                    Book2 = new Book() { SchoolId = 222, Book2 = new Book() { SchoolId = 2 } }
                }
            }).ExecuteCommand();
            var data1 = db.Queryable<Student>().Where(s=>s.Book.Book2.SchoolId==1).ToList();
            var data2 = db.Queryable<Student>().Where(s => s.Book.Book2.Book2.SchoolId == 2).ToList();
            if (data1.Count() != 1 || data1.Count() != 1) Cases.Init();
            if (data1.First().Book.Book2.SchoolId != 1) Cases.Init();
            if (data2.First().Book.Book2.Book2.SchoolId != 2) Cases.Init();
        }

        [SqlSugar.SugarTable("UnitStudentd23351")]
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
            public int SchoolId { get; set; }
             
            [SqlSugar.SugarColumn(IsJson = true)]
            public Book Book2 { get; set; }
        }
    }

}
