using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
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
            var studentId = db.Insertable(new Student() { CreateDateTime=dt,Age=11, Name="a", SchoolId="aa" })
               .ExecuteCommand(); 
            var list=db.Queryable<Student>().Select(it => new
            {
                date=it.CreateDateTime.Date,
                year=it.CreateDateTime.Year,
                day=it.CreateDateTime.Day,
                hour=it.CreateDateTime.Hour,
                Minute = it.CreateDateTime.Minute,
                month=it.CreateDateTime.Month,
                Second=it.CreateDateTime.Second,
                Week = it.CreateDateTime.DayOfWeek
            }).ToList();
            if (list.First().date != dt.Date) Cases.ThrowUnitError();
            if (list.First().year != dt.Date.Year) Cases.ThrowUnitError();
            if (list.First().month != dt.Date.Month) Cases.ThrowUnitError();
            if (list.First().day != dt.Date.Day) Cases.ThrowUnitError();
            if (list.First().hour != dt.Hour) Cases.ThrowUnitError();
            if (list.First().Minute != dt.Minute) Cases.ThrowUnitError();
            if (list.First().Second != dt.Second) Cases.ThrowUnitError();
            if (list.First().Week != dt.DayOfWeek) Cases.ThrowUnitError();
            var list2 = db.Queryable<Student>().Select(it => new
            {
                date = it.CreateDateTime.ToString("yyyy-MM-dd")
            }).ToList();
            if(list2.First().date!=dt.ToString("yyyy-MM-dd")) Cases.ThrowUnitError();
            var list3 = db.Queryable<Student>().Select(it => new
            {
                date = it.Age.ToString(),
                int32 = Convert.ToInt32(it.Age),
                dateTime = Convert.ToDateTime(it.CreateDateTime),
            }).ToList(); ;
            var list4 = db.Queryable<Student>().Select(it => new
            {
                Day = it.CreateDateTime.AddDays(1),
                Year = it.CreateDateTime.AddYears(1),
                AddMonth = it.CreateDateTime.AddMonths(1)
            }).ToList();
            if (list4.First().Day.Date != dt.Date.AddDays(1)) Cases.ThrowUnitError();
            if (list4.First().Year.Date != dt.Date.AddYears(1)) Cases.ThrowUnitError();
            if (list4.First().AddMonth.Date != dt.Date.AddMonths(1)) Cases.ThrowUnitError();
        }
        [SqlSugar.SugarTable("UnitStudent1231sds3z1")]
        public class Student : MongoDbBase
        {
            public string Name { get; set; }

            public string SchoolId { get; set; }

            public int Age { get; set; }

            public DateTime CreateDateTime { get; set; }
        } 
    }
}
