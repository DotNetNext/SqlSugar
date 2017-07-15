using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OrmTest.Demo;
using OrmTest.Models;

namespace OrmTest.UnitTest
{
    public class EnumTest : UnitTestBase
    {
        private EnumTest() { }
        public EnumTest(int eachCount)
        {
            this.Count = eachCount;
        }
        public void Init()
        {

            var db = GetInstance();
            var shoolValue = SchoolEnum.HarvardUniversity;
            var list = db.Queryable<StudentEnum>().AS("student").Where(it => it.SchoolId == shoolValue).ToList();
        }
    }
}