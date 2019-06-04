using OrmTest.Demo;
using OrmTest.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest.Demo
{
    public class MasterSlave : DemoBase
    {

        public static void Init()
        {
            var db = GetMasterSlaveInstance();
            for (int i = 0; i < 10; i++)
            {
                var list = db.Queryable<Student>().ToList(); // ConnectionString2 or ConnectionString3
            }
            db.Insertable(new Student() { Name = "masterTest" }).ExecuteCommand();// Config.ConnectionString
        }

        public static SqlSugarClient GetMasterSlaveInstance()
        {
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = Config.ConnectionString,
                DbType = DbType.MySql,
                IsAutoCloseConnection = true,
                SlaveConnectionConfigs = new List<SlaveConnectionConfig>() {
                     new SlaveConnectionConfig() { HitRate=0, ConnectionString=Config.ConnectionString2 },
                     new SlaveConnectionConfig() { HitRate=30, ConnectionString=Config.ConnectionString3 }
                }
            });
            db.Aop.OnLogExecuting = (sql, pars) =>
            {
                Console.WriteLine(db.Ado.Connection.ConnectionString);
            };
            return db;
        }
    }
}
