using OrmTest.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest.Demo
{
    public class Update : DemoBase
    {
        public static void Init()
        {
            var db = GetInstance();
            var updateObj = new Student() { Id = 1, Name = "jack", SchoolId = 0, CreateTime = Convert.ToDateTime("2017-05-21 09:56:12.610") };
            var updateObjs = new List<Student>() { updateObj, new Student() { Id = 2, Name = "sun", SchoolId = 0 } }.ToArray();
            db.IgnoreColumns.Add("TestId", "Student");
            //db.MappingColumns.Add("id","dbid", "Student");


            //update reutrn Update Count
            var t1 = db.Updateable(updateObj).ExecuteCommand();
            var t11 = db.Updateable<Student>(it => new Student() { Name = "a", CreateTime = DateTime.Now }).Where(it => it.Id == 11).ExecuteCommand();
            var t111= db.Updateable<Student>(it => it.Name == "Name").Where(it => it.Id == 1).ExecuteCommand();


            //Only  update  Name 
            var t3 = db.Updateable(updateObj).UpdateColumns(it => new { it.Name }).ExecuteCommand();
            var t3_1 = db.Updateable(updateObj).UpdateColumns("Name").ExecuteCommand();


            //Ignore  Name and TestId
            var t4 = db.Updateable(updateObj).IgnoreColumns(it => new { it.Name, it.TestId }).ExecuteCommand();
            //Ignore  Name and TestId
            var t5 = db.Updateable(updateObj).IgnoreColumns("Name","TestId").ExecuteCommand();


            //Use Lock
            var t6 = db.Updateable(updateObj).With(SqlWith.UpdLock).ExecuteCommand();

            //update List<T>
            var t7 = db.Updateable(updateObjs).ExecuteCommand();

            //Re Set Value
            var t8 = db.Updateable(updateObj)
                .ReSetValue(it => it.Name == (it.Name + 1)).ExecuteCommand();
            var t88 = db.Updateable(updateObj)
            .SetColumns(it => it.Name == (it.Name + 1)).ExecuteCommand();

            var t888 = db.Updateable(updateObj).SetColumns(it =>new Student() {  Name="", CreateTime=DateTime.Now }).ExecuteCommand();

            var t8888 = db.Updateable(updateObj).SetColumns(it => new Student() { Name = "", CreateTime = DateTime.Now }).Where(it=>it.Id==1).ExecuteCommand();

            //Where By Expression
            var t9 = db.Updateable(updateObj).Where(it => it.Id == 1).ExecuteCommand();

            //Update By Expression  Where By Expression
            var t10 = db.Updateable<Student>()
                .UpdateColumns(it => new Student() { Name = "a", CreateTime = DateTime.Now })
                .Where(it => it.Id == 11).ExecuteCommand();

            //Rename 
            db.Updateable<School>().AS("Student").UpdateColumns(it => new School() { Name = "jack" }).Where(it => it.Id == 1).ExecuteCommand();
            //Update Student set Name='jack' Where Id=1

            //Column is null no update
            db.Updateable(updateObj).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommand();

            //sql
            db.Updateable(updateObj).Where("id=@x", new { x = "1" }).ExecuteCommand();
            db.Updateable(updateObj).Where("id", "=", 1).ExecuteCommand();
            var t12 = db.Updateable<School>().AS("Student").UpdateColumns(it => new School() { Name = "jack" }).Where(it => it.Id == 1).ExecuteCommandAsync();
            t12.Wait();

            //update one columns
            var count = db.Updateable<Student>().UpdateColumns(it => it.SchoolId == it.SchoolId).Where(it => it.Id == it.Id + 1).ExecuteCommand();

            var count1 = db.Updateable<Student>()
                .UpdateColumnsIF(false, it => it.SchoolId == it.SchoolId)//ignore
                .UpdateColumnsIF(true, it => it.SchoolId == 2).//ok
                Where(it => it.Id == it.Id + 1).ExecuteCommand();


            //update one columns
            var count2 = db.Updateable<Student>().UpdateColumns(it => it.SchoolId == it.SchoolId + 1).Where(it => it.Id == it.Id + 1).ExecuteCommand();

            var dt = new Dictionary<string, object>();
            dt.Add("id", 1);
            dt.Add("name", null);
            dt.Add("createTime", DateTime.Now);
            var t66 = db.Updateable(dt).AS("student").WhereColumns("id").With(SqlWith.UpdLock).ExecuteCommand();

            db.Updateable(new Student() { Id = 1 }).UpdateColumns(new string[] { "name" }).Where(it => it.Name == "1").ExecuteCommand();

            var dt2 = new Dictionary<string, object>();
            dt2.Add("id", 2);
            dt2.Add("name", null);
            dt2.Add("createTime", DateTime.Now);
            var dtList = new List<Dictionary<string, object>>();
            dtList.Add(dt);
            dtList.Add(dt2);
            var t666 = db.Updateable(dtList).AS("student").WhereColumns("id").With(SqlWith.UpdLock).ExecuteCommand();


            var t20 = db.Updateable<Student>().UpdateColumns(p => new Student()
            {
                SchoolId = SqlFunc.IIF(p.Id == 1, 2, 3)
            }).Where(p => p.Id == 10000).ExecuteCommand();
            var t21 = db.Updateable<Student>().UpdateColumns(p => new Student()
            {
                SchoolId = SqlFunc.IF(p.Id == 1).Return(1).End(p.Id)
            }).Where(p => p.Id == 10000).ExecuteCommand();


            var t22 = db.Updateable<Student>().UpdateColumns(p => new Student()
            {
                SchoolId = SqlFunc.Subqueryable<Student>().Where(s => s.SchoolId == p.Id).Select(s => s.Id)
            }).Where(p => p.Id == 10000).ExecuteCommand();


            var t23 = db.Updateable<Student>(new Student() { })
                .Where(p => p.SchoolId == SqlFunc.Subqueryable<Student>().Where(s => s.SchoolId == p.Id).Select(s => s.Id)).ExecuteCommand();

            var t24 = db.Updateable(new Student() { }).WhereColumns(it => it.CreateTime).ExecuteCommand();

            var t25 = db.Updateable(new Student() { }).UpdateColumns(it => new { it.Name, it.CreateTime }).WhereColumns(it => it.CreateTime).ExecuteCommand();

            var t26 = db.Updateable(new List<Student>() { new Student() { }, new Student() { } }).UpdateColumns(it => new { it.Name, it.CreateTime }).WhereColumns(it => it.CreateTime).ExecuteCommand();


            db.Updateable<Student>().UpdateColumns(it => new Student { SchoolId = GeneratePassword(2, 1), Name = SqlFunc.ToString(it.Name), CreateTime = DateTime.Now.AddDays(1) }).Where(it => it.Id == 1).ExecuteCommand();
            db.Updateable(new Student[] { new Student() { Id = 2, Name = "a2" }, new Student() { Id = 1, Name = "a1" } })
                   .UpdateColumns(it => new { it.Name, it.Id, it.SchoolId })
                   .WhereColumns(it => it.Name)
                  .Where(it => it.Id == 1)
                  .ExecuteCommand();
        }

        private static int GeneratePassword(int v1, int v2)
        {
            return 1;
        }
    }

}
