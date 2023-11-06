using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OrmTest._4_JoinQuery;

namespace OrmTest
{
    internal class _4_JoinQuery
    {
        public static void Init()
        {
            InitializeDatabase();
            SyntaxSugar1();
            SyntaxSugar2();
            SyntaxSugar3();
        }
        /// <summary>
        /// 使用语法糖1进行联表查询。
        /// Performs a Join query using Syntax Sugar 1.
        /// </summary>
        /// <returns>查询结果列表。The list of query results.</returns>
        public static List<ViewOrder2> SyntaxSugar1()
        {
            var db = DbHelper.GetNewDb();
            var query = db.Queryable<Order>()
                .LeftJoin<Custom>((o, cus) => o.CustomId == cus.Id)
                .LeftJoin<OrderDetail>((o, cus, oritem) => o.Id == oritem.OrderId)
                .Where(o => o.Id == 1)
                .Select((o, cus, oritem) => new ViewOrder2 { Id = o.Id,Name=o.Name, CustomName = cus.Name })
                .ToList();

            return query;
        }

        /// <summary>
        /// 使用语法糖2进行联表查询。
        /// Performs a Join query using Syntax Sugar 2.
        /// </summary>
        /// <returns>查询结果列表。The list of query results.</returns>
        public static List<Order> SyntaxSugar2()
        {
            var db = DbHelper.GetNewDb();
            var rightQueryable = db.Queryable<Custom>()
                .LeftJoin<OrderDetail>((o, i) => o.Id == i.Id)
                .Select(o => o);

            var list = db.Queryable<Order>()
                .LeftJoin(rightQueryable, (c, j) => c.CustomId == j.Id)
                .Select(c => c)
                .ToList();
            return list;
        }

        /// <summary>
        /// 使用语法糖3进行联表查询。
        /// Performs a Join query using Syntax Sugar 3.
        /// </summary>
        /// <returns>查询结果列表。The list of query results.</returns>
        public static List<ViewOrder2> SyntaxSugar3()
        {
            var db = DbHelper.GetNewDb();
            var list = db.Queryable<Order, OrderDetail, Custom>((o, i, c) => o.Id == i.OrderId && c.Id == o.CustomId)
                .Select((o, i, c) => new ViewOrder2 { Id = o.Id, Name = o.Name, CustomName = c.Name })
                .ToList();

            return list;
        }
         

        static void InitializeDatabase()
        {
            // Initialize order data
            // 初始化订单数据
            var orders = new List<Order>
            {
                new Order { Id = 1, Name = "Order 1", CustomId = 1 },
                new Order { Id = 2, Name = "Order 2", CustomId = 2 },
                new Order { Id = 3, Name = "Order 3", CustomId = 1 },
            };

            // Initialize order details data
            // 初始化订单详情数据
            var orderDetails = new List<OrderDetail>
            {
                new OrderDetail { Id = 1, OrderId = 1 },
                new OrderDetail { Id = 2, OrderId = 2 },
                new OrderDetail { Id = 3, OrderId = 3 },
            };

            // Initialize customer data
            // 初始化客户数据
            var customers = new List<Custom>
            {
                new Custom { Id = 1, Name = "Customer 1" },
                new Custom { Id = 2, Name = "Customer 2" },
                new Custom { Id = 3, Name = "Customer 3" },
            };

            // Get database connection
            // 获取数据库连接
            var db = DbHelper.GetNewDb();

            // Initialize database tables and truncate data
            // 初始化数据库表并清空数据
            db.CodeFirst.InitTables<Custom, OrderDetail, Order>();
            db.DbMaintenance.TruncateTable<Custom, OrderDetail, Order>();

            // Insert data into tables
            // 向表中插入数据
            db.Insertable(orders).ExecuteCommand();
            db.Insertable(orderDetails).ExecuteCommand();
            db.Insertable(customers).ExecuteCommand();
        }

        /// <summary>
        /// 订单实体类。
        /// Order entity class.
        /// </summary>
        [SqlSugar.SugarTable("Order04")]
        public class Order
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }
            public string Name { get; set; }
            public int CustomId { get; set; }
            // 其他订单相关属性...
        }

        /// <summary>
        /// 订单详情实体类。
        /// Order detail entity class.
        /// </summary>
        [SqlSugar.SugarTable("OrderDetail04")]
        public class OrderDetail
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }
            public int OrderId { get; set; }
            // 其他订单详情相关属性...
        }

        /// <summary>
        /// 客户实体类。
        /// Customer entity class.
        /// </summary>
        [SqlSugar.SugarTable("Custom04")]
        public class Custom
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }
            public string Name { get; set; }
            // 其他客户相关属性...
        }
 

        
        /// <summary>
        /// 类1实体类。
        /// Class1 entity class.
        /// </summary>
        public class ViewOrder2
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string CustomName { get; set; }
            // 其他类1相关属性...
        }
    }
}
