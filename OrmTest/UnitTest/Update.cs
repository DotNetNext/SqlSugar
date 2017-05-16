using OrmTest.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest.UnitTest
{
    public class Update:ExpTestBase
    {
        private Update() { }
        public Update(int eachCount)
        {
            this.Count = eachCount;
        }

        public void Init()
        {
            var db = GetInstance();
            var updateObj = new Student() { Id=1,Name = "jack", CreateTime = DateTime.Now };
            var updateObjs = new List<Student>() { updateObj }.ToArray();
            db.IgnoreColumns.Add("TestId", "Student");
            //db.MappingColumns.Add("id","dbid", "Student");

            var s1 = db.Updateable(updateObj).ToSql();

            //Insert reutrn Command Count
            var s2 = db.Updateable(updateObj).ExecuteCommand();

            db.IgnoreColumns = null;
            //Only  insert  Name 
            var s3 = db.Updateable(updateObj).UpdateColumns(it => new { it.Name}).ToSql();

            //Ignore  Name and TestId
            var s4 = db.Updateable(updateObj).IgnoreColumns(it => new { it.Name, it.TestId }).ToSql();

            //Ignore  Name and TestId
            var s5 = db.Updateable(updateObj).IgnoreColumns(it => it == "Name" || it == "TestId").With(SqlWith.UpdLock).ToSql();

            //Use Lock
            var s6 = db.Updateable(updateObj).With(SqlWith.UpdLock).ToSql();

            //ToSql
            var s7 = db.Insertable(updateObj).With(SqlWith.UpdLock)
                .InsertColumns(it => new { it.Name }).ToSql();

            //Insert List<T>
            var s8 = db.Updateable(updateObj).With(SqlWith.UpdLock).ToSql();
        }

        public SqlSugarClient GetInstance()
        {
            SqlSugarClient db = new SqlSugarClient(new SystemTablesConfig() { ConnectionString = Config.ConnectionString, DbType = DbType.SqlServer, IsAutoCloseConnection = true });
            return db;
        }
    }
}
