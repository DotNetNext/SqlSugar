using OrmTest.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest.Demo
{
    public class Delete
    {
        public static void Init()
        {
            var db = GetInstance();
            //by entity
            var t1 = db.Deleteable<Student>().Where(new Student() { Id = 1 }).ExecuteCommand();

            //use lock
            var t2 = db.Deleteable<Student>().With(SqlWith.RowLock).ExecuteCommand();


            //by primary key
            var t3 = db.Deleteable<Student>().In(1).ExecuteCommand();

            //by primary key array
            var t4 = db.Deleteable<Student>().In(new int[] { 1, 2 }).ExecuteCommand();

            //by expression
            var t5 = db.Deleteable<Student>().Where(it => it.Id == 1).ExecuteCommand();
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
