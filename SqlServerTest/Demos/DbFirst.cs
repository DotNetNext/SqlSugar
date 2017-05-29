using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest.Demo
{
    public class DbFirst:DemoBase
    {
        public static void Init()
        {
            var db = GetInstance();
            //Create all class
            db.DbFirst.CreateClassFile("c:\\Demo\\1");

            //Create student calsss
            db.DbFirst.Where("Student").CreateClassFile("c:\\Demo\\2");
            //Where(array)

            //Mapping name
            db.MappingTables.Add("ClassStudent", "Student");
            db.DbFirst.Where("Student").CreateClassFile("c:\\Demo\\3");

            //Remove mapping
            db.MappingTables.Clear();

            //Create class with default value
            db.DbFirst.IsCreateDefaultValue().CreateClassFile("c:\\Demo\\4","Demo.Models");
        }
    }
}