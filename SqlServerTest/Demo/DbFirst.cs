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
            db.DbFirst.CreateClassFile("c:\\Demo\\all");
        }
    }
}