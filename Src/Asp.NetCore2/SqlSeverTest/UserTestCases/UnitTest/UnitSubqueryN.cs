using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrmTest
{
    internal class UnitSubqueryN
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<shouji, shouji_subnum, insurance_section_config>();
            var data = db.Queryable<shouji>()
            .Where(w => w.shouji_status >= 4 && w.shouji_guid ==
                SqlFunc.Subqueryable<shouji_subnum>().Where(s => s.amount > 0
                    && s.insurance_section_id ==
                    SqlFunc.Subqueryable<insurance_section_config>()
                    .GroupBy(i => i.insurance_section_id)
                    .Where(i => i.category_code == "" && i.insurance_section_id == s.insurance_section_id)
                    .Select(i => i.insurance_section_id)
               ).GroupBy(s => s.shouji_guid)
               .Select(s => s.shouji_guid) && w.xuqin_code == "")
            .ToList();
        }

        [SugarTable("unitshouji")]
        public class shouji

        {
            public shouji()

            {
            }

            public int? shouji_status { get; set; }

            public string shouji_guid { get; set; }

            public string xuqin_code { get; set; }
        }
        [SugarTable("unitshouji_subnum")]
        public class shouji_subnum
        {
            public shouji_subnum()

            {
            }

            public int? amount { get; set; }

            public int? insurance_section_id { get; set; }

            public string shouji_guid { get; set; }
        }
        [SugarTable("unitinsurance_section_config")]
        public class insurance_section_config
        {
            public insurance_section_config()

            {
            }


            public int insurance_section_id { get; set; }

            public string category_code { get; set; }
        }
    }
}
