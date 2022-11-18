using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class UCustom20
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;
            db.DbMaintenance.TruncateTable<Order>();
            db.Insertable(new Order() { Id = 1, Name = "jack", CreateTime = DateTime.Now, CustomId = 1 }).ExecuteCommand();
           
            var test8 = db.Queryable<Order>()
                .LeftJoin<Order>((x, y) => x.Id == y.Id)
               .Take(2)
                .Select((x, y) => new TestDTO
                {
                    SubOne = new TestSubDTO { NameOne = false, NameTwo = x.Name }
                })
               .ToList();

            if (test8.First().SubOne.NameOne != false || test8.First().SubOne.NameTwo != "jack")
            {
                throw new Exception("unit error");
            }
            var servebillIdList = new string[] { };
            var sql= db.Queryable<BilPayment>().Where(pm1 => SqlFunc.ContainsArray(servebillIdList, pm1.ServebillId))
                         .GroupBy(pm1 => pm1.ServebillId)
                         .Select(pm1 => (object)new
                         {
                             pm1.ServebillId,
                             PmCurrencyamount = SqlFunc.AggregateSum(pm1.PmCurrencyamount),
                             PmAmount = SqlFunc.AggregateSum(pm1.PmAmount),
                             SbrAmount = SqlFunc.AggregateSum(SqlFunc.Subqueryable<BilSupplierbalancerecord>()
                                                                     .Where(x => x.IpfCode == "" && x.PmId == pm1.Id)
                                                                     .Select(x => SqlFunc.AggregateSum(SqlFunc.ToDecimal(x.SbrAmount))))
                         }).ToSqlString();
            if (!sql.Contains("`bil_payment` pm1")) 
            {
                throw new Exception("unit error");
            }
        }

        public class TestDTO
        {
            public TestSubDTO SubOne { get; set; }

            public TestSubDTO SubTwo { get; set; }
        }

        public class TestSubDTO
        {
            public bool NameOne { get; set; }

            public string NameTwo { get; set; }
        }
    }
}
