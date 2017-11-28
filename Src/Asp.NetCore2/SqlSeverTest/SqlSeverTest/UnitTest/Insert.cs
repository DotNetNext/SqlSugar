using OrmTest.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest.UnitTest
{
    public class Insert : UnitTestBase
    {
        private Insert() { }
        public Insert(int eachCount)
        {
            this.Count = eachCount;
        }

        public void Init()
        {
            var db = GetInstance();
            var insertObj = new Student() { Name = "jack", CreateTime = Convert.ToDateTime("2010-1-1"), SchoolId=0 };
            db.IgnoreColumns.Add("TestId", "Student");
            //db.MappingColumns.Add("id","dbid", "Student");

            var t1 = db.Insertable(insertObj).ToSql();
            base.Check(@"INSERT INTO [STudent]  
           ([SchoolId],[Name],[CreateTime])
     VALUES
           (@SchoolId,@Name,@CreateTime) ;SELECT SCOPE_IDENTITY();",
           new List<SugarParameter>() {
               new SugarParameter("@SchoolId",0),
               new SugarParameter("@CreateTime",Convert.ToDateTime("2010-1-1")),
               new SugarParameter("@Name","jack")
           }, t1.Key, t1.Value, "Insert t1 error"
           );


            //Insert reutrn Command Count
            var t2 = db.Insertable(insertObj).ExecuteReturnEntity();

            db.IgnoreColumns = null;
            //Only  insert  Name 
            var t3 = db.Insertable(insertObj).InsertColumns(it => new { it.Name }).ToSql();
            base.Check(@"INSERT INTO [STudent]  
           ([Name])
     VALUES
           (@Name) ;SELECT SCOPE_IDENTITY();", new List<SugarParameter>() {
                           new SugarParameter("@Name","jack")
            }, t3.Key, t3.Value, "Insert t3 error");


            //Ignore  Name and TestId
            var t4 = db.Insertable(insertObj).IgnoreColumns(it => new { it.Name, it.TestId }).ToSql();
            base.Check(@"INSERT INTO [STudent]  
           ([SchoolId],[CreateTime])
     VALUES
           (@SchoolId,@CreateTime) ;SELECT SCOPE_IDENTITY();",
      new List<SugarParameter>() {
               new SugarParameter("@SchoolId",0),
               new SugarParameter("@CreateTime",Convert.ToDateTime("2010-1-1")),
      }, t4.Key, t4.Value, "Insert t4 error"
      );

            //Ignore  Name and TestId
            var t5 = db.Insertable(insertObj).IgnoreColumns(it => it == "Name" || it == "TestId").With(SqlWith.UpdLock).ToSql();
            base.Check(@"INSERT INTO [STudent] WITH(UPDLOCK)  
           ([SchoolId],[CreateTime])
     VALUES
           (@SchoolId,@CreateTime) ;SELECT SCOPE_IDENTITY();",
new List<SugarParameter>() {
               new SugarParameter("@SchoolId",0),
               new SugarParameter("@CreateTime",Convert.ToDateTime("2010-1-1")),
}, t5.Key, t5.Value, "Insert t5 error"
);
            //Use Lock
            var t6 = db.Insertable(insertObj).With(SqlWith.UpdLock).ToSql();
            base.Check(@"INSERT INTO [STudent] WITH(UPDLOCK)  
           ([SchoolId],[Name],[CreateTime])
     VALUES
           (@SchoolId,@Name,@CreateTime) ;SELECT SCOPE_IDENTITY();",
new List<SugarParameter>() {
               new SugarParameter("@SchoolId",0),
               new SugarParameter("@CreateTime",Convert.ToDateTime("2010-1-1")),
               new SugarParameter("@Name","jack")
}, t6.Key, t6.Value, "Insert t6 error"
);

            var insertObj2 = new Student() { Name = null,SchoolId=0, CreateTime = Convert.ToDateTime("2010-1-1") };
            var t8 = db.Insertable(insertObj2).Where(true/* Is insert null */, true/*off identity*/).ToSql();
            base.Check(@"INSERT INTO [STudent]  
           ([ID],[SchoolId],[CreateTime])
     VALUES
           (@ID,@SchoolId,@CreateTime) ;SELECT SCOPE_IDENTITY();",
               new List<SugarParameter>() {
               new SugarParameter("@SchoolId", 0),
               new SugarParameter("@ID", 0),
               new SugarParameter("@CreateTime", Convert.ToDateTime("2010-1-1"))
               },
               t8.Key,
               t8.Value,
               "Insert t8 error"
           );


            db.IgnoreColumns = new IgnoreColumnList();
            db.IgnoreColumns.Add("TestId", "Student");

            //Insert List<T>
            var insertObjs = new List<Student>();
            for (int i = 0; i < 1000; i++)
            {
                insertObjs.Add(new Student() { Name = "name" + i });
            }
            var s9 = db.Insertable(insertObjs.ToArray()).InsertColumns(it => new { it.Name }).With(SqlWith.UpdLock).ToSql();

            insertObj.Name = null;
            var t10 = db.Insertable(insertObj).ExecuteCommand();

            var t11 = db.Insertable(new MyStudent() { Id = 1, Name = "张三" }).AS("Student").ToSql();
            base.Check(@"INSERT INTO [Student]  
           ([Name])
     VALUES
           (@Name) ;SELECT SCOPE_IDENTITY();", new List<SugarParameter>() {
                           new SugarParameter("@Name","张三")
            }, t11.Key, t11.Value, "Insert t11 error");


            var t12 = db.Insertable<Student>(new { Name = "a" }).ToSql();
            base.Check(@"INSERT INTO [STudent]  
           ([Name])
     VALUES
           (@Name) ;SELECT SCOPE_IDENTITY();", new List<SugarParameter>() {
                           new SugarParameter("@Name","a")
            }, t12.Key, t12.Value, "Insert t12 error");

            var t13 = db.Insertable<Student>(new Dictionary<string, object>() { {"id",0 },{ "name","2"} }).ToSql();
            base.Check(@"INSERT INTO [STudent]  
           ([Name])
     VALUES
           (@Name) ;SELECT SCOPE_IDENTITY();", new List<SugarParameter>() {
                           new SugarParameter("@Name","2")
            }, t13.Key, t13.Value, "Insert t13 error");
        }
    }

    public class MyStudent {

        public int Id { get; set; }
        public string Name { get; set; }
    }
}
