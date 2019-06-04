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
         
            for (int i = 0; i < 10; i++)
            {
                var db = GetMasterSlaveInstance();
                var list = db.Insertable(new Student() { Name="aa" }).ExecuteCommand(); // ConnectionString2 or ConnectionString3
            }
            //db.Insertable(new Student() { Name = "masterTest" }).ExecuteCommand();// Config.ConnectionString
        }

        public static SqlSugarClient GetMasterSlaveInstance()
        {
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = Config.ConnectionString,
                DbType = DbType.PostgreSQL,
                IsAutoCloseConnection = true,
                SlaveConnectionConfigs = new List<SlaveConnectionConfig>() {
                     new SlaveConnectionConfig() { HitRate=10, ConnectionString=Config.ConnectionString2 } ,
                       new SlaveConnectionConfig() { HitRate=10, ConnectionString=Config.ConnectionString2 }
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
