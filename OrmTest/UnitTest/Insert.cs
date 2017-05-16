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

        public void Init() {
            var db = GetInstance();
            var insertObj = new Student() { Name="jack",CreateTime=DateTime.Now };
            db.IgnoreColumns.Add("TestId", "Student");
            //db.MappingColumns.Add("id","dbid", "Student");
           
            var s1= db.Insertable(insertObj).ToSql();

            //Insert reutrn Command Count
            var s2=db.Insertable(insertObj).ExecuteCommand();

            db.IgnoreColumns = null;
            //Only  insert  Name 
            var s3 = db.Insertable(insertObj).InsertColumns(it => new {it.Name}).ToSql();

            //Ignore  Name and TestId
            var s4=db.Insertable(insertObj).IgnoreColumns(it => new{ it.Name,it.TestId }).ToSql();

            //Ignore  Name and TestId
            var s5 = db.Insertable(insertObj).IgnoreColumns(it => it == "Name" || it == "TestId").With(SqlWith.UpdLock).ToSql();

            //Use Lock
            var s6 =db.Insertable(insertObj).With(SqlWith.UpdLock).ToSql();

            //ToSql
            var s7= db.Insertable(insertObj).With(SqlWith.UpdLock)
                .InsertColumns(it => new { it.Name }).ToSql();

            db.IgnoreColumns = new IgnoreComumnList();
            db.IgnoreColumns.Add("TestId", "Student");
            //Insert List<T>
            var insertObjs = new List<Student>();
            for (int i = 0; i < 1000; i++)
            {
                insertObjs.Add(new Student() { Name="name"+i });
            }
            var s8= db.Insertable(insertObjs.ToArray()).InsertColumns(it=>new{ it.Name}).With(SqlWith.UpdLock).ToSql();
        }

        public SqlSugarClient GetInstance()
        {
            SqlSugarClient db = new SqlSugarClient(new SystemTablesConfig() { ConnectionString = Config.ConnectionString, DbType = DbType.SqlServer, IsAutoCloseConnection=true });
            return db;
        }
    }
}
