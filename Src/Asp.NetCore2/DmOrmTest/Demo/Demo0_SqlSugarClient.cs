using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace OrmTest
{
    public class Demo0_SqlSugarClient
    {

        public static void Init()
        {
            SqlSugarClient();//Create db
            DbContext();//Optimizing SqlSugarClient usage
            SingletonPattern();//Singleten Pattern
            //DistributedTransactionExample();
            MasterSlave();//Read-write separation 
            CustomAttribute(); 
        }

        private static void MasterSlave()
        {
            Console.WriteLine("");
            Console.WriteLine("#### MasterSlave Start ####");
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = Config.ConnectionString,//Master Connection
                DbType = DbType.Dm,
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true,
                SlaveConnectionConfigs = new List<SlaveConnectionConfig>() {
                     new SlaveConnectionConfig() { HitRate=10, ConnectionString=Config.ConnectionString2 } ,
                       new SlaveConnectionConfig() { HitRate=10, ConnectionString=Config.ConnectionString2 }
                } 
            });
            db.Aop.OnLogExecuted = (s, p) =>
            {
                Console.WriteLine(db.Ado.Connection.ConnectionString);
            };
            Console.WriteLine("Master:");
            db.Insertable(new Order() { Name = "abc", CustomId = 1, CreateTime = DateTime.Now }).ExecuteCommand();
            Console.WriteLine("Slave:");
            db.Queryable<Order>().First();
            Console.WriteLine("#### MasterSlave End ####");
        }

        private static void SqlSugarClient()
        {
            //Create db
            Console.WriteLine("#### SqlSugarClient Start ####");
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                DbType = DbType.Dm,
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

            //If no exist create datebase 
            //db.DbMaintenance.CreateDatabase();

            //Use db query
            var dt = db.Ado.GetDataTable("select 1");

            //Create tables
            db.CodeFirst.InitTables(typeof(OrderItem),typeof(Order));
            var id = db.Insertable(new Order() { Name = "order1", CustomId = 1, Price = 0, CreateTime = DateTime.Now }).ExecuteReturnIdentity();

            //Insert data
            db.Insertable(new OrderItem() { OrderId = id, Price = 0, CreateTime=DateTime.Now }).ExecuteCommand();
            Console.WriteLine("#### SqlSugarClient End ####");

        }

        private static void DbContext()
        {
            Console.WriteLine("");
            Console.WriteLine("#### DbContext Start ####");
            var insertObj = new Order { Name = "jack", CreateTime = DateTime.Now };
            var InsertObjs = new Order[] { insertObj };

            DbContext context = new DbContext();

            context.Db.CodeFirst.InitTables<Order, OrderItem,Custom>();//Create Tables
            ;
            var orderDb = context.OrderDb;

            //Select
            var data1 = orderDb.GetById(1);
            var data2 = orderDb.GetList();
            var data3 = orderDb.GetList(it => it.Id == 1);
            var data4 = orderDb.GetSingle(it => it.Id == 1);
            var p = new PageModel() { PageIndex = 1, PageSize = 2 };
            var data5 = orderDb.GetPageList(it => it.Name == "xx", p);
            Console.Write(p.TotalCount);
            var data6 = orderDb.GetPageList(it => it.Name == "xx", p, it => it.Name, OrderByType.Asc);
            Console.Write(p.TotalCount);
            List<IConditionalModel> conModels = new List<IConditionalModel>();
            conModels.Add(new ConditionalModel() { FieldName = "id", ConditionalType = ConditionalType.Equal, FieldValue = "1", FieldValueConvertFunc=it=>Convert.ToInt32(it) });//id=1
            var data7 = orderDb.GetPageList(conModels, p, it => it.Name, OrderByType.Asc);
            orderDb.AsQueryable().Where(x => x.Id == 1).ToList();

            //Insert
            orderDb.Insert(insertObj);
            orderDb.InsertRange(InsertObjs);
            var id = orderDb.InsertReturnIdentity(insertObj);
            orderDb.AsInsertable(insertObj).ExecuteCommand();


            //Delete
            orderDb.Delete(insertObj);
            orderDb.DeleteById(11111);
            orderDb.DeleteById(new int[] { 1111, 2222 });
            orderDb.Delete(it => it.Id == 1111);
            orderDb.AsDeleteable().Where(it => it.Id == 1111).ExecuteCommand();

            //Update
            orderDb.Update(insertObj);
            orderDb.UpdateRange(InsertObjs);
            orderDb.Update(it => new Order() { Name = "a", }, it => it.Id == 1);
            orderDb.AsUpdateable(insertObj).UpdateColumns(it => new { it.Name }).ExecuteCommand();

            //Use Inherit DbContext
            OrderDal dal = new OrderDal();
            var data = dal.GetById(1);
            var list = dal.GetList();

            Console.WriteLine("#### DbContext End ####");
        }

        private static void CustomAttribute()
        {
            Console.WriteLine("");
            Console.WriteLine("#### Custom Attribute Start ####");
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = Config.ConnectionString,
                DbType = DbType.Dm,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute,
                ConfigureExternalServices = new ConfigureExternalServices()
                {
                    EntityService = (property, column) =>
                    {

                        var attributes = property.GetCustomAttributes(true);//get all attributes 

                        if (attributes.Any(it => it is KeyAttribute))// by attribute set primarykey
                        {
                            column.IsPrimarykey = true;
                        }
                    },
                    EntityNameService = (type, entity) =>
                    {
                        var attributes = type.GetCustomAttributes(true);
                        if (attributes.Any(it => it is TableAttribute))
                        {
                            entity.DbTableName = (attributes.First(it => it is TableAttribute) as TableAttribute).Name;
                        }
                    }
                }
            });
            db.CodeFirst.InitTables<AttributeTable>();//Create Table

            db.Insertable(new AttributeTable() { Id = Guid.NewGuid().ToString(), Name = "Name" }).ExecuteCommand();
            var list = db.Queryable<AttributeTable>().ToList();

            Console.WriteLine("#### Custom Attribute End ####");
        }


        private static void SingletonPattern()
        {
            Console.WriteLine("");
            Console.WriteLine("#### Singleton Pattern Start ####");
            Console.WriteLine("Db_Id:" + singleDb.ContextID);
            Console.WriteLine("Db_Id:" + singleDb.ContextID);
            var task = new Task(() =>
            {
                Console.WriteLine("Task DbId:" + singleDb.ContextID);
                new Task(() =>
                {
                    Console.WriteLine("_Task_Task DbId:" + singleDb.ContextID);
                    Console.WriteLine("_Task_Task DbId:" + singleDb.ContextID);

                }).Start();
                Console.WriteLine("Task DbId:" + singleDb.ContextID);
            });
            task.Start();
            task.Wait();
            System.Threading.Thread.Sleep(500);
            Console.WriteLine(string.Join(",", singleDb.TempItems.Keys));

            Console.WriteLine("#### Singleton Pattern end ####");
        }

        static SqlSugarScope singleDb = new SqlSugarScope(
            new ConnectionConfig()
            {
                ConfigId = 1,
                DbType = DbType.Dm,
                ConnectionString = Config.ConnectionString,
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true,
                AopEvents = new AopEvents()
                {
                    OnLogExecuting = (sql, p) => { Console.WriteLine(sql); }
                }
            });


        private static void DistributedTransactionExample()
        {
            Console.WriteLine("");
            Console.WriteLine("#### Distributed TransactionExample Start ####");
            SqlSugarClient db = new SqlSugarClient(new List<ConnectionConfig>()
            {
                new ConnectionConfig(){ ConfigId="1", DbType=DbType.Dm, ConnectionString=Config.ConnectionString,InitKeyType=InitKeyType.Attribute,IsAutoCloseConnection=true },
                new ConnectionConfig(){ ConfigId="2", DbType=DbType.Dm, ConnectionString=Config.ConnectionString2 ,InitKeyType=InitKeyType.Attribute ,IsAutoCloseConnection=true}
            });

            //use db1
            db.CodeFirst.SetStringDefaultLength(200).InitTables(typeof(Order), typeof(OrderItem));//
            db.Insertable(new Order() { Name = "order1", CreateTime = DateTime.Now }).ExecuteCommand();
            Console.WriteLine(db.CurrentConnectionConfig.DbType + ":" + db.Queryable<Order>().Count());

            //use db2
            db.ChangeDatabase("2");
            //db.DbMaintenance.CreateDatabase();//Create Database2
            db.CodeFirst.SetStringDefaultLength(200).InitTables(typeof(Order), typeof(OrderItem));
            db.Insertable(new Order() { Name = "order1", CreateTime = DateTime.Now }).ExecuteCommand();
            Console.WriteLine(db.CurrentConnectionConfig.DbType + ":" + db.Queryable<Order>().Count());

            // Example 1
            Console.WriteLine("Example 1");
            try
            {
                db.BeginTran();

                db.ChangeDatabase("1");//use db1
                db.Deleteable<Order>().ExecuteCommand();
                Console.WriteLine("---Delete all " + db.CurrentConnectionConfig.DbType);
                Console.WriteLine(db.Queryable<Order>().Count());

                db.ChangeDatabase("2");//use db2
                db.Deleteable<Order>().ExecuteCommand();
                Console.WriteLine("---Delete all " + db.CurrentConnectionConfig.DbType);
                Console.WriteLine(db.Queryable<Order>().Count());

                throw new Exception();
                db.CommitTran();
            }
            catch
            {
                db.RollbackTran();
                Console.WriteLine("---Roll back");
                db.ChangeDatabase("1");//use db1
                Console.WriteLine(db.CurrentConnectionConfig.DbType);
                Console.WriteLine(db.Queryable<Order>().Count());

                db.ChangeDatabase("2");//use db2
                Console.WriteLine(db.CurrentConnectionConfig.DbType);
                Console.WriteLine(db.Queryable<Order>().Count());
            }



            // Example 2
            Console.WriteLine("Example 2");

            var result=db.UseTran(() =>
            {

                db.ChangeDatabase("1");//use db1
                db.Deleteable<Order>().ExecuteCommand();
                Console.WriteLine("---Delete all " + db.CurrentConnectionConfig.DbType);
                Console.WriteLine(db.Queryable<Order>().Count());

                db.ChangeDatabase("2");//use db2
                db.Deleteable<Order>().ExecuteCommand();
                Console.WriteLine("---Delete all " + db.CurrentConnectionConfig.DbType);
                Console.WriteLine(db.Queryable<Order>().Count());
                throw new Exception("");

            });
            if (result.IsSuccess == false) {
                Console.WriteLine("---Roll back");
                db.ChangeDatabase("1");//use db1
                Console.WriteLine(db.CurrentConnectionConfig.DbType);
                Console.WriteLine(db.Queryable<Order>().Count());

                db.ChangeDatabase("2");//use db2
                Console.WriteLine(db.CurrentConnectionConfig.DbType);
                Console.WriteLine(db.Queryable<Order>().Count());
            }

            // Example 3
            Console.WriteLine("Example 3");

            var result2 = db.UseTranAsync(() =>
            {

                db.ChangeDatabase("1");//use db1
                db.Deleteable<Order>().ExecuteCommand();
                Console.WriteLine("---Delete all " + db.CurrentConnectionConfig.DbType);
                Console.WriteLine(db.Queryable<Order>().Count());

                db.ChangeDatabase("2");//use db2
                db.Deleteable<Order>().ExecuteCommand();
                Console.WriteLine("---Delete all " + db.CurrentConnectionConfig.DbType);
                Console.WriteLine(db.Queryable<Order>().Count());
                throw new Exception("");

            });
            result2.Wait();
            if (result.IsSuccess == false)
            {
                Console.WriteLine("---Roll back");
                db.ChangeDatabase("1");//use sqlserver
                Console.WriteLine(db.CurrentConnectionConfig.DbType);
                Console.WriteLine(db.Queryable<Order>().Count());

                db.ChangeDatabase("2");//use mysql
                Console.WriteLine(db.CurrentConnectionConfig.DbType);
                Console.WriteLine(db.Queryable<Order>().Count());
            }

            Console.WriteLine("#### Distributed TransactionExample End ####");
        }
    }

    /// <summary>
    /// DbContext Example 1
    /// </summary>
    public class DbContext
    {

        public SqlSugarClient Db;
        public DbContext()
        {
            Db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = Config.ConnectionString,
                DbType = DbType.Dm,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute,
                AopEvents = new AopEvents()
                {
                    OnLogExecuting = (sql, p) =>
                    {
                        Console.WriteLine(sql);
                    }
                }
            });
        }
        public SimpleClient<Order> OrderDb => new SimpleClient<Order>(Db);
        public SimpleClient<OrderItem> OrderItemDb => new SimpleClient<OrderItem>(Db);
    }


    public class OrderDal : DbContext<Order>
    {

    }
    /// <summary>
    /// DbContext  Example 2
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DbContext<T> where T : class, new()
    {

        public SqlSugarClient Db;
        public DbContext()
        {
            Db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = Config.ConnectionString,
                DbType = DbType.Dm,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute,
                AopEvents = new AopEvents()
                {
                    OnLogExecuting = (sql, p) =>
                    {
                        Console.WriteLine(sql);
                    }
                }
            });
        }
        public SimpleClient<T> CurrentDb => new SimpleClient<T>(Db);
        public virtual T GetById(int id)
        {
            return CurrentDb.GetById(id);
        }
        public virtual List<T> GetList()
        {
            return CurrentDb.GetList();
        }
        public virtual bool Delete(int id)
        {
            return CurrentDb.DeleteById(id);
        }
    }

}
