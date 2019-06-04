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
                DbType = DbType.Oracle,
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
            db.Aop.OnDiffLogEvent = it =>//Get data changes
            {
                var editBeforeData = it.BeforeData;
                var editAfterData = it.AfterData;
                var sql = it.Sql;
                var parameter = it.Parameters;
                var businessData = it.BusinessData;
                var time = it.Time;
                var diffType = it.DiffType;//enum insert 、update and delete  
                Console.WriteLine(businessData);
                Console.WriteLine(editBeforeData[0].Columns[1].Value);
                Console.WriteLine("to");
                Console.WriteLine(editAfterData[0].Columns[1].Value);
                //Write logic
            };

     
            db.Queryable<Order>().ToList();
            db.Queryable<OrderItem>().ToList();

            //OnDiffLogEvent
            var data = db.Queryable<Order>().First();
            data.Name = "changeName";
            db.Updateable(data).EnableDiffLogEvent("--update Order--").ExecuteCommand();

            Console.WriteLine("#### Aop End ####");
        }
    }
}
