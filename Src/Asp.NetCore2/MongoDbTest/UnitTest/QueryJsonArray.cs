using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SqlSugar;
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
            var db = DbHelper.GetNewDb();
            db.CodeFirst.InitTables<Student>();
            db.DbMaintenance.TruncateTable<Student>();
            db.Insertable(new Student() { Age = 1, Name = "tom", SchoolId = "a", Book = new List<Book>() { new Book() { CreateTime = DateTime.Now, Price = 21 } } }).ExecuteCommand();
            var data1=db.Queryable<Student>().ToList();
            if (data1.First().Book.Count != 1)  Cases.ThrowUnitError();
            if (data1.First().Book.First().Price != 21) Cases.ThrowUnitError();
            data1.First().Book.First().Price = 100;
            db.Updateable(data1).ExecuteCommand();
            var data2 = db.Queryable<Student>().ToList(); 
            if (data2.First().Book.First().Price != 100) Cases.ThrowUnitError();
            var exp=Expressionable.Create<Student>().ToExpression();
            var data3 = db.Queryable<Student>().Where(exp).ToList();
            db.Insertable(new Student() { Age = 1, Name = "haha", SchoolId = "1", Book = new List<Book>() { new Book() { CreateTime = DateTime.Now, Price = 21 } } }).ExecuteCommand();
            var data4=db.Queryable<Student>().Where(it => it.Book.Any(s => s.Price == 21)).ToList(); 
            if(data4.Count!=1||data4.First().Book.First().Price!=21) Cases.ThrowUnitError();

            db.CodeFirst.InitTables<IdsModel>();
            db.DbMaintenance.TruncateTable<IdsModel>();
            var ids = new List<string> { ObjectId.GenerateNewId() + "" };
            var sid =ObjectId.GenerateNewId() + "";
            db.Insertable(new IdsModel() {name="a", Ids =ids,Students=new List<Student>() {
              new Student(){ Id =sid}
            } }).ExecuteCommand();
            db.Insertable(new IdsModel()
            {
                name = "b",
                Ids = new List<string> { ObjectId.GenerateNewId()+"" },
                Students = new List<Student>() {
              new Student(){ Id =ObjectId.GenerateNewId()+""}
            }
            }).ExecuteCommand();
            var x = ids.Last();
            var list2=db.Queryable<IdsModel>().Where(it => it.Ids.Contains(x)).ToList();
            if (list2.Count != 1) Cases.ThrowUnitError();
            if (!list2.First().Ids.Contains(x)) Cases.ThrowUnitError();
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
        public class IdsModel 
        {
            public string name { get; set; }
            [SugarColumn(IsJson =true)] 
            public List<string> Ids { get; set; }
            [SugarColumn(IsJson = true)]
            public List<Student> Students { get; set; }
        }
        public class Book
        {
            public decimal Price { get; set; }
            public DateTime CreateTime { get; set; }
        }
    }

}
