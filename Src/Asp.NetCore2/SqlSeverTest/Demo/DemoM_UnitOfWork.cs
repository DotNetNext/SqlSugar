using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class DemoM_UnitOfWork
    {
        public static void Init()
        {
            Console.WriteLine("");
            Console.WriteLine("#### DemoM_UnitOfWork ####");

            var db = NewUnitTest.Db;
            db.CurrentConnectionConfig.ConfigId = "1";



            using (var uow = db.CreateContext<MyDbContext>())//带事务
            {
                var list3 = uow.OrderItem.GetList();//查询OrderItem
                var list4 = uow.Orders.GetList();//查询Orders
                //也可以手动调用仓储
                //var orderItemDal=uow.GetMyRepository<DbSet<OrderItem>>();

                uow.Commit();
            }
        }
        /// <summary>
        /// 自定义DbContext
        /// </summary>
        public class MyDbContext : SugarUnitOfWork
        {
            /// <summary>
            /// OrderItem
            /// </summary>
            public DbSet<OrderItem> OrderItem { get; set; }
            /// <summary>
            ///Orders2
            /// </summary>
            public DbSet<ORDER> Orders { get; set; }
        }
        /// <summary>
        /// 自定义仓储
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class DbSet<T> : SimpleClient<T> where T : class, new()
        {
            /// <summary>
            /// 仓储自定义方法
            /// </summary>
            /// <returns></returns>
            public List<T> CommQuery()
            {
                return base.Context.Queryable<T>().ToList();
            }

        }

        [Tenant("1")]
        public class ORDER
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }

            public string Name { get; set; }
            public decimal Price { get; set; }
            [SugarColumn(IsNullable = true)]
            public DateTime CreateTime { get; set; }
            [SugarColumn(IsNullable = true)]
            public int CustomId { get; set; }
            [SugarColumn(IsIgnore = true)]
            public List<OrderItem> Items { get; set; }
        }

    }
}
