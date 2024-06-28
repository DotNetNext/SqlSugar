using Data.Model;
using Microsoft.IdentityModel.Tokens;
using SqlSugar;
using SqlSugar.DbConvert;
using SqlSugar.Xugu;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using XuguClient;

namespace XuguTest
{
    /// <summary>
    /// Fork to ：https://github.com/dreamsfly900/Weave.XuguCore/tree/master/Xugu.Sqlsugar
    /// </summary>
    internal class Program
    {
        static void Main(string[] args)
        {

            //注册DLL写在程序启动时
            InstanceFactory.CustomAssemblies = new System.Reflection.Assembly[] {
                 typeof(XuguProvider).Assembly
            };



            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = "IP=118.123.17.3;DB=HOUSE;User=SYSDBA;PWD=SYSDBA;Port=5138;AUTO_COMMIT=on;CHAR_SET=UTF8",//CHAR_SET=GBK
                DbType = SqlSugar.DbType.Xugu,
                IsAutoCloseConnection = true,
                //ConfigureExternalServices = new ConfigureExternalServices() { SqlFuncServices = SqlFuncCustom.Methods }
            },
            db => {
                db.Aop.OnLogExecuting = (sql, pars) =>
                {

                    Console.WriteLine(SqlSugar.UtilMethods.GetNativeSql(sql, pars));
                };
            });

           
            db.CodeFirst.InitTables<MY_USER>();

            db.DbMaintenance.TruncateTable<MY_USER>();

            db.Insertable(new MY_USER()
            {
                C_BIGINT = 1,
                C_BINARY = new byte[] { 1 },
                C_BLOB = new byte[] { 1},
                C_BOOLEAN = true,
                C_CHAR = "A",
                C_CLOB = "A",
                C_DATE = DateTime.Now,
                C_DATETIME = DateTime.Now,
                C_DATETIME_WITH_TIME_ZONE = DateTimeOffset.Now,
                C_DECIMAL = 1.1M,
                C_DOUBLE = 1.1,
                C_FLOAT = 1.1F,
                C_GUID = Guid.NewGuid(),
                C_INT = 1,
                C_INTEGER = 1,
                C_INTERVAL_DAY = "A",
                C_INTERVAL_DAY_TO_HOUR = "A",
                C_INTERVAL_DAY_TO_MINUTE = "A",
                C_INTERVAL_DAY_TO_SECOND = "A",
                C_INTERVAL_HOUR = "A",
                C_INTERVAL_HOUR_TO_MINUTE = "A",
                C_INTERVAL_HOUR_TO_SECOND = "A",
                C_INTERVAL_MINUTE = "A",
                C_INTERVAL_MINUTE_TO_SECOND = "A",
                C_INTERVAL_MONTH = "A",
                C_INTERVAL_SECOND = "A",
                C_INTERVAL_YEAR = "2001",
                C_INTERVAL_YEAR_TO_MONTH = "2",
                C_NCHAR = "A",
                C_NUMERIC = 1.1M,
                C_NVARCHAR = "A",
                C_ROWID = "A", 
                C_TIMESTAMP = DateTime.Now,
                C_TIME = DateTimeOffset.Now.TimeOfDay,
                C_TINYINT = 1,
                C_VARCHAR = "A",
                C_TIMESTAMP_AUTO_UPDATE = DateTime.Now,
                C_TIME_WITH_TIME_ZONE = DateTime.Now.TimeOfDay

            }).ExecuteCommand();

         //   var list0=db.Ado.GetDataTable("select * from MY_USER");

            var list = db.Queryable<MY_USER>().ToList();
            list.ForEach(it =>
            {
                Console.WriteLine(it.C_BIGINT);
                Console.WriteLine(it.C_BINARY);
                Console.WriteLine(it.C_BLOB);
                Console.WriteLine(it.C_BOOLEAN);
                Console.WriteLine(it.C_CHAR);
                Console.WriteLine(it.C_CLOB);
                Console.WriteLine(it.C_DATE);
                Console.WriteLine(it.C_DATETIME);
                Console.WriteLine(it.C_DATETIME_WITH_TIME_ZONE);
                Console.WriteLine(it.C_DECIMAL);
                Console.WriteLine(it.C_DOUBLE);
                Console.WriteLine(it.C_FLOAT);
                Console.WriteLine(it.C_GUID);
                Console.WriteLine(it.C_INT);
                Console.WriteLine(it.C_INTEGER);
                Console.WriteLine(it.C_INTERVAL_DAY);
                Console.WriteLine(it.C_INTERVAL_DAY_TO_HOUR);
                Console.WriteLine(it.C_INTERVAL_DAY_TO_MINUTE);
                Console.WriteLine(it.C_INTERVAL_DAY_TO_SECOND);
                Console.WriteLine(it.C_INTERVAL_HOUR);
                Console.WriteLine(it.C_INTERVAL_HOUR_TO_MINUTE);
                Console.WriteLine(it.C_INTERVAL_HOUR_TO_SECOND);
                Console.WriteLine(it.C_INTERVAL_MINUTE);
                Console.WriteLine(it.C_INTERVAL_MINUTE_TO_SECOND);
                Console.WriteLine(it.C_INTERVAL_MONTH);
                Console.WriteLine(it.C_INTERVAL_SECOND);
                Console.WriteLine(it.C_INTERVAL_YEAR);
                Console.WriteLine(it.C_INTERVAL_YEAR_TO_MONTH);
                Console.WriteLine(it.C_NCHAR);
                Console.WriteLine(it.C_NUMERIC);
                Console.WriteLine(it.C_NVARCHAR);
                Console.WriteLine(it.C_ROWID);
                Console.WriteLine(it.C_TIMESTAMP);
                Console.WriteLine(it.C_TIME);
                Console.WriteLine(it.C_TINYINT);
                Console.WriteLine(it.C_VARCHAR);
                Console.WriteLine(it.C_TIMESTAMP_AUTO_UPDATE);
                Console.WriteLine(it.C_TIME_WITH_TIME_ZONE);
            });
            db.Updateable(list).ExecuteCommand();
            db.Deleteable(list).ExecuteCommand();
            Console.ReadKey();
        }
    }


}
