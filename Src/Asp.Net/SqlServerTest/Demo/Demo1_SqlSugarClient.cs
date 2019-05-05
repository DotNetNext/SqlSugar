using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace OrmTest
{
    public class Demo1_SqlSugarClient
    {

        public static void Init()
        {
            SingletonPattern();//单例
            DistributedTransactionExample();//分布式事务
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

        static SqlSugarClient singleDb = new SqlSugarClient(
            new ConnectionConfig()
            {
                ConfigId = 1,
                DbType = DbType.SqlServer,
                ConnectionString = Config.ConnectionString,
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection=true,
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
                new ConnectionConfig(){ ConfigId=1, DbType=DbType.SqlServer, ConnectionString=Config.ConnectionString,InitKeyType=InitKeyType.Attribute,IsAutoCloseConnection=true },
                new ConnectionConfig(){ ConfigId=2, DbType=DbType.MySql, ConnectionString=Config.ConnectionString4 ,InitKeyType=InitKeyType.Attribute ,IsAutoCloseConnection=true}
            });

            //use first(SqlServer)
            db.CodeFirst.SetStringDefaultLength(200).InitTables(typeof(Order), typeof(OrderItem));//
            db.Insertable(new Order() { Name = "order1", CreateTime = DateTime.Now }).ExecuteCommand();
            Console.WriteLine(db.CurrentConnectionConfig.DbType + ":" + db.Queryable<Order>().Count());

            //use mysql
            db.ChangeDatabase(it => it.DbType == DbType.MySql);
            db.CodeFirst.SetStringDefaultLength(200).InitTables(typeof(Order), typeof(OrderItem));
            db.Insertable(new Order() { Name = "order1", CreateTime = DateTime.Now }).ExecuteCommand();
            Console.WriteLine(db.CurrentConnectionConfig.DbType + ":" + db.Queryable<Order>().Count());

            //SqlServer
            db.ChangeDatabase(it => it.DbType == DbType.SqlServer);//use sqlserver
            try
            {
                db.BeginAllTran();

                db.ChangeDatabase(it => it.DbType == DbType.SqlServer);//use sqlserver
                db.Deleteable<Order>().ExecuteCommand();
                Console.WriteLine("---Delete all " + db.CurrentConnectionConfig.DbType);
                Console.WriteLine(db.Queryable<Order>().Count());

                db.ChangeDatabase(it => it.DbType == DbType.MySql);//use mysql
                db.Deleteable<Order>().ExecuteCommand();
                Console.WriteLine("---Delete all " + db.CurrentConnectionConfig.DbType);
                Console.WriteLine(db.Queryable<Order>().Count());

                throw new Exception();
                db.CommitAllTran();
            }
            catch
            {
                db.RollbackAllTran();
                Console.WriteLine("---Roll back");
                db.ChangeDatabase(it => it.DbType == DbType.SqlServer);//use sqlserver
                Console.WriteLine(db.CurrentConnectionConfig.DbType);
                Console.WriteLine(db.Queryable<Order>().Count());

                db.ChangeDatabase(it => it.DbType == DbType.MySql);//use mysql
                Console.WriteLine(db.CurrentConnectionConfig.DbType);
                Console.WriteLine(db.Queryable<Order>().Count());
            }

            Console.WriteLine("#### Distributed TransactionExample End ####");
        }



    }
}
