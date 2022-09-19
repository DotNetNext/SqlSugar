using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
  
    public class UExp
    {
        public static void Init() 
        {
            new UExp().Test();
        }
        private readonly ITestExo order;
        public UExp() 
        {
            order = new TestExo();
        }

        public void Test() 
        {
            var list = NewUnitTest.Db.Queryable<Order>().Where(it => it.Id == order.GetCurrentOrder.Id).ToList();
        }

    }
    public class TestExo : ITestExo
    {

        public Order GetCurrentOrder
        {
            get
            {
                return new Order();
            }
        }
    }
}
