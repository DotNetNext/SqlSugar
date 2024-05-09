using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class Demo6_Queue
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;
            var insertObjs = new List<Order> {
                 new Order() { Id = 11, Name = "order11", Price=0 },
                 new Order() { Id = 12, Name = "order12" , Price=0}
            };
            var insertObj = new Order() { Id = 1, Name = "order1", Price = 0 };
       
            //blukcopy
            db.Insertable(insertObjs).AddQueue();
            db.Insertable(insertObj).AddQueue();
             
            db.SaveQueues();
        }
    }
}
