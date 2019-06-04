using OrmTest.Demo;
using OrmTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest.Demo
{
    /// <summary>
    /// Secure string operations
    /// </summary>
    public class JoinSql : DemoBase
    {
        public static void Init()
        {
            Where();
            OrderBy();
        }

        private static void Where()
        {
            var db = GetInstance();
            //Parameterized processing
            string value = "'jack';drop table Student";
            var list = db.Queryable<Student>().Where("name=@name", new { name = value }).ToList();
            //Nothing happened
        }

        private static void OrderBy()
        {
            var db = GetInstance();
            //propertyName is valid
            string propertyName = "Id";
            string dbColumnName = db.EntityMaintenance.GetDbColumnName<Student>(propertyName);
            var list = db.Queryable<Student>().OrderBy(dbColumnName).ToList();

            //propertyName is invalid
            try
            {
                propertyName = "Id'";
                dbColumnName = db.EntityMaintenance.GetDbColumnName<Student>(propertyName);
                var list2 = db.Queryable<Student>().OrderBy(dbColumnName).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
