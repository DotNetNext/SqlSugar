using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OrmTest
{
    public class Unitadfasyya
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<TB1, TB2>();
            var sql = db.Queryable<TB1>().AS("UNIT_AHWY_TB1")
                .InnerJoinIF<TB2>(false, (a, s) => 1 == 1)
                .InnerJoinIF<TB2>(true, (a,   t) => 1 == 1)
                .ToList();

            var sql1 = db.Queryable<TB1>()
               .InnerJoinIF<TB2>(false, (a, s) => 1 == 1)
               .InnerJoinIF<TB2>(true, (a, t) => 1 == 1)
               .ToList();

            var sql2 = db.Queryable<TB1>().AS("UNIT_AHWY_TB1")
                  .LeftJoinIF<TB2>(false, (a, s) => 1 == 1)
                  .LeftJoinIF<TB2>(true, (a, t) => 1 == 1)
                  .ToList();

            var sql22 = db.Queryable<TB1>() 
               .LeftJoinIF<TB2>(false, (a, s) => 1 == 1)
               .LeftJoinIF<TB2>(true, (a, t) => 1 == 1)
               .ToList();
        }
        [SugarTable("unit_ahwy_tb1")]

        public class TB1

        {

            public string code { get; set; }

            public string name { get; set; }

            public DateTime Addtime { get; set; }

        }

        [SugarTable("unit_ahwy_tb2")]

        public class TB2

        {

            public string code { get; set; }

            public string name { get; set; }

            public DateTime Addtime { get; set; }

        }
    }
}
