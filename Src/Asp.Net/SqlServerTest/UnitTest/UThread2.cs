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
 
        public static void Thread2()
        {
            Simple2();
            IsShardSameThread2();
            Single2();
            SingleAndIsShardSameThread2();
            SimpleAsync2();
            IsShardSameThreadAsync2();
            SingleAsync2();
            SingleAndIsShardSameThreadAsync2();

        }

        private static void Simple2()
        {
            var t1 = new Task(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    simpleDb.Queryable<Order>().Take(10).ToList();
                    System.Threading.Thread.Sleep(1);
                }

            });
            var t2 = new Task(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    simpleDb.Queryable<Order>().Take(10).ToList();
                    System.Threading.Thread.Sleep(10);
                }

            });
            var t3 = new Task(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    simpleDb.Queryable<Order>().Take(10).ToList();
                    System.Threading.Thread.Sleep(6);
                }

            });
            t1.Start();
            t2.Start();
            t3.Start();

            Task.WaitAll(t1, t2, t3);
        }

        private static void SingleAndIsShardSameThread2()
        {
            var t1 = new Task(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    simpleDb.Queryable<Order>().Take(10).ToList();
                    System.Threading.Thread.Sleep(1);
                }

            });
            var t2 = new Task(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    simpleDb.Queryable<Order>().Take(10).ToList();
                    System.Threading.Thread.Sleep(10);
                }

            });
            var t3 = new Task(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    simpleDb.Queryable<Order>().Take(10).ToList();
                    System.Threading.Thread.Sleep(6);
                }

            });
            t1.Start();
            t2.Start();
            t3.Start();

            Task.WaitAll(t1, t2, t3);
        }

        private static void Single2()
        {
            var t1 = new Task(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    simpleDb.Queryable<Order>().Take(10).ToList();
                    System.Threading.Thread.Sleep(1);
                }

            });
            var t2 = new Task(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    simpleDb.Queryable<Order>().Take(10).ToList();
                    System.Threading.Thread.Sleep(10);
                }

            });
            var t3 = new Task(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    simpleDb.Queryable<Order>().Take(10).ToList();
                    System.Threading.Thread.Sleep(6);
                }

            });
            t1.Start();
            t2.Start();
            t3.Start();

            Task.WaitAll(t1, t2, t3);
        }

        private static void IsShardSameThread2()
        {
            var t1 = new Task(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    simpleDb.Queryable<Order>().Take(10).ToList();
                    System.Threading.Thread.Sleep(1);
                }

            });
            var t2 = new Task(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    simpleDb.Queryable<Order>().Take(10).ToList();
                    System.Threading.Thread.Sleep(10);
                }

            });
            var t3 = new Task(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    simpleDb.Queryable<Order>().Take(10).ToList();
                    System.Threading.Thread.Sleep(6);
                }

            });
            t1.Start();
            t2.Start();
            t3.Start();

            Task.WaitAll(t1, t2, t3);
        }



        private static void SimpleAsync2()
        {
            var t1 = new Task(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    simpleDb.Queryable<Order>().Take(10).ToListAsync().Wait();
                    System.Threading.Thread.Sleep(1);
                }

            });
            var t2 = new Task(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    simpleDb.Queryable<Order>().Take(10).ToListAsync().Wait(); ;
                    System.Threading.Thread.Sleep(10);
                }

            });
            var t3 = new Task(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    simpleDb.Queryable<Order>().Take(10).ToListAsync().Wait();
                    System.Threading.Thread.Sleep(6);
                }

            });
            t1.Start();
            t2.Start();
            t3.Start();

            Task.WaitAll(t1, t2, t3);
        }

        private static void SingleAndIsShardSameThreadAsync2()
        {
            var t1 = new Task(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    simpleDb.Queryable<Order>().Take(10).ToListAsync().Wait();
                    System.Threading.Thread.Sleep(1);
                }

            });
            var t2 = new Task(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    simpleDb.Queryable<Order>().Take(10).ToListAsync().Wait();
                    System.Threading.Thread.Sleep(10);
                }

            });
            var t3 = new Task(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    simpleDb.Queryable<Order>().Take(10).ToListAsync().Wait();
                    System.Threading.Thread.Sleep(6);
                }

            });
            t1.Start();
            t2.Start();
            t3.Start();

            Task.WaitAll(t1, t2, t3);
        }

        private static void SingleAsync2()
        {
            var t1 = new Task(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    simpleDb.Queryable<Order>().Take(10).ToListAsync().Wait();
                    System.Threading.Thread.Sleep(1);
                }

            });
            var t2 = new Task(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    simpleDb.Queryable<Order>().Take(10).ToListAsync().Wait();
                    System.Threading.Thread.Sleep(10);
                }

            });
            var t3 = new Task(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    simpleDb.Queryable<Order>().Take(10).ToListAsync().Wait();
                    System.Threading.Thread.Sleep(6);
                }

            });
            t1.Start();
            t2.Start();
            t3.Start();

            Task.WaitAll(t1, t2, t3);
        }

        private static void IsShardSameThreadAsync2()
        {
            var t1 = new Task(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    simpleDb.Queryable<Order>().Take(10).ToListAsync().Wait();
                    System.Threading.Thread.Sleep(1);
                }

            });
            var t2 = new Task(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    simpleDb.Queryable<Order>().Take(10).ToListAsync().Wait();
                    System.Threading.Thread.Sleep(10);
                }

            });
            var t3 = new Task(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    simpleDb.Queryable<Order>().Take(10).ToListAsync().Wait();
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
