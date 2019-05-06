using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest
{
    public class DemoD_DbFirst
    {
        public static void Init()
        {
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                DbType = DbType.SqlServer,
                ConnectionString = Config.ConnectionString,
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true,
                AopEvents = new AopEvents
                {
                    OnLogExecuting = (sql, p) =>
                    {
                        Console.WriteLine(sql);
                    }
                }
            });
            db.DbFirst.CreateClassFile("c:\\Demo\\1", "Models");


            db.DbFirst.Where("Student").CreateClassFile("c:\\Demo\\2", "Models");


            db.DbFirst.Where(it => it.ToLower().StartsWith("view")).CreateClassFile("c:\\Demo\\3");

            db.DbFirst.Where(it => it.ToLower().StartsWith("view")).CreateClassFile("c:\\Demo\\4");


        }
    }
}
