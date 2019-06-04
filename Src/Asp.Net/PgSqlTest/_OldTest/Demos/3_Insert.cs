using OrmTest.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest.Demo
{
    public class Insert:DemoBase
    {
        public static void Init()
        {
            var db = GetInstance();
            db.IgnoreColumns.Add("TestId", "Student");

            var insertObj = new Student() { Name = "jack", CreateTime = Convert.ToDateTime("2010-1-1"), SchoolId = 1 };

            //Insert reutrn Insert Count
            var t2 = db.Insertable(insertObj).ExecuteCommand();

            //Insert reutrn Identity Value
            var t3 = db.Insertable(insertObj).ExecuteReturnIdentity();

            //Insert reutrn Identity Value
            var t31 = db.Insertable(insertObj).ExecuteReturnEntity();

            //Only  insert  Name and SchoolId
            var t4 = db.Insertable(insertObj).InsertColumns(it => new { it.Name, it.SchoolId }).ExecuteReturnIdentity();
            var t4_1 = db.Insertable(insertObj).InsertColumns(it => it=="Name"||it== "SchoolId").ExecuteReturnIdentity();


            //Ignore TestId
            var t5 = db.Insertable(insertObj).IgnoreColumns(it => new { it.Name, it.TestId }).ExecuteReturnIdentity();


            //Ignore   TestId
            var t6 = db.Insertable(insertObj).IgnoreColumns(it => it == "Name" || it == "TestId").ExecuteReturnIdentity();


            //Use Lock
            var t8 = db.Insertable(insertObj).With(SqlWith.UpdLock).ExecuteCommand();


            var insertObj2 = new Student() { Name = null, CreateTime = Convert.ToDateTime("2010-1-1") };
            var t9 = db.Insertable(insertObj2).Where(true/* Is insert null */, false/*off identity*/).ExecuteCommand();

            //Insert List<T>
            var insertObjs = new List<Student>();
            for (int i = 0; i < 1000; i++)
            {
                insertObjs.Add(new Student() { Name = "name" + i });
            }
            var t10 = db.Insertable(insertObjs.ToArray()).InsertColumns(it => new { it.Name }).ExecuteCommand();

            var t11 = db.Insertable(insertObjs.ToArray()).ExecuteCommand();


            var t12 = db.Insertable(insertObj).IgnoreColumns(it => it == "Name" || it == "TestId").ExecuteReturnIdentityAsync();
            t12.Wait();


            var dt = new Dictionary<string, object>();
            dt.Add("name", "1");
            dt.Add("CreateTime", DateTime.Now);
            var t66 = db.Insertable(dt).AS("student").ExecuteCommand();
        }
    }
}
