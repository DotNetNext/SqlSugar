using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public partial class NewUnitTest
    {
       
        public static void AopTest() 
        {
            var db = Db;
            db.CurrentConnectionConfig.ConfigureExternalServices = new SqlSugar.ConfigureExternalServices()
            {

                EntityNameService = (t, e) => {

                    e.DbTableName = "dbo." + e.DbTableName;
                }
            };
            db.Queryable<Order>().Select(it=> new { 
               x=SqlSugar.SqlFunc.Subqueryable<Order>().Select(s=>s.Id)
            }).ToList();
            db.CurrentConnectionConfig.ConfigureExternalServices = new SqlSugar.ConfigureExternalServices();

            db.Aop.DataExecuting = (value, entityInfo) =>
            {
                if (entityInfo.PropertyName == "Price"&&entityInfo.OperationType==SqlSugar.DataFilterType.InsertByObject)
                {
                    entityInfo.SetValue(1);
                }
                if (entityInfo.PropertyName == "CreateTime" && entityInfo.OperationType == SqlSugar.DataFilterType.InsertByObject)
                {
                    entityInfo.SetValue(DateTime.Now);
                }
                if (entityInfo.PropertyName == "Price" && entityInfo.OperationType == SqlSugar.DataFilterType.UpdateByObject)
                {
                    entityInfo.SetValue(-1);
                }
            };

            var id= db.Insertable(new Order()
            {
                CustomId = 1,
                Name = "a"
            }).ExecuteReturnIdentity();
            var data = db.Queryable<Order>().InSingle(id);
            if (data.Price != 1) 
            {
                throw new Exception("Unit Aop error");
            }
            db.Updateable(data).ExecuteCommand();
            if (data.Price != -1)
            {
                throw new Exception("Unit Aop error");
            }
        }
    }
}
