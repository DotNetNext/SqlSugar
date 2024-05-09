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
                ConnectionString = @"PORT=5433;DATABASE=x;HOST=localhost;PASSWORD=haosql;USER ID=postgres",
                DbType = DbType.PostgreSQL,
                IsAutoCloseConnection = true,
           
                InitKeyType = InitKeyType.Attribute,
            });
            //调式代码 用来打印SQL 
            Db.Aop.OnLogExecuting = (sql, pars) =>
            {
               Console.WriteLine(sql);
            };

            Db.CodeFirst.InitTables(typeof(testmmxxxmm121));
            Db.Insertable(new testmmxxxmm121() { name = (float) 0.01 , name2 = 1 }).ExecuteCommand();
            var list= Db.Queryable<testmmxxxmm121>().ToList();
        }
    }

    public class testmmxxxmm121
    {
        [SugarColumn(IsPrimaryKey =true,IsIdentity =true)]
        public int id { get; set; }
        [SugarColumn(ColumnDataType ="float4",IsNullable =true)]
        public float? name { get; set; }
        [SugarColumn(ColumnDataType = "float4", IsNullable = false)]
        public float? name2 { get; set; }

    }

}
