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
            var data5 = db.Queryable<Student>().Where(it => it.Book.Any(s => s.Price == 21||s.Price==100)).ToList();
            db.DbMaintenance.TruncateTable<Student>();
            var id = ObjectId.GenerateNewId()+"";
            db.Insertable(new Student() { Age = 1, Name = "a", SchoolId = "1", Book = new List<Book>() { new Book() { SId=id, CreateTime = DateTime.Now, Price = 21 } } }).ExecuteCommand();
            db.Insertable(new Student() { Age = 1, Name = "b", SchoolId = "1", Book = new List<Book>() { new Book() { SId = id, CreateTime = DateTime.Now, Price = 100 } } }).ExecuteCommand();
            db.Insertable(new Student() { Age = 1, Name = "c", SchoolId = "1", Book = new List<Book>() { new Book() { SId = ObjectId.GenerateNewId() + "", CreateTime = DateTime.Now, Price = 21 } } }).ExecuteCommand();
            var data6= db.Queryable<Student>().Where(it => it.Book.Any(s => s.Price == 21 &&s.SId==id)).ToList();
            if(data6.Count!=1||data6.First().Name!="a") Cases.ThrowUnitError();
            db.Insertable(new Student() { Age = 99, Name = "price=age", SchoolId = "1", Book = new List<Book>() { new Book() { SId = ObjectId.GenerateNewId() + "", CreateTime = DateTime.Now, Price = 99 } } }).ExecuteCommand();
            var data7= db.Queryable<Student>().Where(it => it.Book.Any(s => s.Price == it.Age)).ToList();
            var data8 = db.Queryable<Student>().Where(it => it.Book.Any(s => it.Age == s.Price)).ToList();
            if(data7.Count != 1 || data8.Count!=1) Cases.ThrowUnitError();
            if (data7.FirstOrDefault().Name!= "price=age" || data8.FirstOrDefault().Name != "price=age") Cases.ThrowUnitError();
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
            var list3 = db.Queryable<IdsModel>().Where(it => !it.Ids.Contains(x)).ToList();
            if(list3.Any(s=>s.Ids.Contains(x))) Cases.ThrowUnitError();
            db.Insertable(new IdsModel()
            {
                name = "b",
                Ids = new List<string> { ObjectId.GenerateNewId() + "", ObjectId.GenerateNewId() + "" },
                Students = new List<Student>() {
              new Student(){ Id =ObjectId.GenerateNewId()+""}
            }
            }).ExecuteCommand();
            var list4 = db.Queryable<IdsModel>().Select(it => it.Ids.Count()).ToList();
            if (list4.Last()!=2) Cases.ThrowUnitError();
            var list6= db.Queryable<IdsModel>().Select(it => new IdsModel { Students= it.Students }).ToList();
            var list5 = db.Queryable<IdsModel>().Select(it => new { it.Students}).ToList();
            if (list5.Last().Students.First().Id!= list6.Last().Students.First().Id) Cases.ThrowUnitError();
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

            [BsonRepresentation(BsonType.ObjectId)]
            [SqlSugar.SugarColumn(ColumnDataType = nameof(ObjectId))]
            public string SId { get; set; }
        }
    }

}
