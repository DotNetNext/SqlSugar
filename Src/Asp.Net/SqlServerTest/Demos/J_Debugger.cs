using OrmTest.Demo;
using OrmTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest.Demo
{
    public class Debugger : DemoBase
    {
        public static void Init()
        {
            var db = GetInstance();
            db.CurrentConnectionConfig.Debugger = new SqlSugar.SugarDebugger() { EnableThreadSecurityValidation = true };

            db.Queryable<Student>().ToList();
            db.Queryable<Student>().ToListAsync().Wait();
            db.Insertable<Student>(new Student() { Name = "a" }).ExecuteCommandAsync().Wait();
            db.Updateable<Student>(new Student() { Name = "a" }).ExecuteCommandAsync().Wait();
            db.Deleteable<Student>(1111).ExecuteCommandAsync().Wait();

            var task = new Task(() =>
                                      {
                                          try
                                          {
                                              //is error 
                                              Console.WriteLine("is error");
                                              db.Queryable<Student>().ToList();
                                          }
                                          catch (Exception ex)
                                          {
                                              Console.WriteLine(ex.Message);
                                          }

                                      });

            task.Start();
            task.Wait();

            for (int i = 0; i < 10; i++)
            {
                var task2 = new Task(() =>
                {
                    //is ok
                    Console.WriteLine("is ok");
                    var db2 = GetInstance();
                    db2.CurrentConnectionConfig.Debugger = new SqlSugar.SugarDebugger() { EnableThreadSecurityValidation = true };
                    db2.Queryable<Student>().ToList();
                    db2.Queryable<Student>().ToList();
                });
                task2.Start();
                task2.Wait();
            }

        }


    }
}