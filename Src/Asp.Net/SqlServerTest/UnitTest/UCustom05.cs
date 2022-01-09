using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest 
{
    public class UCustom05
    {
        public static void Init() 
        {
            
            var db = NewUnitTest.Db;
            var id=db.Insertable(new Order() { CreateTime = DateTime.Now, Name = "a", Price =Convert.ToDecimal( 111.00) , CustomId = 1 }).ExecuteReturnIdentity();
            var data = db.GetSimpleClient<Order>().GetById(id);
            var x=db.Storageable(data).WhereColumns(it=>it.Price).ToStorage();
            Check.Exception(x.UpdateList.Count == 0, "unit error");
        }
       
    }
}
