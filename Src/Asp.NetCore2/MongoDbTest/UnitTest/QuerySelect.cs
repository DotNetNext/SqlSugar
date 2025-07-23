using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq; 
using SqlSugar;
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
            var db =DbHelper.GetNewDb();
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
                AddMonth = it.CreateDateTime.AddMonths(1),
                datediff=SqlFunc.DateDiff(DateType.Day,it.CreateDateTime,DateTime.Now.AddDays(2))
            }).ToList();
            if(list4.First().datediff!=2) Cases.ThrowUnitError();
            if (list4.First().Day.Date != dt.Date.AddDays(1)) Cases.ThrowUnitError();
            if (list4.First().Year.Date != dt.Date.AddYears(1)) Cases.ThrowUnitError();
            if (list4.First().AddMonth.Date != dt.Date.AddMonths(1)) Cases.ThrowUnitError();

            var list5 = db.Queryable<Student>().Select(it => new
            {
                Day =2
            }).ToList();
            if (list5.First().Day!=2) Cases.ThrowUnitError();

            var list6 = db.Queryable<Student>().Select(it => new Student()
            {
                  Age = 2
            }).ToList();
            if (list6.First().Age != 2) Cases.ThrowUnitError();

            var list7 = db.Queryable<Student>().Select(it => new Student()
            {
                Age = true?2:1
            }).ToList();
            if (list7.First().Age != 2) Cases.ThrowUnitError();

            var list8 = db.Queryable<Student>().Select(it => new Student()
            {
                Age = true ? it.Age : it.Age
            }).ToList();
            if (list8.First().Age != 11) Cases.ThrowUnitError();

            var list9 = db.Queryable<Student>().Select(it => new Student()
            {
                Age = it.Age>0 ? it.Age: 1,
            }).ToList();
            if (list9.First().Age != 11) Cases.ThrowUnitError();

            var list10 = db.Queryable<Student>().Select(it => new  
            {
                Age = it.Age > 0 ? it.Age : 1,
                Age2 = it.Age < 0 ? it.Age : 1,
                Age3 = false ? 0:it.Age,
                Age4 = true ? 100 : it.Age,
                Age5= it.Age <0 ? 1:it.Age  ,
                Age6 = it.Age > 0 ? 1 : it.Age,
            }).ToList();
            if (list10.First().Age != 11) Cases.ThrowUnitError();
            if (list10.First().Age2 != 1) Cases.ThrowUnitError();
            if (list10.First().Age3 != 11) Cases.ThrowUnitError();
            if (list10.First().Age4 != 100) Cases.ThrowUnitError();
            if (list10.First().Age5 != 11) Cases.ThrowUnitError();
            if (list10.First().Age6 != 1) Cases.ThrowUnitError();

            var list11 = db.Queryable<Student>().Select(it => new
            {
                Age = it.Age > 0 ? it.Age +1: 1,
                Age2 = it.Age > 0 ? 2+it.Age  : 1,
            }).ToList();
            if (list11.First().Age != 12) Cases.ThrowUnitError();
            if (list11.First().Age2 != 13) Cases.ThrowUnitError();

            var list12= db.Queryable<Student>().Select(it => new
            {
                Age = -it.Age  
            }).ToList();
            if(list12.First().Age!=-11) Cases.ThrowUnitError();

            db.Insertable(new Student() { CreateDateTime = dt, Age = 12, Name = "a2", SchoolId = "aa22" })
            .ExecuteCommand();
            db.Insertable(new Student() { CreateDateTime = dt, Age = 13, Name = "b", SchoolId = "bss" })
           .ExecuteCommand();
            var count=db.Queryable<Student>().Count();
            var count2 = db.Queryable<Student>().ToList().Count();
            var count21 = db.Queryable<Student>().CountAsync().GetAwaiter().GetResult();
            if (count2 != count|| count != count21) Cases.ThrowUnitError();

            var count3 = db.Queryable<Student>().Where(it=>it.Age==11).Count();
            var count4 = db.Queryable<Student>().Where(it => it.Age == 11).ToList().Count();
            if (count3 != count4) Cases.ThrowUnitError();

            var isAny=db.Queryable<Student>().Where(it => it.Age == 11).Any();
            var isAny2 = db.Queryable<Student>().Where(it => it.Age == 11111).Any();
            if (!isAny || isAny2) Cases.ThrowUnitError();

            int countp = 0;
            RefAsync<int> countp2 = 0;
            var list13=db.Queryable<Student>().Where(it => it.Age >0).ToPageList(1, 2, ref countp);
            var list131 = db.Queryable<Student>().Where(it => it.Age > 0).ToPageListAsync(1, 2,  countp2).GetAwaiter().GetResult();
            if (count != countp|| list13.Count!=2||countp!= countp2.Value) Cases.ThrowUnitError();

            var list14=db.Queryable<Student>().OrderBy(it => it.Age).ToList();
            var list15 = db.Queryable<Student>().OrderBy(it => it.Age).ToListAsync().GetAwaiter().GetResult();
            if (list14.Count != list15.Count) Cases.ThrowUnitError();

            var list16=db.Queryable<Student>().GroupBy(it => it.Age)
                .Select(it => new
                {
                     name=SqlFunc.AggregateMin(it.Name),
                     age=it.Age
                }).ToList();

            var list17 = db.Queryable<Student>()  
                .Where(it=>SqlFunc.Length(it.Name)>1)
               .Select(it => new
               { 
                   name2=it.Name,
                   name = it.Name.Substring(0,1),  
               }).ToList();
            if(list17.First().name2!="a2"||list17.First().name.Substring(0,1)!= list17.First().name) Cases.ThrowUnitError();

            var list18 = db.Queryable<Student>() 
             .Select(it => new
             { 
                 name = it.Name.Length,
             }).ToList();
            if(list18.Max(it=>it.name)!=2) Cases.ThrowUnitError();

            var list19= db.Queryable<Student>()
           .Select(it => new
           {
               name =string.IsNullOrEmpty(it.Name),
           }).ToList();
            if (list19.Max(it => it.name) != true) Cases.ThrowUnitError();

            var list20 = db.Queryable<Student>()
              .Select(it => new
              {
                  name = string.IsNullOrEmpty(it.Name).ToString(),
              }).ToList();
            if (list20.Max(it => it.name) != "true") Cases.ThrowUnitError();

            var list22= db.Queryable<Student>()
            .Select(it=>it.Name).ToList();
            if (list22.First() != "a") Cases.ThrowUnitError();

            var list23 = db.Queryable<Student>()
             .Select(it => it.Name.ToUpper()).ToList();
            if (list23.First() != "A") Cases.ThrowUnitError();

            var list24 = db.Queryable<Student>().Where(it => it.Name == it.Name).ToList();
            if (list24.Count!=3||list24.First().Name != "a") Cases.ThrowUnitError();

            var list25 = db.Queryable<Student>().Where(it => it.Name == it.Name && it.Name=="a").ToList();
            if (list25.Count!=1||list25.First().Name != "a") Cases.ThrowUnitError();

            var list26 = db.Queryable<Student>().Select(it => new  
            {
                Age1 =  it.Age > 0? it.Age: 1,
                Age2 =SqlFunc.IIF( it.Age > 0 , it.Age ,1),
            }).ToList();
            if (list26.First().Age1 != list26.First().Age2) Cases.ThrowUnitError();

            var id = db.Queryable<Student>().First().Id;
            var data1 = db.Queryable<Student>().Where(it=>it.Id==id).Single();
            var data2=db.Queryable<Student>().InSingle(id);
            var data3 = db.Queryable<Student>().InSingleAsync(id).GetAwaiter().GetResult();
            if (data1.Id != data2.Id || data2.Id != data3.Id) Cases.ThrowUnitError();
          
            db.DbMaintenance.TruncateTable<Student>();
            db.Insertable(new Student() { Age = 1 }).ExecuteCommand();
            db.Insertable(new Student() { Age = 10 }).ExecuteCommand();
            db.Insertable(new Student() { Age = 20 }).ExecuteCommand();
            db.Insertable(new Student() { Age = 30 }).ExecuteCommand();
            var list27 = db.Queryable<Student>().Select(it => new
            { 
                age=it.Age,
                age2 = SqlFunc.IF(it.Age >=30).Return(it.Age).ElseIF(it.Age>=20).Return(it.Age).ElseIF(it.Age>=10).Return(11).End(10),
            }).ToList();
            if (list27.Any(it=>it.age==1&&it.age2!=10)) Cases.ThrowUnitError();
            if (list27.Any(it => it.age == 10 && it.age2 != 11)) Cases.ThrowUnitError();
            if (list27.Any(it => it.age == 20 && it.age2 != 20)) Cases.ThrowUnitError();
            if (list27.Any(it => it.age == 30 && it.age2 != 30)) Cases.ThrowUnitError();
            var list28 = db.Queryable<Student>().Select(it => new
            {
                age = it.Age,
                age2 = SqlFunc.IF(it.Age >= 30).Return(it.Age).ElseIF(it.Age >= 10).Return(11).End(10),
            }).ToList();
            if (list28.Any(it => it.age <10 && it.age2 != 10)) Cases.ThrowUnitError();
            if (list28.Any(it => it.age>=10 &&it.age<30&& it.age2 != 11)) Cases.ThrowUnitError();
            if (list28.Any(it => it.age >=30 && it.age2 != 30)) Cases.ThrowUnitError();
            var list29 = db.Queryable<Student>().Select(it => new
            {
                age = it.Age,
                age2 = SqlFunc.IF(it.Age >= 15).Return(1).End(2),
            }).ToList();
            if (list29.Any(it => it.age >15 && it.age2 != 1)) Cases.ThrowUnitError();
            if (list29.Any(it => it.age < 15 && it.age2 != 2)) Cases.ThrowUnitError();
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
