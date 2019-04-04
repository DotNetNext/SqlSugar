using OrmTest.Demo;
using OrmTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest.Demo
{
    public class Queue : DemoBase
    {
        public static void Init()
        {
            var db = GetInstance();
            db.Insertable<Student>(new Student() { Name = "a" }).AddQueue();
            db.Insertable<Student>(new Student() { Name = "b" }).AddQueue();
            db.SaveQueues();

            db.Insertable<Student>(new Student() { Name = "a" }).AddQueue();
            db.Insertable<Student>(new Student() { Name = "b" }).AddQueue();
            db.Insertable<Student>(new Student() { Name = "c" }).AddQueue();
            db.Insertable<Student>(new Student() { Name = "d" }).AddQueue();
            var ar = db.SaveQueuesAsync();
            ar.Wait();


            db.Queryable<Student>().AddQueue();
            db.Queryable<School>().AddQueue();
            var result = db.SaveQueues<Student, School>();

            db.Queryable<Student>().AddQueue();
            db.Queryable<School>().AddQueue();
            db.AddQueue("select @id", new { id = 1 });
            var result2 = db.SaveQueues<Student, School, int>();



            db.AddQueue("select 1");
            db.AddQueue("select 2");
            db.AddQueue("select 3");
            db.AddQueue("select 4");
            db.AddQueue("select 5");
            db.AddQueue("select 6");
            db.AddQueue("select 7");

            var result3 = db.SaveQueues<int, int, int, int, int, int, int>();


            db.AddQueue("select 1");
            var result4 = db.SaveQueues<int >();


            db.AddQueue("select 1");
            db.AddQueue("select 2");
            var result5 = db.SaveQueues<int,int>();


            db.AddQueue("select 1");
            db.AddQueue("select 2");
            db.AddQueue("select 3");
            var result6 = db.SaveQueuesAsync<int, int,int>();
            result6.Wait();
        }
    }
}
