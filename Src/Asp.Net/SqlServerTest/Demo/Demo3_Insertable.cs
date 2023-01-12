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
                DbType = DbType.SqlServer,
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

            var insertObj = new Order() { Id = 1, Name = "order1", Price = 0 };
            var insertObjs = new List<Order> {
                 new Order() { Id = 11, Name = "XX", Price=0 },
                 new Order() { Id = 12, Name = "XX2" , Price=0}
            };

            var x=db.Insertable(insertObjs).RemoveDataCache().IgnoreColumns(it=>it.CreateTime).UseParameter().ExecuteCommand();

            //Ignore  CreateTime
            db.Insertable(insertObj).IgnoreColumns(it => new { it.CreateTime }).ExecuteReturnIdentity();//get identity
            db.Insertable(insertObj).IgnoreColumns("CreateTime").ExecuteReturnIdentity();

            //Only  insert  Name and Price
            db.Insertable(insertObj).InsertColumns(it => new { it.Name, it.Price }).ExecuteReturnIdentity();
            db.Insertable(insertObj).InsertColumns("Name", "Price").ExecuteReturnIdentity();

            //ignore null columns
            db.Insertable(insertObjs).ExecuteCommand();//get change row count

            //Use Lock
            db.Insertable(insertObj).With(SqlWith.UpdLock).ExecuteCommand();

            insertObjs = new List<Order> {
                 new Order() { Id = 11, Name = "order11", Price=1 },
                 new Order() { Id = 12, Name = "order12" , Price=20, CreateTime=DateTime.Now, CustomId=1}
            };
            db.Insertable(insertObjs).UseSqlServer().ExecuteBulkCopy();
            var dt = db.Queryable<Order>().Take(5).ToDataTable();
            dt.TableName = "Order";
            db.Insertable(dt).UseSqlServer().ExecuteBulkCopy();
            db.CodeFirst.InitTables<RootTable0, TwoItem, TwoItem2, TwoItem3>();
            db.CodeFirst.InitTables<ThreeItem2>();
            db.DbMaintenance.TruncateTable("RootTable0");
            db.DbMaintenance.TruncateTable("TwoItem");
            db.DbMaintenance.TruncateTable("TwoItem2");
            db.DbMaintenance.TruncateTable("TwoItem3");
            db.DbMaintenance.TruncateTable("ThreeItem2");
            Console.WriteLine("SubInsert Start");

  
            var dict = new Dictionary<string, object>();
            dict.Add("name", "1");
            dict.Add("CreateTime", DateTime.Now);
            dict.Add("Price", 1);
            db.Insertable(dict).AS("[Order]").ExecuteCommand();

            db.Insertable(new List<Order>()).UseParameter().ExecuteCommand();

            db.Fastest<Order>().BulkCopy(insertObjs);


            var dataTable= db.Queryable<Order>().Select("id,name,Price").Take(2).ToDataTable();
            int result= db.Fastest<System.Data.DataTable>().AS("order").BulkCopy("order", dataTable);
            int result2 = db.Fastest<System.Data.DataTable>().AS("order").BulkCopy( dataTable);

            object o = db.Queryable<Order>().First();
            db.InsertableByObject(o).ExecuteCommand();
            object os = db.Queryable<Order>().Take(2).ToList();
            db.InsertableByObject(os).ExecuteCommand();

            db.CodeFirst.InitTables<City>();
            Console.WriteLine("#### Insertable End ####");

        }

  
    }
}
