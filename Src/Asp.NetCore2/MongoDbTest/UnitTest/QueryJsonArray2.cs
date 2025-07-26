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
        }

        [SqlSugar.SugarTable("UnitStudentdsafaz1")]
        public class Student : MongoDbBase
        {
            public string Name { get; set; } 

            [SqlSugar.SugarColumn(IsJson = true)]
            public List<double> Book { get; set; }
        } 
    } 
}
