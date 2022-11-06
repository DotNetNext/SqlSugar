using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class Demo3_Insertable
    {
        public static void Init()
        {
            Console.WriteLine("");
            Console.WriteLine("#### Insertable Start ####");

            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                DbType = DbType.ClickHouse,
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

            var insertObj = new Order() { Id = 1, Name = "order1",Price=0 };
            var updateObjs = new List<Order> {
                 new Order() { Id = 11, Name = "order11", Price=0 },
                 new Order() { Id = 12, Name = "order12" , Price=0}
            };

            //Ignore  CreateTime
            db.Insertable(insertObj).IgnoreColumns(it => new { it.CreateTime }).ExecuteReturnSnowflakeId();//get identity
            db.Insertable(insertObj).IgnoreColumns("CreateTime").ExecuteReturnSnowflakeId();

            //Only  insert  Name and Price
            db.Insertable(insertObj).InsertColumns(it => new { it.Name, it.Price }).ExecuteReturnSnowflakeId();
            db.Insertable(insertObj).InsertColumns("Name", "Price").ExecuteReturnSnowflakeId();

            //ignore null columns
            db.Insertable(updateObjs).ExecuteReturnSnowflakeId();//get change row count

            //Use Lock
            db.Insertable(insertObj).With(SqlWith.UpdLock).ExecuteReturnSnowflakeId();

            var updateObjs2 = new List<Order> {
                 new Order() { Id = 222221, Name = "bk", Price=10 },
                 new Order() { Id = 222221, Name = "bk2" , Price= 11}
            };
            db.Fastest<Order>().BulkCopy(updateObjs2);

            var list3 = db.Queryable<Order>().Where(it => it.Id == updateObjs2[0].Id).ToList();
      
            Console.WriteLine("#### Insertable End ####");
        }
    }
}
