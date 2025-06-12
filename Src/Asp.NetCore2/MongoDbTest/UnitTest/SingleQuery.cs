using MongoDB.Bson.Serialization.IdGenerators;
using SqlSugar.MongoDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbTest 
{
    public class SingleQuery
    {
        public static void Init() 
        {
            var db = DBHelper.DbHelper.GetNewDb();

            db.DbMaintenance.TruncateTable<Student>();
            db.DbMaintenance.TruncateTable<School>(); 

            var schoolId = db.Insertable(new School() { Name = "XX大学" }).ExecuteReturnPkList<string>().First();
            var studentId=db.Insertable(new Student() { Name = "jack",SchoolId= schoolId }).ExecuteReturnPkList<string>().First();
            var studentData = db.Queryable<Student>().Where(it => it.Id == studentId).First();
            var schoolData = db.Queryable<School>().Where(it => it.Id == schoolId).First(); 
            if (studentData.Name != "jack" || schoolData.Name!= "XX大学") Cases.ThrowUnitError(); 

            db.Insertable(new School() { Name = "zz大学" }).ExecuteCommand();
            db.Insertable(new School() { Name = "yy大学" }).ExecuteCommand();
            var tableCount = db.Queryable<School>().ToList().Count;
            if(tableCount!=3) Cases.ThrowUnitError();

            db.Insertable(new School() { Name = "ss大学" }).ExecuteCommand();
            db.Insertable(new School() { Name = "yy大学" }).ExecuteCommand();
            var count = 0;
            var list = db.Queryable<School>().OrderBy(it=>it.Name).ToPageList(1,2,ref count);
            if(count != 5||list.Count!=2) Cases.ThrowUnitError();

            var list3 = db.Queryable<School>().Where(it=>it.Name== "zz大学"||it.Name== "ss大学").ToPageList(1, 2, ref count);
            if(count!=2||list3.Count!=2) Cases.ThrowUnitError();
            if (list3.First().Name!= "zz大学"|| list3.Last().Name != "ss大学") Cases.ThrowUnitError();
        }
    }
    [SqlSugar.SugarTable("UnitStudent123131")]
    public class Student : MongoDbBase 
    {
        public string Name { get; set; }

        public string SchoolId { get; set; }
    }
    [SqlSugar.SugarTable("UnitSchool123131")]
    public class School : MongoDbBase 
    {
        public string Name{ get; set; }
    }
}
