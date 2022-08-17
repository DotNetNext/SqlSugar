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
                DbType = DbType.QuestDB,
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

            var insertObj = new Order() { Id = 1, Name = "order1",Price=0 ,Value = 10.133};
            var updateObjs = new List<Order> {
                 new Order() { Id = SnowFlakeSingle.Instance.NextId(), Name = "order11", Price=0 ,Value = 0.242},
                 new Order() { Id = SnowFlakeSingle.Instance.NextId(), Name = "order12" , Price=0,Value = 3.343}
            };

            var x = db.Insertable(updateObjs).RemoveDataCache().IgnoreColumns(it => it.CreateTime).UseParameter().ExecuteCommand();

            //Ignore  CreateTime
            db.Insertable(insertObj).IgnoreColumns(it => new { it.CreateTime }).ExecuteReturnSnowflakeId();//get identity
            db.Insertable(insertObj).IgnoreColumns("CreateTime").ExecuteReturnSnowflakeId();

            //Only  insert  Name and Price
            db.Insertable(insertObj).InsertColumns(it => new { it.Name, it.Price }).ExecuteReturnSnowflakeId();
            db.Insertable(insertObj).InsertColumns("Name", "Price").ExecuteReturnSnowflakeId();

            foreach (var item in updateObjs)
            {
                item.Id = 0;
            }
            //ignore null columns
            db.Insertable(updateObjs).ExecuteReturnSnowflakeId();//get change row count

            //Use Lock
            db.Insertable(insertObj).With(SqlWith.UpdLock).ExecuteReturnSnowflakeId();

  
            Console.WriteLine("#### Insertable End ####");
        }
    }
}
