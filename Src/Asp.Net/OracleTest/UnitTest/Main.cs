using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest
{
    public partial class NewUnitTest
    {
       public static  SqlSugarClient Db=> new SqlSugarClient(new ConnectionConfig()
        {
            DbType = DbType.Oracle,
            ConnectionString = Config.ConnectionString,
            InitKeyType = InitKeyType.Attribute,
            IsAutoCloseConnection = true,
            AopEvents = new AopEvents
            {
                OnLogExecuting = (sql, p) =>
                {
                    Console.WriteLine(sql);
                    Console.WriteLine(string.Join(",", p?.Select(it => it.ParameterName + ":" + it.Value)));
                }
            }
        });

        public static void RestData()
        {
            Db.DbMaintenance.TruncateTable<Order>();
            Db.DbMaintenance.TruncateTable<OrderItem>();
            Db.DbMaintenance.TruncateTable<A>();
            Db.DbMaintenance.TruncateTable<B>();
            Db.DbMaintenance.TruncateTable<ABMapping>();
        }
        public static void Init()
        {
            CodeFirst();
            Updateable();
            Json();
            Ado();
            Queryable();
            QueryableAsync();
            //Thread();
            //Thread2();
            //Thread3();
        }
    }
}
