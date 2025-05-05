using MongoDbTest.DBHelper;
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


            var updateRow = db.Updateable(new List<OrderInfo>() 
            {
                new OrderInfo() { Id = ids.First(),Name="a3",Price=11},
                new OrderInfo() { Id = ids.Last(),Name="a4"}
            })
           .ExecuteCommand();

            var delrow = db.Deleteable(new OrderInfo() { Id = ids.Last() })
              .ExecuteCommand(); 

            var list = db.Queryable<OrderInfo>().ToDataTable();
             
            var list2 = db.Queryable<OrderInfo>().Where(it=>it.Name=="a3"&&it.Price==11).ToList();

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
