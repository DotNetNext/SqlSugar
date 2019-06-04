using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlSugar;
using OrmTest.Models;

namespace OrmTest.Demo
{
    public class ExtSqlFun : DemoBase
    {
        public static SqlSugarClient GetDb()
        {
            //Create ext method
            var expMethods = new List<SqlFuncExternal>();
            expMethods.Add(new SqlFuncExternal()
            {
                UniqueMethodName = "MyToString",
                MethodValue = (expInfo, dbType, expContext) =>
                {
                    return string.Format("CAST({0} AS VARCHAR)", expInfo.Args[0].MemberName);
                }
            });

            var config = new ConnectionConfig()
            {
                ConnectionString = Config.ConnectionString,
                DbType = DbType.PostgreSQL,
                IsAutoCloseConnection = true,
                ConfigureExternalServices = new ConfigureExternalServices()
                {
                    SqlFuncServices = expMethods//set ext method
                }
            };

            SqlSugarClient db = new SqlSugarClient(config);
            return db;
        }

        public static string MyToString<T>(T str)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        public static void Init()
        {
            var db = GetDb();
            var list = db.Queryable<Student>().Where(it => MyToString(it.Id) == "1302583").ToList();
            var sql = db.Queryable<Student>().Where(it => MyToString(it.Id) == "1302583").ToSql();
            Console.WriteLine(sql);
        }
    }
}
