using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest.Test
{
    public class BugTest
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

            var list2 = Db.Queryable<Order, OrderItem, Custom>((o, i, c) => new JoinQueryInfos(
              JoinType.Left, o.Id == i.OrderId,
              JoinType.Left, c.Id == o.CustomId
             ))
            .Select((o,i,c) => new Order
            {
             Id=SqlFunc.IsNull(
             SqlFunc.Subqueryable<Order>().Where(f=>f.CreateTime>Convert.ToDateTime( o.CreateTime.AddDays(-1))  ).Select(f => f.Id)    
             ,-1)    
            }).ToList();
        }
    }
   

}
