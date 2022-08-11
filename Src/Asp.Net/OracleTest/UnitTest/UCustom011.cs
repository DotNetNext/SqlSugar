using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class UCustom011
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;
            db.Queryable<Order>().Where(it => (it.CreateTime - SqlFunc.GetDate()).TotalDays > 1).ToList();
            var x1=db.Queryable<Order>().Select(it => (SqlFunc.GetDate() - SqlFunc.GetDate().AddYears(-1)).TotalDays).ToList();
            var x2 = db.Queryable<Order>().Select(it => (SqlFunc.GetDate() - SqlFunc.GetDate().AddDays(-1)).TotalHours).ToList();
            var x3 = db.Queryable<Order>().Select(it => (SqlFunc.GetDate() - SqlFunc.GetDate().AddHours(-1)).TotalMinutes).ToList();
            var x4 = db.Queryable<Order>().Select(it => (SqlFunc.GetDate() - SqlFunc.GetDate().AddMinutes(-1)).TotalSeconds).ToList();
            if (x1.Any()) 
            {
                Check.Exception(x1.First()!=365, "unit error . UCustom011");
                Check.Exception(x2.First() != 24, "unit error . UCustom011");
                Check.Exception(x3.First() != 60, "unit error . UCustom011");
                Check.Exception(!(x4.First() >=60&& x4.First() <= 61), "unit error . UCustom011");
            }
        }
     
     
    }
}
