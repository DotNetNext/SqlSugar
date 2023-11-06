using OrmTest.UnitTest.Models;
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


      

            db.CodeFirst.InitTables<MYOrder>();//建表

            List<MYOrder> orders = new List<MYOrder>
            {
                new MYOrder
                {
                    Name = "123",
                    Price =Convert.ToDecimal(10)
                }
            };
            db.Insertable(orders).ExecuteCommand();
            var x1 = db.Storageable(orders).WhereColumns(m => m.Price).ToStorage();
            Console.WriteLine("insert:" + x1.InsertList.Count);
            Console.WriteLine("update:" + x1.UpdateList.Count);
            Console.WriteLine("------------");
            x1.AsInsertable.ExecuteCommand();
            Check.Exception( x1.AsUpdateable.ExecuteCommand()==0,"unit errors");
        }
       
    }
}
