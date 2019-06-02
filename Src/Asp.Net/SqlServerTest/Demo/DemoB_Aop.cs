using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class DemoB_Aop
    {
        public static void Init()
        {
            Console.WriteLine("");
            Console.WriteLine("#### Aop Start ####");

            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                DbType = DbType.SqlServer,
                ConnectionString = Config.ConnectionString,
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true
            });
            db.Aop.OnLogExecuted = (sql, pars) => //SQL executed event
            {
                Console.WriteLine("OnLogExecuted"+sql);
            };
            db.Aop.OnLogExecuting = (sql, pars) => //SQL executing event (pre-execution)
            {
                Console.WriteLine("OnLogExecuting" + sql);
            };
            db.Aop.OnError = (exp) =>//SQL execution error event
            {
                //exp.sql             
            };
            db.Aop.OnExecutingChangeSql = (sql, pars) => //SQL executing event (pre-execution,SQL script can be modified)
            {
                return new KeyValuePair<string, SugarParameter[]>(sql, pars);
            };



            db.Queryable<Order>().ToList();
            db.Queryable<OrderItem>().ToList();

            Console.WriteLine("#### Aop End ####");
        }
    }
}
