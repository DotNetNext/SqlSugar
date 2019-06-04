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
                DbType = DbType.MySql,
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
            db.Saveable<Order>(new Order() { Id=1, Name="jack" }).ExecuteReturnEntity();


            //insert or update
            db.Saveable<Order>(new Order() { Id = 1000, Name = "jack", CreateTime=DateTime.Now })
                  .InsertColumns(it => new { it.Name,it.CreateTime, it.Price})//if insert  into name,CreateTime,Price
                  .UpdateColumns(it => new { it.Name, it.CreateTime })//if update set name CreateTime
                  .ExecuteReturnEntity();

            Console.WriteLine("");
            Console.WriteLine("#### Saveable End ####");
        }
    }
}
