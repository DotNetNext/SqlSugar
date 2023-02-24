﻿using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
            db.QueryFilter.AddTableFilter<Order>(it => it.Name.Contains("a"));
            db.Queryable<Order>().ToList();

            //dynamic
            Expression<Func<Order, bool>> dynamicExpression = it => it.Name=="b";//动态构造这种表达式
            Expression exp = dynamicExpression;
            Type type = typeof(Order);
            db.QueryFilter.AddTableFilter(type,exp);
            db.Queryable<Order>().ToList();

            //Clear & Restore
            db.QueryFilter.ClearAndBackup();
            db.Queryable<Order>().ToList();
            db.QueryFilter.Restore();
            db.Queryable<Order>().ToList();

            db.Queryable<object>().AS("[Order]").Filter(typeof(Order)).ToList();
            //SELECT [Id],[Name],[Price],[CreateTime],[CustomId] FROM [Order]  WHERE  ([Name] like '%'+@MethodConst0+'%') 

            db.Queryable<object>().AS("[OrderDetail]").Filter(typeof(OrderItem)).ToList();
            //no filter

            //delete Filter
            db.Deleteable<Order>().EnableQueryFilter().Where(it=>it.Id==1).ExecuteCommand();

            db.Queryable<OrderItem, Order>((i, o) => i.OrderId == o.Id)
                .Where(i => i.OrderId != 0)
                .Select("i.*").ToList();
            //SELECT i.* FROM [OrderDetail] i  ,[Order]  o  WHERE ( [i].[OrderId] = [o].[Id] )  AND ( [i].[OrderId] <> @OrderId0 )  AND  ([o].[Name] like '%'+@MethodConst1+'%')

            //no filter
            db.Queryable<Order>().Filter(null, false).ToList();
            //SELECT [Id],[Name],[Price],[CreateTime],[CustomId] FROM [Order]

            db.Queryable<OrderItem>().LeftJoin<Order>((x, y) => x.ItemId == y.Id).ToList();

            db.QueryFilter.Add(new SqlFilterItem()
            {
                FilterName = "Myfilter1",
                FilterValue = it =>
                {
                    //Writable logic
                    return new SqlFilterResult() { Sql = " name like '%a%' " };
                },
                IsJoinQuery = false // single query
            });
            db.Queryable<Order>().Select(x=>
            new { 
               id=SqlFunc.Subqueryable<Order>().EnableTableFilter().Where(z=>true).Select(z=>z.Id)
            }
            ).ToList();

            db.QueryFilter.Clear();
            db.QueryFilter.AddTableFilter<Order>(it => it.Name.Contains("a"));
            db.QueryFilter.AddTableFilter<Custom>(it => it.Name.Contains("a"));
            db.Queryable<Order>().LeftJoin<Order>((x, y) => x.Id == y.Id).Where(x=>x.Id==1).ToList();
            db.Queryable<Order>().LeftJoin<Order>((x, y) => x.Id == y.Id).ToList();
            db.Queryable<Order>().LeftJoin<Custom>((x, y) => x.Id == y.Id).ToList();
            db.Queryable<Order>()
                .LeftJoin<Custom>((x, y) => x.Id == y.Id)
                .LeftJoin<Custom>((x, y,z) => x.Id == y.Id).ToList();
          
            db.Deleteable<Order>().Where(it => it.Id == 1)
            .EnableQueryFilter().ExecuteCommand();
            db.Updateable<Order>()
                .EnableQueryFilter().SetColumns(it => it.Name=="a1")

                .Where(it => true).ExecuteCommand();
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
                Console.WriteLine(string.Join(",",p.Select(it=>it.ParameterName+":"+it.Value)));
                Console.WriteLine();
            };
            return db;
        }
    }
}
