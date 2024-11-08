using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    internal class Unitsdfa1231
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<CnDataDTO>();
            var sql1 = db.Queryable<CnDataDTO>()
                .Distinct()
                .Select(it => new CnDataDTO()
                {
                    time = SqlFunc.ToDate(it.time)
                }, true)
                .MergeTable()
                .OrderBy(it => it.time)
                .ToList();
        }
        [SugarTable("Unit1sacndata")]
        public class CnDataDTO
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnName = "序号")]
            public int id { get; set; } 
            public string name { get; set; } 
            public string code { get; set; } 
            public string type { get; set; }
 
            public DateTime time { get; set; }
        }
    }
}
