using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public partial class NewUnitTest
    {
        public static void Queryable() {

            var pageindex = 1;
            var pagesize = 10;
            var total = 0;
            var totalPage = 0;
            var list=Db.Queryable<Order>().ToPageList(pageindex, pagesize, ref total, ref totalPage);

            //Db.CodeFirst.InitTables(typeof(CarType));
            //Db.Updateable<CarType>()
            //      .SetColumns(it => new CarType { State = SqlSugar.SqlFunc.IIF(it.State == true, false, true) }).Where(it => true)
            //   .ExecuteCommand();

            //Db.CodeFirst.InitTables(typeof(TestTree));
            //Db.DbMaintenance.TruncateTable<TestTree>();
            //Db.Ado.ExecuteCommand("insert testtree values(hierarchyid::GetRoot(),geography :: STGeomFromText ('POINT(55.9271035250276 -3.29431266523898)',4326),'name')");
            //var list2 = Db.Queryable<TestTree>().ToList();

            Db.CodeFirst.InitTables<UnitGuidTable>();
            Db.Queryable<UnitGuidTable>().Where(it => it.Id.HasValue).ToList();

            Db.Queryable<Order>().Where(it => SqlSugar.SqlFunc.Equals(it.CreateTime.Date, it.CreateTime.Date)).ToList();

           var sql= Db.Queryable<UnitSelectTest>().Select(it => new UnitSelectTest()
            {
             
               DcNull=it.Dc,
               Dc=it.Int
            }).ToSql().Key;
            UValidate.Check(sql, "SELECT  [Dc] AS [DcNull] , [Int] AS [Dc]  FROM [UnitSelectTest]", "Queryable");
        }

        public class UnitSelectTest
        {
            public decimal? DcNull { get; set; }
            public decimal Dc { get; set; }
            public int? IntNull { get; set; }
            public decimal Int { get; set; }
        }
 
        public class UnitGuidTable
        {
            public Guid? Id { get; set; }
        }
    }
}
