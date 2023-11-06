using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  OrmTest
{
    internal class USaveable
    {
        public static void Init()
        {
            List();
            T();
        }

        private static void T()
        {
            var db = NewUnitTest.Db;
            object  o1 = new   Order() { Id = 1, CreateTime = DateTime.Now, CustomId = 1, Name = "a", Price = 1  };
            db.StorageableByObject(o1).ExecuteCommand();

            object o2 = new Order() { Id = 0, CreateTime = DateTime.Now, CustomId = 1, Name = "a", Price = 1 };
            db.StorageableByObject(o2).ExecuteCommand();

        }

        private static void List()
        {
            var db = NewUnitTest.Db;
            List<object> o1 = new List<object>() { new Order() { Id = 0, CreateTime = DateTime.Now, CustomId = 1, Name = "a", Price = 1 } };
            db.StorageableByObject(o1).ExecuteCommand();
            o1 = new List<object>() { new Order() { Id = 1, CreateTime = DateTime.Now, CustomId = 1, Name = "a", Price = 1 } };
            db.StorageableByObject(o1).ExecuteCommand();

            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("insert update");

            List<object> o2 = new List<object>() { new Order() { Id = 1, CreateTime = DateTime.Now, CustomId = 1, Name = "a", Price = 1 }, new Order() { Id = 0, CreateTime = DateTime.Now, CustomId = 1, Name = "a", Price = 1 } };
            var x = db.StorageableByObject(o2).ToStorage();
            x.AsInsertable.ExecuteCommand();
            x.AsUpdateable.ExecuteCommand();

            object o = db.Queryable<Order>().ToList();
            db.StorageableByObject(o).ExecuteCommand();
        }
    }
}
