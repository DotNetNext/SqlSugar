using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest.Test
{
    public class BugTest4
    {
        public static void Init()
        {
            SqlSugarClient Db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = Config.ConnectionString,
                DbType = DbType.SqlServer,
                IsAutoCloseConnection = true,
                //MoreSettings = new ConnMoreSettings()
                //{
                //    PgSqlIsAutoToLower = true //我们这里需要设置为false
                //},
                InitKeyType = InitKeyType.Attribute,
            });
            //调式代码 用来打印SQL 
            Db.Aop.OnLogExecuting = (sql, pars) =>
            {
                Console.WriteLine(sql);
            };

            Db.CodeFirst.InitTables(typeof(Testmmm));
            Db.Insertable(new Testmmm() { b = "a" }).InsertColumns(it => new { it.b}).ExecuteCommand();
            var list = Db.Queryable<Testmmm>().ToList();
        }
        public class Testmmm {
            [SugarColumn(IsIdentity =true,IsPrimaryKey =true)]
            public int id { get; set; }
            [SugarColumn(ColumnDataType ="nvarchar",Length =8, DefaultValue ="222", ColumnDescription ="aa")]
             public string a { get; set; }
             public string b { get; set; }
        }
    }
   

}
