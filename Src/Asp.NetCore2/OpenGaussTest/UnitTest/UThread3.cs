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

        public static void Thread3()
        {
            Console.WriteLine("Thread3");
            SimpleAsync3().Wait();
            IsShardSameThreadAsync3().Wait();
            SingleAsync3().Wait();
            SingleAndIsShardSameThreadAsync3().Wait();

        }



        private static async Task SimpleAsync3()
        {

            for (int i = 0; i < 100; i++)
            {
                await simpleDb.Queryable<Order>().Take(10).ToListAsync();
            }

            for (int i = 0; i < 100; i++)
            {
                await simpleDb.Insertable(new Order() { Name = "a", CustomId = 1 }).ExecuteCommandAsync();
            }

            List<Order> orders = new List<Order>();
            for (int i = 0; i < 100; i++)
            {
                orders = await simpleDb.Queryable<Order>().Take(10).ToListAsync();
            }
            if (orders.Count > 0)
            {
                Console.WriteLine("async is ok");
            }
        }

        private static async Task SingleAndIsShardSameThreadAsync3()
        {
            for (int i = 0; i < 100; i++)
            {
                await singleAndSsDb.Queryable<Order>().Take(10).ToListAsync();
            }

            for (int i = 0; i < 100; i++)
            {
                await singleAndSsDb.Insertable(new Order() { Name = "a", CustomId = 1 }).ExecuteCommandAsync();
            }
            List<Order> orders = new List<Order>();
            for (int i = 0; i < 100; i++)
            {
                orders = await singleAndSsDb.Queryable<Order>().Take(10).ToListAsync();
            }
            if (orders.Count > 0)
            {
                Console.WriteLine("async is ok");
            }
        }

        private static async Task SingleAsync3()
        {
            for (int i = 0; i < 100; i++)
            {
                await singleDb.Queryable<Order>().Take(10).ToListAsync();
            }

            for (int i = 0; i < 100; i++)
            {
                await singleDb.Insertable(new Order() { Name = "a", CustomId = 1 }).ExecuteCommandAsync();
            }

            List<Order> orders = new List<Order>();
            for (int i = 0; i < 100; i++)
            {
                orders = await singleDb.Queryable<Order>().Take(10).ToListAsync();
            }
            if (orders.Count > 0)
            {
                Console.WriteLine("async is ok");
            }
        }

        private static async Task IsShardSameThreadAsync3()
        {
            for (int i = 0; i < 100; i++)
            {
                await ssDb.Queryable<Order>().Take(10).ToListAsync();
            }

            for (int i = 0; i < 100; i++)
            {
                await ssDb.Insertable(new Order() { Name = "a", CustomId = 1 }).ExecuteCommandAsync();
            }

            List<Order> orders = new List<Order>();
            for (int i = 0; i < 100; i++)
            {
                orders = await ssDb.Queryable<Order>().Take(10).ToListAsync();
            }
            if (orders.Count > 0)
            {
                Console.WriteLine("async is ok");
            }
        }
    }
}
