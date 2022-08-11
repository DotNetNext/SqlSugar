using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest 
{
    public class UCustom04
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            var sql = db.Queryable<Order, OrderItem, Custom>((o, i, c) => new JoinQueryInfos(
               JoinType.Left, o.Id == i.OrderId,
               JoinType.Left, o.CustomId == c.Id
           ))
           .Select<ViewModel>().ToSql().Key;
            UValidate.Check(sql, @"SELECT o.[Name] AS [Name],o.[Price] AS [Price],i.[OrderId] AS [OrderItemOrderId],i.[Price] AS [OrderItemPrice],c.[Name] AS [CustomName] FROM [Order] o Left JOIN [OrderDetail] i ON ( [o].[Id] = [i].[OrderId] )  Left JOIN [Custom] c ON ( [o].[CustomId] = [c].[Id] )","unit");
            var list =
                db.Queryable<Order, OrderItem, Custom>((o, i, c) => new JoinQueryInfos(
               JoinType.Left, o.Id == i.OrderId,
               JoinType.Left, o.CustomId == c.Id
           ))
           .Where((o,i,c)=>i.ItemId>0&&c.Id>0)
           .Select<ViewModel>().ToList();
        }
        public class ViewModel
        {
            public string Name { get; set; }
            public decimal Price { get; set; }
            public int OrderItemOrderId { get; set; }
            public int OrderItemPrice { get; set; }
            public string CustomName { get; set; }
        }
    }
}
