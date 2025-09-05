using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OrmTest.Unitaadfas1;
using SqlSugar;
namespace OrmTest
{
    internal class Unitadsfasyss
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<A_TEST>();
            var TestQuery2 = db.Queryable<A_TEST>()
                            .InnerJoin<A_TEST2>((a, b) => a.a == b.a)
                            .GroupBy(a => new { a.a, bb = SqlFunc.IF(a.b == "").Return(a.b).End("无") })
                            .Select(a => new { a.a, bb = SqlFunc.IF(a.b == "").Return(a.b).End("无") })
                            .ToList(); 
        }

        [SugarTable("unit_A_TEST01")]
        public partial class A_TEST
        {
            [SugarColumn(ColumnName = "a")]

            public string a { get; set; }

            [SugarColumn(ColumnName = "b")]

            public string b { set; get; }
            [SugarColumn(ColumnName = "c")]
            public int? c { get; set; }
        }



        [SugarTable("unit_A_TEST01")]
        public partial class A_TEST2
        {
            [SugarColumn(ColumnName = "a")]

            public string a { get; set; }

            [SugarColumn(ColumnName = "b")]

            public string b { set; get; }
            [SugarColumn(ColumnName = "c")]
            public int? c { get; set; }
        }
    }
}
