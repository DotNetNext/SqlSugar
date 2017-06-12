using OrmTest.Demo;
using OrmTest.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest.Demo
{

    public class MaterSlave : DemoBase
    {
        public static void Init()
        {
            var db = GetMasterSlaveInstance();
            var insertObject = new Student()
            {
                CreateTime = DateTime.Now,
                Name = Guid.NewGuid().ToString(),
                SchoolId = 1
            };

            var id= db.Insertable(insertObject).ExecuteReutrnIdentity();

            var sudent = db.Queryable<Student>().InSingle(id);

            db.Deleteable<Student>().In(id).ExecuteCommand();
        }
        public  static SqlSugarClient GetMasterSlaveInstance()
        {
            SqlSugarClient db = new SqlSugarClient(
                //master Write and transaction operations
                new ConnectionConfig() { ConnectionString = "server=.;uid=sa;pwd=sasa;database=SqlSugar4XTest", DbType = DbType.SqlServer, IsAutoCloseConnection = true },

                //Read operation
                new ConnectionConfig[] {

                   new ConnectionConfig() { ConnectionString = "SERVER=.;UID=SA;PWD=SASA;DATABASE=SQLSUGAR4XTEST", DbType = DbType.SqlServer, IsAutoCloseConnection = true },
                   //Can be more than one
                }

                );
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
