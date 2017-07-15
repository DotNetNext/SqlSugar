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

            var x = new StudentEnum()
            {
                Name = shoolValue.ToString(),
                SchoolId = shoolValue
            };
            var id= db.Insertable(x).AS("student").ExecuteReutrnIdentity();
            var data = db.Queryable<StudentEnum>().AS("student").InSingle(id);
            shoolValue = SchoolEnum.UniversityOfOxford;
           var sql= db.Updateable<StudentEnum>().AS("student").UpdateColumns(it=>new StudentEnum() { Name="a" , SchoolId= shoolValue }).Where(it=>it.Id==id).ToSql();
        }
    }
}