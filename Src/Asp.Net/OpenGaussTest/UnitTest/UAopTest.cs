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

                    e.DbTableName = "public." + e.DbTableName;
                }
            };
            db.Queryable<Order>().Select(it=> new { 
               x=SqlSugar.SqlFunc.Subqueryable<Order>().Select(s=>s.Id)
            }).ToList();
            db.CurrentConnectionConfig.ConfigureExternalServices = new SqlSugar.ConfigureExternalServices();
        }
    }
}
