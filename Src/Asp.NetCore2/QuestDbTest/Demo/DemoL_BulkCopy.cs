using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    internal class DemoL_BulkCopy
    {
        public static void Init() 
        {
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                DbType = DbType.QuestDB,
                ConnectionString = Config.ConnectionString3,
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true
            });

            var list=db.Queryable<Order>().Take(2).ToList();
            var i=db.RestApi().BulkCopy(list);
            var i2 = db.RestApi().ExecuteCommand("select 1");
        }
    }
}
