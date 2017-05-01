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
            db.Insertable<Student>(insertObj).ExecuteCommand();

            //Only  insert  Name 
            db.Insertable<Student>(insertObj).InsertColumns(it => new object[] { it.Name}).ExecuteReutrnIdentity();

            //Ignore  Name and TestId
            db.Insertable<Student>(insertObj).IgnoreColumns(it => new object[] { it.Name,it.TestId }).ExecuteReutrnIdentity();

            //Use Lock
            db.Insertable<Student>(insertObj).With(SqlWith.UpdLock).ExecuteCommand();

            //ToSql
            db.Insertable<Student>(insertObj).With(SqlWith.UpdLock).InsertColumns(it => new object[] { it.Name }).ToSql();

            //Insert List<T>
            db.Insertable<Student>(insertObjs).With(SqlWith.UpdLock).ExecuteCommand();
        }

        public SqlSugarClient GetInstance()
        {
            SqlSugarClient db = new SqlSugarClient(new SystemTablesConfig() { ConnectionString = Config.ConnectionString, DbType = DbType.SqlServer, IsAutoCloseConnection=true });
            return db;
        }
    }
}
