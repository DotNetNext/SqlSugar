using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F9.DataEntity.Entity;
using SqlSugar;
namespace  OrmTest
{
    public class UCustom01
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Export>();
            db.CodeFirst.InitTables<LoadCon>();
            db.CodeFirst.InitTables<ExToCon>();
            var LclId = "FCL";
            var conno = "conno";
            var withSameCarriCarNo = db.Queryable<Export>()
                .Where(export => SqlFunc.IsNull(export.IeId, "E") == "E")
                .Where(export => SqlFunc.Subqueryable<LoadCon>().Where(loadconn =>
                                        
                SqlFunc.Subqueryable<ExToCon>().Where(extocon =>
                                             extocon.ExId == export.Id && extocon.LcId == loadconn.Id).Any())
               .WhereIF(LclId == "FCL",
                                         loadconn => SqlFunc.IsNull(loadconn.ConNo, "") == conno)
               .WhereIF(LclId != "FCL",
                                         loadconn => SqlFunc.IsNull(loadconn.ConNo, "") == conno && export.LclId == "FCL")
                                     .Any())
                .ToList();
            string p1 = "p1";
            db.Queryable<Order>().Where(x11 => x11.Name + "a" == x11.Name).ToList();
            db.Queryable<Order>().Where(x11 => x11.Name == x11.Name + "a").ToList();
            db.Queryable<Order>().Where(x11 => "a" + x11.Name + p1 == x11.Name).ToList();
            db.Queryable<Order>().Where(x11 => x11.Name == "a" + x11.Name + p1).ToList();
            db.Queryable<Order>().Where(x11 => SqlFunc.ToString("a" + p1 + x11.Name) == x11.Name).ToList();
            db.Updateable<Order>()
                .SetColumns(x => x.Name == x.Name + "a")
                .Where(z => z.Id == 1)
                .ExecuteCommand();
            db.Updateable<Order>()
              .SetColumns(x => new Order() { Name = x.Name + "a" })
              .Where(z => z.Id == 1)
              .ExecuteCommand();
        }
    }
}
