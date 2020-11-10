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
                DbType = DbType.Oracle,
                IsAutoCloseConnection = true,
           
                InitKeyType = InitKeyType.Attribute,
            });
            //调式代码 用来打印SQL 
            Db.Aop.OnLogExecuting = (sql, pars) =>
            {
               Console.WriteLine(sql);
            };

             var id=Enumerable.Range(0, 100).ToList();
         //   id = null;
            Db.Queryable<Order>().Where(it => id.Contains(it.Id)).ToList();
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
