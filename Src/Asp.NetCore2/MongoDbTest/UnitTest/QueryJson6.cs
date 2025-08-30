using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbTest
{
    internal class QueryJson6
    {

        public static void Init() 
        {
            var db = DbHelper.GetNewDb();
            db.DbMaintenance.TruncateTable<RetailInfo>();
            db.Insertable(new RetailInfo()
            {
                RetailDate = new RetailDateInfo() { SetBeginDate = "2021-01-01", SetEndDate = "2025-01-01" }

            }).ExecuteCommand();
            db.Insertable(new RetailInfo()
            {
                RetailDate = new RetailDateInfo() { SetBeginDate = "2022-01-01", SetEndDate = "2022-01-01" }

            }).ExecuteCommand();
            db.Insertable(new RetailInfo()
            {
                RetailDate = new RetailDateInfo() { SetBeginDate = "2023-01-01", SetEndDate="2023-01-01" }

            }).ExecuteCommand();

            var datas = db.Queryable<RetailInfo>()
                .Where(m => Convert.ToDateTime(m.RetailDate.SetBeginDate).Year == 2021 ).ToList();
            if (datas.Count!=1||datas.First().RetailDate.SetBeginDate != "2021-01-01") Cases.ThrowUnitError();

            var datas2 = db.Queryable<RetailInfo>()
              .Where(m => Convert.ToDateTime(m.RetailDate.SetBeginDate).Year>= 2022).ToList();
             if (datas2.Count!=2) Cases.ThrowUnitError();

            var datas3 = db.Queryable<RetailInfo>()
              .Where(m => Convert.ToDateTime(m.RetailDate.SetBeginDate).Year > 2022
                     && Convert.ToDateTime(m.RetailDate.SetEndDate).Year <= 2023).ToList();

            if (datas2.Count != 1) Cases.ThrowUnitError();
        }

        public class RetailInfo
        {
            /// <summary>
            /// 开始日期（字符串或时间戳，需要能转 DateTime）
            /// </summary>
            public string SetBeginDate { get; set; }

            /// <summary>
            /// 结束日期
            /// </summary>
            public string SetEndDate { get; set; }

            /// <summary>
            /// 零售日期，可能是个组合字段
            /// </summary>
            [SqlSugar.SugarColumn(IsJson =true)]
            public RetailDateInfo RetailDate { get; set; }
        }

        public class RetailDateInfo
        {
            public string SetBeginDate { get; set; }
            public string SetEndDate { get; set; }
        }
    }
}
