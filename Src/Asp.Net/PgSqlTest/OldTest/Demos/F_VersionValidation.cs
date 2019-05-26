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
            TimestampDemo();
            DateTimeDemo();
        }

        private static void TimestampDemo()
        {
            var db = GetInstance();
            try
            {

                var data = new StudentVersion()
                {
                    Id = db.Queryable<Student>().Select(it => it.Id).First(),
                    CreateTime = DateTime.Now,
                    Name = "",
                };
                db.Updateable(data).IgnoreColumns(it => new { it.Timestamp }).ExecuteCommand();

                var time = db.Queryable<StudentVersion>().Where(it => it.Id == data.Id).Select(it => it.Timestamp).Single();

                data.Timestamp = time;

                //is ok
                db.Updateable(data).IsEnableUpdateVersionValidation().IgnoreColumns(it => new { it.Timestamp }).ExecuteCommand();
                //updated Timestamp change

                //is error
                db.Updateable(data).IsEnableUpdateVersionValidation().IgnoreColumns(it => new { it.Timestamp }).ExecuteCommand();

                //IsEnableUpdateVersionValidation Types of support  int or long or byte[](Timestamp) or Datetime 

            }
            catch (Exception ex)
            {
                if (ex is SqlSugar.VersionExceptions)
                {
                    Console.Write(ex.Message);
                }
                else
                {

                }
            }
        }
        private static void DateTimeDemo()
        {
            var db = GetInstance();
            try
            {

                var data = new StudentVersion2()
                {
                    Id = db.Queryable<Student>().Select(it => it.Id).First(),
                    CreateTime = DateTime.Now,
                    Name = "",
                };
                db.Updateable(data).ExecuteCommand();

                var time = db.Queryable<StudentVersion2>().Where(it => it.Id == data.Id).Select(it => it.CreateTime).Single();

                data.CreateTime = time;

                //is ok
                db.Updateable(data).IsEnableUpdateVersionValidation().ExecuteCommand();


                data.CreateTime = time.AddMilliseconds(-1);
                //is error
                db.Updateable(data).IsEnableUpdateVersionValidation().ExecuteCommand();

                //IsEnableUpdateVersionValidation Types of support  int or long or byte[](Timestamp) or Datetime 

            }
            catch (Exception ex)
            {
                if (ex is SqlSugar.VersionExceptions)
                {
                    Console.Write(ex.Message);
                }
                else
                {

                }
            }
        }

        [SqlSugar.SugarTable("Student")]
        public class StudentVersion
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public DateTime CreateTime { get; set; }
            [SqlSugar.SugarColumn(IsEnableUpdateVersionValidation = true,IsOnlyIgnoreInsert=true)]
            public byte[] Timestamp { get; set; }
        }

        [SqlSugar.SugarTable("Student")]
        public class StudentVersion2
        {
            public int Id { get; set; }
            public string Name { get; set; }
            [SqlSugar.SugarColumn(IsEnableUpdateVersionValidation = true, IsOnlyIgnoreInsert = true)]
            public DateTime CreateTime { get; set; }
        }
    }
}
