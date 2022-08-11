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
        }
    }
}
