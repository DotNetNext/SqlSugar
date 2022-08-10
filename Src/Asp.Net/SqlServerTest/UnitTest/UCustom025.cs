using IWMS.Bill.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace OrmTest 
{
    public class UCustom025
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            var cnt =   db.Queryable<ImsBillMst, ImsBillDtl>((m, d) => m.Id == d.MstId)
                              .Where((m, d) => m.Deleted == "N" && m.Code == "")
                              .Select((m, d) => new
                              {
                                  TT_REQ_QTY = d.Qty,
                                  KIT_QTY = SqlFunc.IsNull(SqlFunc.Subqueryable<ImsBillDtlStk>().InnerJoin<ImsBillStk>((ds, sk) => ds.StkId == sk.Id)
                                                          .Where((ds, sk) => ds.DtlId == d.Id && ds.Deleted == "N" && sk.Deleted == "N")
                                                          .Select((ds, sk) => SqlFunc.AggregateSum(sk.ActualQty)), 0),
                              })
                              .MergeTable()
                              .Where(t => t.TT_REQ_QTY > t.KIT_QTY)
                              .ToSql().Key;
            if (!cnt.Contains("SUM([sk].[ACTUAL_QTY])")) 
            {
                throw new Exception("unit error");
            }
        }
    }
}
