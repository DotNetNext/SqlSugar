using OrmTest.Demo;
using OrmTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest.Demo
{
    public class VersionValidation : DemoBase
    {
        public static void Init()
        {
            var db = GetInstance();
            try
            {
                for (int i = 0; i < 10; i++)
                {
                    var data = new StudentVersion()
                    {
                        Id = db.Queryable<Student>().Select(it => it.Id).First(),
                        CreateTime = DateTime.Now,
                        Name = ""
                    };
                    db.Updateable(data).AS("student").ExecuteCommand();

                    var time = db.Queryable<Student>().Where(it=>it.Id==data.Id).Select(it => it.CreateTime).Single();
                    data.CreateTime = time.Value;
                    db.Updateable(data).AS("student").ExecuteCommand();

                    data.CreateTime = time.Value.AddMilliseconds(-1);
                    db.Updateable(data).AS("student").CloseVersionValidation().ExecuteCommand();//Close Version Validation
                    db.Updateable(data).AS("student").ExecuteCommand(); 
                }
            }
            catch (Exception ex)
            {
                if (ex is SqlSugar.VersionExceptions)
                {
                    Console.Write(ex.Message);
                }
                else {

                }
            }
        }

        public class StudentVersion
        {
            public int Id { get; set; }
            public string Name { get; set; }
            [SqlSugar.SugarColumn(IsEnableUpdateVersionValidation = true)]
            public DateTime CreateTime { get; set; }
        }
    }
}
