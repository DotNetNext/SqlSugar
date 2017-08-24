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
            base.Check(@"INSERT INTO `STudent`  
           (`SchoolId`,`Name`,`CreateTime`)
     VALUES
           (@SchoolId,@Name,@CreateTime) ;SELECT LAST_INSERT_ID();",
           new List<SugarParameter>() {
               new SugarParameter("@SchoolId",0),
               new SugarParameter("@CreateTime",Convert.ToDateTime("2010-1-1")),
               new SugarParameter("@Name","jack")
           }, t1.Key, t1.Value, "Insert t1 error"
           );


            //Insert reutrn Command Count
            var t2 = db.Insertable(insertObj).ExecuteCommand();

            db.IgnoreColumns = null;
            //Only  insert  Name 
            var t3 = db.Insertable(insertObj).InsertColumns(it => new { it.Name }).ToSql();
            base.Check(@"INSERT INTO `STudent`  
           (`Name`)
     VALUES
           (@Name) ;SELECT LAST_INSERT_ID();", new List<SugarParameter>() {
                           new SugarParameter("@Name","jack")
            }, t3.Key, t3.Value, "Insert t3 error");


            //Ignore  Name and TestId
            var t4 = db.Insertable(insertObj).IgnoreColumns(it => new { it.Name, it.TestId }).ToSql();
            base.Check(@"INSERT INTO `STudent`  
           (`SchoolId`,`CreateTime`)
     VALUES
           (@SchoolId,@CreateTime) ;SELECT LAST_INSERT_ID();",
      new List<SugarParameter>() {
               new SugarParameter("@SchoolId",0),
               new SugarParameter("@CreateTime",Convert.ToDateTime("2010-1-1")),
      }, t4.Key, t4.Value, "Insert t4 error"
      );

            //Ignore  Name and TestId
            var t5 = db.Insertable(insertObj).IgnoreColumns(it => it == "Name" || it == "TestId").With(SqlWith.UpdLock).ToSql();
            base.Check(@"INSERT INTO `STudent`  
           (`SchoolId`,`CreateTime`)
     VALUES
           (@SchoolId,@CreateTime) ;SELECT LAST_INSERT_ID();",
new List<SugarParameter>() {
               new SugarParameter("@SchoolId",0),
               new SugarParameter("@CreateTime",Convert.ToDateTime("2010-1-1")),
}, t5.Key, t5.Value, "Insert t5 error"
);
            //Use Lock
            var t6 = db.Insertable(insertObj).With(SqlWith.UpdLock).ToSql();
            base.Check(@"INSERT INTO `STudent`  
           (`SchoolId`,`Name`,`CreateTime`)
     VALUES
           (@SchoolId,@Name,@CreateTime) ;SELECT LAST_INSERT_ID();",
new List<SugarParameter>() {
               new SugarParameter("@SchoolId",0),
               new SugarParameter("@CreateTime",Convert.ToDateTime("2010-1-1")),
               new SugarParameter("@Name","jack")
}, t6.Key, t6.Value, "Insert t6 error"
);

            var insertObj2 = new Student() { Name = null,SchoolId=0, CreateTime = Convert.ToDateTime("2010-1-1") };
            var t8 = db.Insertable(insertObj2).Where(true/* Is insert null */, true/*off identity*/).ToSql();
            base.Check(@"INSERT INTO `STudent`  
           (`ID`,`SchoolId`,`CreateTime`)
     VALUES
           (@ID,@SchoolId,@CreateTime) ;SELECT LAST_INSERT_ID();",
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
        }

        public SqlSugarClient GetInstance()
        {
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig() { ConnectionString = Config.ConnectionString, DbType = DbType.MySql, IsAutoCloseConnection = true });
            return db;
        }
    }
}
