using MongoDb.Ado.data;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using SqlSugar;
using SqlSugar.MongoDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbTest
{
    public class QueryJsonArray2
    {
        internal static void Init()
        {
            var db = DbHelper.GetNewDb();
            db.CodeFirst.InitTables<Student>();
            db.DbMaintenance.TruncateTable<Student>();
            db.Insertable(new Student() { Book = new List<double>() { 1, 2.1, 3 } }).ExecuteCommand();
            var list=db.Queryable<Student>().ToList();
            if (list.First().Book[1] != 2.1) Cases.ThrowUnitError();
            list.First().Book[1] = 2.2;
            db.Updateable(list).ExecuteCommand();
            var list2 = db.Queryable<Student>().ToList();
            if (list2.First().Book[1] != 2.2) Cases.ThrowUnitError();
            var list3=db.Queryable<Student>().Where(s => s.Book.Contains(1)).ToList(); 
            var list4 = db.Queryable<Student2>().Where(s => s.Book.Any(s=>s==1)).ToList(); 
            if(list3.Count != 1||list4.Count != 1) Cases.ThrowUnitError();
            var list5 = db.Queryable<Student2>().Where(s => s.Book.Any(s => s == 11)).ToList();
            var list6 = db.Queryable<Student>().Where(s => s.Book.Any(s => s == 11)).ToList();
            if (list5.Count != 0 ) Cases.ThrowUnitError();
            if (list6.Count != 0) Cases.ThrowUnitError();
            db.Insertable(new Student() { Book = new List<double>() {  } }).ExecuteCommand();
            var list7 = db.Queryable<Student>().Where(s => s.Book.Any()).ToList();
            if (!list7.Any()) Cases.ThrowUnitError();
        }

        [SqlSugar.SugarTable("UnitStudentdsafaz1")]
        public class Student : MongoDbBase
        {
            public string Name { get; set; } 

            [SqlSugar.SugarColumn(IsJson = true)]
            public List<double> Book { get; set; }
        }
        [SqlSugar.SugarTable("UnitStudentdsafaz1")]
        public class Student2 : MongoDbBase
        {
            public string Name { get; set; }

            [SqlSugar.SugarColumn(IsJson = true)]
            public  double[] Book { get; set; }
        }
    } 
}
