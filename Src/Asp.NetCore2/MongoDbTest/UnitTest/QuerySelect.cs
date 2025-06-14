using MongoDB.Bson;
using MongoDB.Driver;
using MongoDbTest.DBHelper;
using SqlSugar.MongoDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbTest 
{
    public class QuerySelect
    {
        internal static void Init()
        { 
            var db = DBHelper.DbHelper.GetNewDb();
            db.CodeFirst.InitTables<Student>(); ;
            var dt = DateTime.Now;
            var studentId = db.Insertable(new Student() { CreateDateTime=dt, Name="a", SchoolId="aa" })
               .ExecuteCommand(); 
            var list=db.Queryable<Student>().Select(it => new
            {
                date=it.CreateDateTime.Date
            }).ToList();
            if (list.First().date != dt.Date) Cases.ThrowUnitError();
        }
        [SqlSugar.SugarTable("UnitStudent1231sds3z1")]
        public class Student : MongoDbBase
        {
            public string Name { get; set; }

            public string SchoolId { get; set; }

            public DateTime CreateDateTime { get; set; }
        } 
    }
}
