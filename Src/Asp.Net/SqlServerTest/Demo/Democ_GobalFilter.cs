using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest
{
    public class DemoC_GobalFilter
    {
        public static void Init()
        {
            Console.WriteLine("");
            Console.WriteLine("#### Filter Start ####");

            TableFilterDemo();

            NameFilterDemo();

            Console.WriteLine("#### Filter End ####");
        }
        private static void TableFilterDemo()
        {
            var db = GetInstance();
            //Order add filter  
            db.QueryFilter.Add(new TableFilterItem<Order>(it => it.Name.Contains("a")));


            db.Queryable<Order>().ToList();
            //SELECT [Id],[Name],[Price],[CreateTime],[CustomId] FROM [Order]  WHERE  ([Name] like '%'+@MethodConst0+'%') 

            db.Queryable<OrderItem, Order>((i, o) => i.OrderId == o.Id)
                .Where(i => i.OrderId != 0)
                .Select("i.*").ToList();
            //SELECT i.* FROM [OrderDetail] i  ,[Order]  o  WHERE ( [i].[OrderId] = [o].[Id] )  AND ( [i].[OrderId] <> @OrderId0 )  AND  ([o].[Name] like '%'+@MethodConst1+'%')

            //no filter
            db.Queryable<Order>().Filter(null, false).ToList();
            //SELECT [Id],[Name],[Price],[CreateTime],[CustomId] FROM [Order]
        }


        private static void NameFilterDemo()
        {
            var db2 = GetInstance();
            db2.QueryFilter.Add(new SqlFilterItem()
            {
                FilterName = "Myfilter1",
                FilterValue = it =>
                {
                    //Writable logic
                    return new SqlFilterResult() { Sql = " name like '%a%' " };
                },
                IsJoinQuery = false // single query
            });
            db2.QueryFilter.Add(new SqlFilterItem()
            {
                FilterName = "Myfilter1",
                FilterValue = it =>
                {
                    //Writable logic
                    return new SqlFilterResult() { Sql = " o.name like '%a%' " };
                },
                IsJoinQuery = true //join query
            });

            db2.Queryable<Order>()
                             .Where(it => it.Name == "jack")
                             .Filter("Myfilter1")
                             //IF .Filter("Myfilter",false)  only execute Myfilter
                             .ToList();
            //SELECT [Id],[Name],[Price],[CreateTime],[CustomId] FROM [Order] 
            //WHERE ( [Name] = 'jack' )  AND  name like '%a%'


            db2.Queryable<Order, OrderItem>((o, i) => o.Id == i.OrderId)
                          .Where(o => o.Name == "jack")
                          .Filter("Myfilter1")
                          .Select(o => o)
                          .ToList();
            //SELECT o.* FROM[Order] o, [OrderDetail]  i WHERE ( [o].[Id] = [i].[OrderId])  
            //AND([o].[Name] = 'jack')  AND o.name like '%a%'

            //no filter
            db2.Queryable<Order>().Filter(null, false).ToList();
            //SELECT [Id],[Name],[Price],[CreateTime],[CustomId] FROM [Order]
        }


        public static SqlSugarClient GetInstance()
        {
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig() { DbType = DbType.SqlServer, ConnectionString = Config.ConnectionString, IsAutoCloseConnection = true });
            db.Aop.OnLogExecuted = (sql, p) =>
            {
                Console.WriteLine(sql);
            };
            return db;
        }
    }
}
