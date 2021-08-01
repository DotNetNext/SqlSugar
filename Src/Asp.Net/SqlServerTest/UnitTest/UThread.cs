using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public partial class NewUnitTest
    {

        public static SqlSugarScope simpleDb => new SqlSugarScope(new ConnectionConfig()
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
        public static ConnectionConfig Config = new ConnectionConfig()
        {
            DbType = DbType.SqlServer,
            ConnectionString = "server=.;uid=sa;pwd=sasa;database=SQLSUGAR4XTEST",
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
        };

        public static SqlSugarScope ssDb => new SqlSugarScope(Config);
        public static SqlSugarScope singleDb =  new SqlSugarScope(new ConnectionConfig()
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
        public static SqlSugarScope singleAndSsDb = new SqlSugarScope(new ConnectionConfig()
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
        public static void Thread()
        {
            Simple();
            IsShardSameThread();
            Single();
            SingleAndIsShardSameThread();
            SimpleAsync();
            IsShardSameThreadAsync();
            SingleAsync();
            SingleAndIsShardSameThreadAsync();

        }

        private static void Simple()
        {
            var t1 = new Task(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    simpleDb.Insertable(new Order() { Name = "test", CreateTime = DateTime.Now }).ExecuteCommand();
                    System.Threading.Thread.Sleep(1);
                }

            });
            var t2 = new Task(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    simpleDb.Insertable(new Order() { Name = "test2", CreateTime = DateTime.Now }).ExecuteCommand();
                    System.Threading.Thread.Sleep(10);
                }

            });
            var t3 = new Task(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    simpleDb.Insertable(new Order() { Name = "test3", CreateTime = DateTime.Now }).ExecuteCommand();
                    System.Threading.Thread.Sleep(6);
                }

            });
            t1.Start();
            t2.Start();
            t3.Start();

            Task.WaitAll(t1, t2, t3);
        }

        private static void SingleAndIsShardSameThread()
        {
            var t1 = new Task(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    singleAndSsDb.Insertable(new Order() { Name = "test", CreateTime = DateTime.Now }).ExecuteCommand();
                    System.Threading.Thread.Sleep(1);
                }

            });
            var t2 = new Task(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    singleAndSsDb.Insertable(new Order() { Name = "test2", CreateTime = DateTime.Now }).ExecuteCommand();
                    System.Threading.Thread.Sleep(10);
                }

            });
            var t3 = new Task(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    singleAndSsDb.Insertable(new Order() { Name = "test3", CreateTime = DateTime.Now }).ExecuteCommand();
                    System.Threading.Thread.Sleep(6);
                }

            });
            t1.Start();
            t2.Start();
            t3.Start();

            Task.WaitAll(t1, t2, t3);
        }

        private static void Single()
        {
            var t1 = new Task(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    singleDb.Insertable(new Order() { Name = "test", CreateTime = DateTime.Now }).ExecuteCommand();
                    System.Threading.Thread.Sleep(1);
                }

            });
            var t2 = new Task(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    singleDb.Insertable(new Order() { Name = "test2", CreateTime = DateTime.Now }).ExecuteCommand();
                    System.Threading.Thread.Sleep(10);
                }

            });
            var t3 = new Task(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    singleDb.Insertable(new Order() { Name = "test3", CreateTime = DateTime.Now }).ExecuteCommand();
                    System.Threading.Thread.Sleep(6);
                }

            });
            t1.Start();
            t2.Start();
            t3.Start();

            Task.WaitAll(t1, t2, t3);
        }

        private static void IsShardSameThread()
        {
            var t1 = new Task(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    Db.Insertable(new Order() { Name = "test", CreateTime = DateTime.Now }).ExecuteCommand();
                    System.Threading.Thread.Sleep(1);
                }

            });
            var t2 = new Task(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    Db.Insertable(new Order() { Name = "test2", CreateTime = DateTime.Now }).ExecuteCommand();
                    System.Threading.Thread.Sleep(10);
                }

            });
            var t3 = new Task(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    Db.Insertable(new Order() { Name = "test3", CreateTime = DateTime.Now }).ExecuteCommand();
                    System.Threading.Thread.Sleep(6);
                }

            });
            t1.Start();
            t2.Start();
            t3.Start();

            Task.WaitAll(t1, t2, t3);
        }



        private static void SimpleAsync()
        {
            var t1 = new Task(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    simpleDb.Insertable(new Order() { Name = "test", CreateTime = DateTime.Now }).ExecuteCommandAsync().Wait();
                    System.Threading.Thread.Sleep(1);
                }

            });
            var t2 = new Task(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    simpleDb.Insertable(new Order() { Name = "test2", CreateTime = DateTime.Now }).ExecuteCommandAsync().Wait(); ;
                    System.Threading.Thread.Sleep(10);
                }

            });
            var t3 = new Task(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    simpleDb.Insertable(new Order() { Name = "test3", CreateTime = DateTime.Now }).ExecuteCommandAsync().Wait();
                    System.Threading.Thread.Sleep(6);
                }

            });
            t1.Start();
            t2.Start();
            t3.Start();

            Task.WaitAll(t1, t2, t3);
        }

        private static void SingleAndIsShardSameThreadAsync()
        {
            var t1 = new Task(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    singleAndSsDb.Insertable(new Order() { Name = "test", CreateTime = DateTime.Now }).ExecuteCommandAsync().Wait();
                    System.Threading.Thread.Sleep(1);
                }

            });
            var t2 = new Task(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    singleAndSsDb.Insertable(new Order() { Name = "test2", CreateTime = DateTime.Now }).ExecuteCommandAsync().Wait();
                    System.Threading.Thread.Sleep(10);
                }

            });
            var t3 = new Task(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    singleAndSsDb.Insertable(new Order() { Name = "test3", CreateTime = DateTime.Now }).ExecuteCommandAsync().Wait();
                    System.Threading.Thread.Sleep(6);
                }

            });
            t1.Start();
            t2.Start();
            t3.Start();

            Task.WaitAll(t1, t2, t3);
        }

        private static void SingleAsync()
        {
            var t1 = new Task(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    //singleDb.Insertable(new Order() { Name = "test", CreateTime = DateTime.Now }).ExecuteCommandAsync().Wait();
                    //System.Threading.Thread.Sleep(1); No Support
                }

            });
            var t2 = new Task(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    //singleDb.Insertable(new Order() { Name = "test2", CreateTime = DateTime.Now }).ExecuteCommandAsync().Wait();
                    //System.Threading.Thread.Sleep(10); No Support
                }

            });
            var t3 = new Task(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    //singleDb.Insertable(new Order() { Name = "test3", CreateTime = DateTime.Now }).ExecuteCommandAsync().Wait();
                    //System.Threading.Thread.Sleep(6); No Support
                }

            });
            t1.Start();
            t2.Start();
            t3.Start();

            Task.WaitAll(t1, t2, t3);
        }

        private static void IsShardSameThreadAsync()
        {
            var t1 = new Task(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    Db.Insertable(new Order() { Name = "test", CreateTime = DateTime.Now }).ExecuteCommandAsync().Wait();
                    System.Threading.Thread.Sleep(1);
                }

            });
            var t2 = new Task(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    Db.Insertable(new Order() { Name = "test2", CreateTime = DateTime.Now }).ExecuteCommandAsync().Wait();
                    System.Threading.Thread.Sleep(10);
                }

            });
            var t3 = new Task(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    Db.Insertable(new Order() { Name = "test3", CreateTime = DateTime.Now }).ExecuteCommandAsync().Wait();
                    System.Threading.Thread.Sleep(6);
                }

            });
            t1.Start();
            t2.Start();
            t3.Start();

            Task.WaitAll(t1, t2, t3);
        }
    }
}
