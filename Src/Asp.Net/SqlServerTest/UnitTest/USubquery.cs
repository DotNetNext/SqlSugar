using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace OrmTest 
{
    public partial class NewUnitTest
    {
        public static void SubQueryTest() 
        {
           var sql= Db.Queryable<Order>().Where(it => SqlFunc.Subqueryable<OrderItem>().Where(s => s.OrderId == it.Id).Any()).ToSql();
            if (sql.Key != "SELECT [Id],[Name],[Price],[CreateTime],[CustomId] FROM [Order] it  WHERE (EXISTS ( SELECT * FROM [OrderDetail] WHERE ( [OrderId] = [it].[Id] ) ))") 
            {
                throw new Exception("unit error");
            }

            sql = Db.Queryable<Order>().Select(it => new
            {
                ItemId = SqlFunc.Subqueryable<OrderItem>().Where(s => s.OrderId == it.Id).Select(s => s.ItemId)
            }).ToSql();
            if (sql.Key != "SELECT  (SELECT TOP 1 [ItemId] FROM [OrderDetail] WHERE ( [OrderId] = [it].[Id] )) AS [ItemId]  FROM [Order] it ")
            {
                throw new Exception("unit error");
            }
        }
    }
}
