using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest.Test
{
    public class BugTest1
    {
        public static void Init()
        {
            SqlSugarClient Db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = Config.ConnectionString,
                DbType = DbType.Dm,
                IsAutoCloseConnection = true,
           
                InitKeyType = InitKeyType.Attribute,
            });
            //调式代码 用来打印SQL 
            Db.Aop.OnLogExecuting = (sql, pars) =>
            {
               Console.WriteLine(sql);
            };

            Db.CodeFirst.InitTables(typeof(testmmxxxmm121));
            var id = Guid.NewGuid();
            Db.Insertable(new testmmxxxmm121() { x = id }).ExecuteCommand();
            var x= Db.Queryable<testmmxxxmm121>().Where(it => it.x.ToString().ToUpper() == id.ToString().ToUpper()).ToList();
        }
    }

    public class testmmxxxmm121
    {
        [SugarColumn(IsPrimaryKey =true,IsIdentity =true)]
        public int id { get; set; }
 [SugarColumn(ColumnDataType ="varchar(36)")]
        public Guid x { get; set; }

    }

}
