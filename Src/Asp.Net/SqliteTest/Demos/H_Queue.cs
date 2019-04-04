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
            var x= db.SaveQueues();

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
            db.Queryable<School>().AddQueue();
            var result2 = db.SaveQueues<Student, School, School>();
 
        }
    }
}
