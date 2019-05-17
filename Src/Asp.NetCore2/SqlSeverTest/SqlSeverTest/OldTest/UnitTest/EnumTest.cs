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
            var enums = new SchoolEnum[] { shoolValue, SchoolEnum.UniversityOfOxford };
            var list = db.Queryable<StudentEnum>().AS("student").Where(it => it.SchoolId == shoolValue).Select(it=>it.SchoolId).ToList();

            var x = new StudentEnum()
            {
                Name = shoolValue.ToString(),
                SchoolId = shoolValue
            };
            var x2 = db.Queryable<StudentEnum>().AS("student").Where(it => enums.Contains(it.SchoolId)).ToSql();
            var id= db.Insertable(x).AS("student").ExecuteReturnIdentity();
            var data = db.Queryable<StudentEnum>().AS("student").InSingle(id);
            shoolValue = SchoolEnum.UniversityOfOxford;
            var sql= db.Updateable<StudentEnum>().AS("student").UpdateColumns(it=>new StudentEnum() { Name="a" , SchoolId= shoolValue }).Where(it=>it.Id==id).ToSql();
            var sql2 = db.Updateable<StudentEnum>().AS("student").UpdateColumns(it => new StudentEnum() { Name = "a", SchoolId = SchoolEnum.UniversityOfOxford }).Where(it => it.Id == id).ToSql();
            var sql3 = db.Updateable<StudentEnum>().AS("student").UpdateColumns(it =>  it.SchoolId == shoolValue  ).ToSql();
        }
    }
}