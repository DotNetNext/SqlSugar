using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public  class Demo8_Saveable
    {
        public static void Init()
        {
            Console.WriteLine("");
            Console.WriteLine("#### Saveable Start ####");

            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                DbType = DbType.Access,
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


            //insert or update
            var x= db.Storageable<Order>(new Order() { Id=159, Name="jack" }).ToStorage();
            var updateRow=x.AsUpdateable.ExecuteCommand();
            x.AsInsertable.ExecuteCommand();

            var x2 = db.Storageable<Order>(new Order() { Id = 0, Name = "jack" }).ToStorage();
            var updateRow2 = x2.AsUpdateable.ExecuteCommand();
            x2.AsInsertable.ExecuteCommand();


           // db.Saveable(new Order() { Id = 159, Name = "jack" }).ExecuteCommand();
            Console.WriteLine("");
            Console.WriteLine("#### Saveable End ####");
        }
    }
}
