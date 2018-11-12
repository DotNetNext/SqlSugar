using OrmTest.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest.Demo
{
    public class Aop
    {

        public static void Init()
        {
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig() { ConnectionString = Config.ConnectionString, DbType = DbType.SqlServer, IsAutoCloseConnection = true });

            
            db.Aop.OnLogExecuted = (sql, pars) =>
            {
                Console.Write("time:" + db.Ado.SqlExecutionTime.ToString());
            };
            db.Aop.OnLogExecuting = (sql, pars) =>
            {

            };
            db.Aop.OnError = (exp) =>
            {

            };
            db.Aop.OnExecutingChangeSql = (sql, pars) =>
            {
                return new KeyValuePair<string, SugarParameter[]>(sql, pars);
            };

            db.Queryable<CMStudent>().ToList();

            try
            {
                db.Queryable<CMStudent>().AS(" ' ").ToList();
            }
            catch (Exception)
            {


            }


            //diff log demo

            db.Aop.OnDiffLogEvent = it =>
            {
                var editBeforeData = it.BeforeData;
                var editAfterData = it.AfterDate;
                var sql = it.Sql;
                var parameter = it.Parameters;
                var data = it.BusinessData;
            };


            var id = db.Queryable<Student>().First().Id;
            db.Updateable<Student>(new Student()
            {
                Id = id,
                CreateTime = DateTime.Now,
                Name = "before",
                SchoolId = 1
            })
            .EnableDiffLogEvent(new { title = "update Student", Modular = 1, Operator = "admin" }).ExecuteCommand();


            db.Updateable<Student>(new Student()
            {
                Id = id,
                CreateTime = DateTime.Now,
                Name = "after",
                SchoolId = 2
            })
            .EnableDiffLogEvent(new { title= "update Student", Modular=1, Operator="admin" })
            .ExecuteCommand();
        }

    }
}