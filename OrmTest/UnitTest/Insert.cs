using OrmTest.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest.UnitTest
{
    public class Insert : ExpTestBase
    {
        private Insert() { }
        public Insert(int eachCount)
        {
            this.Count = eachCount;
        }

        public void Init() {
            var db = GetInstance();
            var insertObj = new Student() { Name="jack",CreateTime=DateTime.Now };
            var insertObjs = new List<Student>() { insertObj }.ToArray();
            db.IgnoreColumns.Add("TestId", "Student");
            //db.MappingColumns.Add("id","dbid", "Student");
           
            var s1= db.Insertable<Student>(insertObj).ToSql();

            //Insert reutrn Command Count
            var s2=db.Insertable<Student>(insertObj).ExecuteCommand();

            db.IgnoreColumns = null;
            //Only  insert  Name 
            var s3 = db.Insertable<Student>(insertObj).InsertColumns(it => new object[] { it.Name}).ToSql();

            //Ignore  Name and TestId
            var s4=db.Insertable<Student>(insertObj).IgnoreColumns(it => new object[] { it.Name,it.TestId }).ToSql();

            //Ignore  Name and TestId
            var s5 = db.Insertable<Student>(insertObj).IgnoreColumns(it => it == "Name" || it == "TestId").With(SqlWith.UpdLock).ToSql();

            //Use Lock
            var s6 =db.Insertable<Student>(insertObj).With(SqlWith.UpdLock).ToSql();

            //ToSql
            var s7= db.Insertable<Student>(insertObj).With(SqlWith.UpdLock)
                .InsertColumns(it => new object[] { it.Name }).ToSql();

            //Insert List<T>
            var s8= db.Insertable<Student>(insertObjs).With(SqlWith.UpdLock).ToSql();
        }

        public SqlSugarClient GetInstance()
        {
            SqlSugarClient db = new SqlSugarClient(new SystemTablesConfig() { ConnectionString = Config.ConnectionString, DbType = DbType.SqlServer, IsAutoCloseConnection=true });
            return db;
        }
    }
}
