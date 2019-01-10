using OrmTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest.Demo
{
    public class Mapper : DemoBase
    {
        public static void Init()
        {
            var db = GetInstance();


            var x = db.Queryable<Student>()

                   .Mapper((it, cache) =>
                   {
                     
                       it.Name = "xx";
                   }).ToListAsync();

          x .Wait();


        }
    }
}
