using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest 
{
    internal class UnitFilterUpdate
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            var updateObj = new Order() { Id = 1, Name = "a", CustomId = 1, CreateTime = DateTime.Now };
            db.QueryFilter.AddTableFilter<Order>(x => x.Name == "a");

            db.Updateable(updateObj)
            .PageSize(1)//需要一条一条更新才能用过滤器
            .EnableQueryFilter().ExecuteCommand();//不支持全局设置需要手动处理

            db.QueryFilter.Clear();

            db.Updateable(updateObj)
              .PageSize(1)//需要一条一条更新才能用过滤器
              .EnableQueryFilter().ExecuteCommand();//不支持全局设置需要手动处理
        }
    }
}
