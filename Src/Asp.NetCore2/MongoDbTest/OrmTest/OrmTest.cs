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
            db.Insertable(new   List<OrderInfo>(){
                new OrderInfo() { CreateTime = DateTime.Now, Name = "a1", Price = 2 },
                new OrderInfo() { CreateTime = DateTime.Now, Name = "a2", Price = 3 }})
        .ExecuteCommand();
        }
    }
}
