using OrmTest.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest.Demo
{
    public class Update
    {
        public static void Init() {
            var db = GetInstance();
            var updateObj = new Student() { Id = 1, Name = "jack", SchoolId = 0, CreateTime = Convert.ToDateTime("2017-05-21 09:56:12.610") };
            var updateObjs = new List<Student>() { updateObj, new Student() { Id = 2, Name = "sun", SchoolId = 0 } }.ToArray();
            db.IgnoreColumns.Add("TestId", "Student");
            //db.MappingColumns.Add("id","dbid", "Student");


            //update reutrn Update Count
            var t1= db.Updateable(updateObj).ExecuteCommand();

            //Only  update  Name 
            var t3 = db.Updateable(updateObj).UpdateColumns(it => new { it.Name }).ExecuteCommand();


            //Ignore  Name and TestId
            var t4 = db.Updateable(updateObj).IgnoreColumns(it => new { it.Name, it.TestId }).ExecuteCommand();

            //Ignore  Name and TestId
            var t5 = db.Updateable(updateObj).IgnoreColumns(it => it == "Name" || it == "TestId").With(SqlWith.UpdLock).ExecuteCommand();


            //Use Lock
            var t6 = db.Updateable(updateObj).With(SqlWith.UpdLock).ExecuteCommand();

            //update List<T>
            var t7 = db.Updateable(updateObjs).ExecuteCommand();

            //Re Set Value
            var t8 = db.Updateable(updateObj)
                .ReSetValue(it => it.Name == (it.Name + 1)).ExecuteCommand();

            //Where By Expression
            var t9 = db.Updateable(updateObj).Where(it => it.Id == 1).ExecuteCommand();

            //Where
            var t10= db.Updateable<Student>()
                .UpdateColumns(it => new Student() { Name = "jack", SchoolId =1 })
                .Where(it => it.Id == 11).ExecuteCommand();
        }
        public static SqlSugarClient GetInstance()
        {
            SqlSugarClient db = new SqlSugarClient(new SystemTableConfig() { ConnectionString = Config.ConnectionString, DbType = DbType.SqlServer, IsAutoCloseConnection = true });
            db.Ado.IsEnableLogEvent = true;
            db.Ado.LogEventStarting = (sql, pars) =>
            {
                Console.WriteLine(sql + "\r\n" + db.RewritableMethods.SerializeObject(pars));
                Console.WriteLine();
            };
            return db;
        }
    }
}
