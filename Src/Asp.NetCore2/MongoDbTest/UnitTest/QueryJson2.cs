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
    public class QueryJson2
    {
        internal static void Init()
        {
            var db =  DbHelper.GetNewDb();
            db.CodeFirst.InitTables<Student>();
            db.DbMaintenance.TruncateTable<Student>();
            db.Insertable(new Student()
            {
                Age = 1,
                Book = new Book() { SchoolId = ObjectId.GenerateNewId().ToString() }
            }).ExecuteCommand();
            var data = db.Queryable<Student>().First();
            var list=db.Queryable<Student>().Where(s => s.Book.SchoolId == data.Book.SchoolId).ToList();
            if (list.Any() == false) Cases.ThrowUnitError(); 
            var ids = new List<string>() { data.Book.SchoolId};
            var list2 = db.Queryable<Student>().Where(s => ids.Contains( s.Book.SchoolId ) ).ToList();
            if (list2.Any() == false) Cases.ThrowUnitError();
        }

        [SqlSugar.SugarTable("UnitStudentdddd1")]
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
            [BsonRepresentation(BsonType.ObjectId)]//比普通类多个序列化ObjectId
            [SqlSugar.SugarColumn(ColumnDataType = nameof(ObjectId))]
            public string SchoolId { get; set; }

            public decimal BookId { get; set; }
        }
    }

}
