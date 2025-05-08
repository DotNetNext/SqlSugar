using MongoDbTest.DBHelper;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbTest
{
    public class OrmTest
    {
        public static void Init()
        {
            var db = DbHelper.GetNewDb();
            db.Insertable(new OrderInfo() { CreateTime = DateTime.Now, Name = "a", Price = 1 })
            .ExecuteCommand();
            var ids = db.Insertable(new List<OrderInfo>(){
                new OrderInfo() { CreateTime = DateTime.Now, Name = "a1", Price = 2 },
                new OrderInfo() { CreateTime = DateTime.Now, Name = "a2", Price = 3 }})
           .ExecuteReturnPkList<string>();


            var updateRow1 = db.Updateable(new OrderInfo
            {
                Id = ids.First(),
                Name = "a3",
                Price = 11
            }
            )
            .ExecuteCommand();

            var updateRow2 = db.Updateable(new List<OrderInfo>() 
            {
                new OrderInfo() { Id = ids.First(),Name="a31",Price=11},
                new OrderInfo() { Id = ids.Last(),Name="a41"}
            })
           .ExecuteCommand();


            var updateRow3= db.Updateable<OrderInfo>()
             .SetColumns(it=>it.Name=="aa")
             .Where(it=>it.Id==ids.Last())
            .ExecuteCommand();

            var delrow = db.Deleteable(new OrderInfo() { Id = ids.Last() })
              .ExecuteCommand(); 

            var list = db.Queryable<OrderInfo>().ToDataTable();
             
            var list2 = db.Queryable<OrderInfo>().Where(it=>it.Name=="a3"&&it.Price==11).ToList();

            var list3= db.Queryable<OrderInfo>().Skip(1).Take(1).ToList();

            var list4 = db.Queryable<OrderInfo>().OrderByDescending(it=>it.Price).ToList();

            var list5 = db.Queryable<OrderInfo>().OrderByDescending(it => it.Price).ToList();

            var list6 = db.Queryable<OrderInfo>().OrderBy(it => new { it.Id,Name=it.Name }).ToList();

            var list7 = db.Queryable<OrderInfo>().OrderByDescending(it => new { it.Id, Name = it.Name }).ToList();

            var list8= db.Queryable<OrderInfo>().OrderBy(it => new { it.Id, Name = it.Name },OrderByType.Desc).ToList();

            var list9 = db.Queryable<OrderInfo>().OrderBy(it=>it.Name).OrderByDescending(it => it.Price).ToList();

            //测试生成SQL性能
            TestSqlBuilder(db);
        }

        private static void TestSqlBuilder(SqlSugar.SqlSugarClient db)
        {
            for (int i = 0; i < 10000; i++)
            {
                db.Insertable(new OrderInfo() { CreateTime = DateTime.Now, Name = "a", Price = 1 })
                 .ToSql();
            }
        }
    }
}
