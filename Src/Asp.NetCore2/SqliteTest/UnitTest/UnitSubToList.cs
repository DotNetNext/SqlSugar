using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace OrmTest 
{
    public class UnitSubToList
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;

            for (int i = 0; i < 100; i++)
            {
                var guids = db.Queryable<Order>().Select(it => new
                {
                    id = SqlFunc.NewUid()
                }).Take(10).ToList();
            }
            //   db.Aop.OnLogExecuting = null;

            db.DbMaintenance.TruncateTable<Order, OrderItem, Custom>();

            db.Insertable(new Order() { Id = 1, Name = "订单01", CustomId = 1, Price = 111, CreateTime = DateTime.Now }).ExecuteCommand();
            db.Insertable(new Order() { Id = 1, Name = "订单011", CustomId = 1, Price = 1111, CreateTime = DateTime.Now }).ExecuteCommand();
            db.Insertable(new Order() { Id = 1, Name = "订单033", CustomId = 3, Price = 31, CreateTime = DateTime.Now }).ExecuteCommand();
            db.Insertable(new Order() { Id = 1, Name = "订单03", CustomId = 3, Price = 331, CreateTime = DateTime.Now }).ExecuteCommand();
            db.Insertable(new Order() { Id = 1, Name = "订单04", CustomId = 4, Price = 411, CreateTime = DateTime.Now }).ExecuteCommand();

            db.Insertable(new Custom() { Id = 1, Name = "客户1",  }).ExecuteCommand();
            db.Insertable(new Custom() { Id = 3, Name = "客户3",  }).ExecuteCommand();
            db.Insertable(new Custom() { Id = 4, Name = "客户4",   }).ExecuteCommand();

            db.Insertable(new OrderItem() { ItemId = 1,  OrderId =1, Price=1 }).ExecuteCommand();
            db.Insertable(new OrderItem() { ItemId = 3,  OrderId =3, Price=3}).ExecuteCommand();
            db.Insertable(new OrderItem() { ItemId = 4,  OrderId=4 , Price=4}).ExecuteCommand();

            TestGetAll(db);
            TestWhere(db);
            TestJoin(db);
            TestJoin2(db);
            TestJoin3(db);
            TestJoin4(db);
        }
        private static void TestJoin4(SqlSugarClient db)
        {
            var test1 = db.Queryable<Order>()
                .LeftJoin<Custom>((o, c) => c.Id == o.CustomId)
                .Select((o, c) => new myDTO5
                {
                    OrderName = o.Name,
                    disCount = SqlFunc.Subqueryable<Custom>().Where(d => d.Name == c.Name).ToList()
                },
                true)
              .ToList();
            if (test1.Any(z => z.disCount.Any(y => y.Id != z.CustomId)))
            {
                throw new Exception("unit error");
            }
            var test2= db.Queryable<Order>()
              .Select(o => new myDTO5
              { 
                  disCount = SqlFunc.Subqueryable<Custom>().Where(d => d.Id == o.CustomId).ToList()
              },
              true)
            .ToList();
            if (test2.Any(z => z.disCount.Any(y => y.Id != z.CustomId))||test2.Any(z=>z.Id==0))
            {
                throw new Exception("unit error");
            }
        }
        private static void TestJoin3(SqlSugarClient db)
        {
            var test1 = db.Queryable<Order>()
                .LeftJoin<Custom>((o, c) => c.Id == o.CustomId)
                .Select((o, c) => new
                {
                    CustomId = o.CustomId,
                    OrderId = o.Id,
                    OrderName = o.Name,
                    disCount = SqlFunc.Subqueryable<Custom>().Where(d => d.Name == c.Name).ToList()
                })
           .ToList();
            if (test1.Any(z => z.disCount.Any(y => y.Id != z.CustomId)))
            {
                throw new Exception("unit error");
            }
            var test2 = db.Queryable<Custom>()
                .LeftJoin<Order>((o, c) => c.CustomId == o.Id)
                .Select((o, c) => new
                {
                    Id=o.Id,
                    cusName = o.Name,
                    Orders = SqlFunc.Subqueryable<Order>().Where(d => d.CustomId == o.Id).ToList()
                })
                .ToList();
            if (test2.Any(z => z.Orders.Any(y => y.CustomId != z.Id))|| test2.First().Orders.Count()==0)
            {
                throw new Exception("unit error");
            }
        }
        private static void TestJoin2(SqlSugarClient db)
        {
            var test1 = db.Queryable<Order>()
                .LeftJoin<Custom>((o, c) => c.Id == o.CustomId)
                .LeftJoin<OrderItem>((o, c, i) => i.OrderId == o.Id)
                .Select((o, c, i) => new  
                {
                    itemId = i.ItemId,
                    CustomId = o.CustomId,
                    OrderId = o.Id,
                    OrderName = o.Name,
                    disCount = SqlFunc.Subqueryable<OrderItem>().Where(d => d.ItemId == i.ItemId).ToList()
                })
           .ToList();
            if (test1.Any(z => z.disCount.Any(y => y.ItemId != z.itemId)))
            {
                throw new Exception("unit error");
            }
            var test2 = db.Queryable<Order>()
             .LeftJoin<Custom>((o, c) => c.Id == o.CustomId)
             .LeftJoin<OrderItem>((o, c, i) => i.OrderId == o.Id)
             .Select((o, c, i) => new  
             {
                 itemId = i.ItemId,
                 CustomId = o.CustomId,
                 CusName = SqlFunc.Subqueryable<Custom>().Where(s => s.Id == o.CustomId).Select(s => s.Name),
                 OrderId = o.Id,
                 cusList = SqlFunc.Subqueryable<Custom>().Where(d => d.Id == o.CustomId).ToList(),
                 OrderName = o.Name,
                 disCount = SqlFunc.Subqueryable<OrderItem>().Where(d => d.ItemId == i.ItemId).ToList()
             })
            .ToList();
            if (test2.Any(z => z.disCount.Any(y => y.ItemId != z.itemId)))
            {
                throw new Exception("unit error");
            }
            if (test2.Any(z => z.disCount.Any(y => y.ItemId != z.itemId)))
            {
                throw new Exception("unit error");
            }
            if (test2.Any(z => z.cusList.Any(y => y.Id != z.CustomId)))
            {
                throw new Exception("unit error");
            }
            if (test2.Any(z => z.cusList.Any(y => y.Name != z.CusName)))
            {
                throw new Exception("unit error");
            }
        }
        private static void TestJoin(SqlSugarClient db)
        {
            var test1 = db.Queryable<Order>()
                .LeftJoin<Custom>((o,c)=>c.Id==o.CustomId)
                .LeftJoin<OrderItem>((o, c, i) => i.OrderId==o.Id)
                .Select((o,c,i) => new myDTO4
                 {
                itemId=i.ItemId,
                CustomId = o.CustomId,
                OrderId = o.Id,
                OrderName = o.Name,
                disCount = SqlFunc.Subqueryable<OrderItem>().Where(d => d.ItemId == i.ItemId).ToList()
            })
           .ToList();
            if (test1.Any(z => z.disCount.Any(y => y.ItemId != z.itemId)))
            {
                throw new Exception("unit error");
            }
            var test2 = db.Queryable<Order>()
             .LeftJoin<Custom>((o, c) => c.Id == o.CustomId)
             .LeftJoin<OrderItem>((o, c, i) => i.OrderId == o.Id)
             .Select((o, c, i) => new myDTO4
             {
                 itemId = i.ItemId,
                 CustomId = o.CustomId,
                 CusName=SqlFunc.Subqueryable<Custom>().Where(s=>s.Id==o.CustomId).Select(s=>s.Name),
                 OrderId = o.Id,
                 cusList= SqlFunc.Subqueryable<Custom>().Where(d => d.Id == o.CustomId).ToList(),
                 OrderName = o.Name,
                 disCount = SqlFunc.Subqueryable<OrderItem>().Where(d => d.ItemId == i.ItemId).ToList()
             })
            .ToList();
            if (test2.Any(z => z.disCount.Any(y => y.ItemId != z.itemId)))
            {
                throw new Exception("unit error");
            }
            if (test2.Any(z => z.disCount.Any(y => y.ItemId != z.itemId)))
            {
                throw new Exception("unit error");
            }
            if (test2.Any(z => z.cusList.Any(y => y.Id != z.CustomId)))
            {
                throw new Exception("unit error");
            }
            if (test2.Any(z => z.cusList.Any(y => y.Name != z.CusName)))
            {
                throw new Exception("unit error");
            }
        }
        private static void TestWhere(SqlSugarClient db)
        {
            var test1 = db.Queryable<Order>().Select(it => new myDTO3
            {
                CustomId = it.CustomId,
                OrderId=it.Id,
                OrderName=it.Name,
                disCount = SqlFunc.Subqueryable<Custom>().Where(c=>c.Id==it.CustomId).ToList()
            })
           .ToList();

            if (test1.Any(z => z.disCount.Any(y => y.Id != z.CustomId))) 
            {
                throw new Exception("unit error");
            }
 
            var test2 = db.Queryable<Order>().Select(it => new
            {
                CustomId = it.CustomId,
                OrderId = it.Id,
                OrderName = it.Name,
                disCount = SqlFunc.Subqueryable<Custom>().Where(c => c.Id == it.CustomId).ToList()
            })
            .ToList();


            if (test2.Any(z => z.disCount.Any(y => y.Id != z.CustomId)))
            {
                throw new Exception("unit error");
            }


            var test3 = db.Queryable<Order>().Select(it => new myDTO3
            {
                CustomId = it.CustomId,
                OrderId = it.Id,
                OrderName = it.Name,
                disCount = SqlFunc.Subqueryable<Custom>().Where(c => c.Id == it.CustomId).ToList()
            })
           .ToListAsync().GetAwaiter().GetResult();

            if (test3.Any(z => z.disCount.Any(y => y.Id != z.CustomId)))
            {
                throw new Exception("unit error");
            }

            var test4 = db.Queryable<Order>().Select(it => new
            {
                CustomId = it.CustomId,
                OrderId = it.Id,
                OrderName = it.Name,
                disCount = SqlFunc.Subqueryable<Custom>().Where(c => c.Id == it.CustomId).ToList()
            })
            .ToListAsync().GetAwaiter().GetResult();

            if (test4.Any(z => z.disCount.Any(y => y.Id != z.CustomId)))
            {
                throw new Exception("unit error");
            }

        }
        private static void TestGetAll(SqlSugarClient db)
        {
            var test1 = db.Queryable<Order>().Select(it => new myDTO
            {
                Id = it.Id,
                disCount = SqlFunc.Subqueryable<Order>().ToList()
            })
           .ToList();

            if (test1.First().disCount.Count != test1.Count) 
            {
                throw new Exception("unit error");
            }

            var test2 = db.Queryable<Order>().Select(it => new
            {
                Id = it.Id,
                disCount = SqlFunc.Subqueryable<Order>().ToList()
            })
            .ToList();

            if (test2.First().disCount.Count != test2.Count)
            {
                throw new Exception("unit error");
            }

            var test3 = db.Queryable<Order>().Select(it => new myDTO
            {
                Id = it.Id,
                disCount = SqlFunc.Subqueryable<Order>().ToList()
            })
           .ToListAsync().GetAwaiter().GetResult();

            if (test3.First().disCount.Count != test3.Count)
            {
                throw new Exception("unit error");
            }

            var test4 = db.Queryable<Order>().Select(it => new
            {
                Id = it.Id,
                disCount = SqlFunc.Subqueryable<Order>().ToList()
            })
            .ToListAsync().GetAwaiter().GetResult();

            if (test4.First().disCount.Count != test4.Count)
            {
                throw new Exception("unit error");
            }

        }

        internal class myDTO
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public List<Order> disCount { get; set; }
        }
        internal class myDTO2
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public List<OrderItem> disCount { get; set; }
        }
    }

    internal class myDTO5
    {
        public int CustomId { get; set; }
        public int OrderId { get; set; }
        public string OrderName { get; set; }
        public List<Custom> disCount { get; set; }
        public int Id { get; internal set; }
    }

    internal class myDTO4
    {
        public int CustomId { get; set; }
        public int OrderId { get; set; }
        public string OrderName { get; set; }
        public List<OrderItem> disCount { get; set; }
        public int itemId { get; internal set; }
        public List<Custom> cusList { get; internal set; }
        public string CusName { get; internal set; }
    }

    internal class myDTO3
    {
        public int Id { get; set; }
        public List<Custom> disCount { get; set; }
        public int OrderId { get; internal set; }
        public string OrderName { get; internal set; }
        public int CustomId { get; internal set; }
    }
}
