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
            using (var uow = db.CreateContext<MyDbContext>())
            {
                var o = uow.GetRepository<ORDER>();
                var o2 = uow.GetMyRepository<DbSet<ORDER>>();
                var list = o.GetList();//默认仓储
                var list2 = o2.CommQuery();//自定义仓储
                var list3 = uow.Orders1.GetList();//MyDbContext中的默认仓储
                var list4 = uow.Orders2.GetList();//MyDbContext中的自定义仓储
                uow.Commit();
            }
        }
        /// <summary>
        /// 自定义DbContext
        /// </summary>
        public class MyDbContext : SugarUnitOfWork
        {
            /// <summary>
            /// 原生仓储
            /// </summary>
            public SimpleClient<ORDER> Orders1 { get; set; }
            /// <summary>
            ///自定义仓储
            /// </summary>
            public DbSet<ORDER> Orders2 { get; set; }
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
