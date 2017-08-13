using OrmTest.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest.UnitTest
{
    public class MapTable : UnitTestBase
    {
        public void Init()
        {
            //IsAutoCloseConnection
            for (int i = 0; i < 200; i++)
            {
                var db = GetInstance();
                var x = db.Queryable<Student>().ToList(); 
            }
        }
    }
}
