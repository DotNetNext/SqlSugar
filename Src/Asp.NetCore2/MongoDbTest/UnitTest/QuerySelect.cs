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
            db.CodeFirst.InitTables<Student>();
            db.DbMaintenance.TruncateTable<Student>();
            var dt = DateTime.Now;
            var studentId = db.Insertable(new Student() { CreateDateTime=dt, Name="a", SchoolId="aa" })
               .ExecuteCommand(); 
            var list=db.Queryable<Student>().Select(it => new
            {
                date=it.CreateDateTime.Date,
                year=it.CreateDateTime.Year,
                day=it.CreateDateTime.Day,
                hour=it.CreateDateTime.Hour,
                Minute = it.CreateDateTime.Minute,
                month=it.CreateDateTime.Month,
                Second=it.CreateDateTime.Second
            }).ToList();
            if (list.First().date != dt.Date) Cases.ThrowUnitError();
            if (list.First().year != dt.Date.Year) Cases.ThrowUnitError();
            if (list.First().month != dt.Date.Month) Cases.ThrowUnitError();
            if (list.First().day != dt.Date.Day) Cases.ThrowUnitError();
            if (list.First().hour != dt.Hour) Cases.ThrowUnitError();
            if (list.First().Minute != dt.Minute) Cases.ThrowUnitError();
            if (list.First().Second != dt.Second) Cases.ThrowUnitError();
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
