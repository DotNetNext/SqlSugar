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
         //   db.Aop.OnLogExecuting = null;
            var test1 = db.Queryable<Order>().Select(it => new myDTO
            {
                Id=it.Id,
                Name=it.Name,
                disCount = SqlFunc.Subqueryable<Order>().Where(s=>s.Name ==it.Name).Where(s=>s.Id==it.Id).ToList()
            })
            .ToList();

            var test2 = db.Queryable<Order>().Where(it=>it.Id>0).Select(it => new  
            {
                Id = it.Id,
                Name = it.Name,
                disCount = SqlFunc.Subqueryable<OrderItem>().Where(s => s.OrderId==it.Id).ToList()
            })
           .ToList();

            var test21 = db.Queryable<Order>().Where(it => it.Id > 0).Select(it => new
            {
                Id = it.Id,
                Name = it.Name,
                disCount = SqlFunc.Subqueryable<OrderItem>().Where(s => s.OrderId == it.Id).ToList()
            })
            .ToListAsync().GetAwaiter().GetResult();
        }

        internal class myDTO
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public List<Order> disCount { get; set; }
        }
    }
}
