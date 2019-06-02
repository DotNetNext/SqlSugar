using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public partial class NewUnitTest
    {
        public static void Queryable()
        {

            var pageindex = 1;
            var pagesize = 10;
            var total = 0;
            var totalPage = 0;
            var list = Db.Queryable<Order>().ToPageList(pageindex, pagesize, ref total, ref totalPage);

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

            var sql = Db.Queryable<UnitSelectTest>().Select(it => new UnitSelectTest()
            {

                DcNull = it.Dc,
                Dc = it.Int
            }).ToSql().Key;
            UValidate.Check(sql, "SELECT  [Dc] AS [DcNull] , [Int] AS [Dc]  FROM [UnitSelectTest]", "Queryable");

            sql = Db.Updateable<UnitSelectTest2>(new UnitSelectTest2()).ToSql().Key;
            UValidate.Check(sql, @"UPDATE [UnitSelectTest2]  SET
           [Dc]=@Dc,[IntNull]=@IntNull  WHERE [Int]=@Int", "Queryable");

            sql = Db.Queryable<Order>().IgnoreColumns(it => it.CreateTime).ToSql().Key;
            UValidate.Check(sql, "SELECT [Id],[Name],[Price],[CustomId] FROM [Order] ", "Queryable");
            sql = Db.Queryable<Order>().IgnoreColumns(it => new { it.Id, it.Name }).ToSql().Key;
            UValidate.Check(sql, "SELECT [Price],[CreateTime],[CustomId] FROM [Order] ", "Queryable");
            sql = Db.Queryable<Order>().IgnoreColumns("id").ToSql().Key;
            UValidate.Check(sql, "SELECT [Name],[Price],[CreateTime],[CustomId] FROM [Order] ", "Queryable");

            var cts = IEnumerbleContains.Data();
            var list2=Db.Queryable<Order>()
                    .Where(p => /*ids.*/cts.Select(c => c.Id).Contains(p.Id)).ToList();

            var cts2 = IEnumerbleContains.Data().ToList(); ;
            var list3 = Db.Queryable<Order>()
                    .Where(p => /*ids.*/cts2.Select(c => c.Id).Contains(p.Id)).ToList();


            var list4 = Db.Queryable<Order>()
                .Where(p => new List<int> { 1, 2, 3 }.Where(b => b > 1).Contains(p.Id)).ToList();

            Db.CodeFirst.InitTables<UnitTest3>();
            var list5 = Db.Queryable<UnitTest3>().Where(it => SqlSugar.SqlFunc.ToString(it.Date.Value.Year) == "1").ToList();
            var list6 = Db.Queryable<UnitTest3>().Where(it => it.Date.Value.Year ==1).ToList();
            var list7 = Db.Queryable<UnitTest3>().Where(it => it.Date.Value.Date==DateTime.Now.Date).ToList();
        }

        public static class IEnumerbleContains
        {
            public static IEnumerable<Order> Data()
            {
                for (int i = 0; i < 100; i++)
                {
                    yield return new Order
                    {
                        Id = i,
                    };
                }
            }
        }

        public class UnitTest3
        {
            public DateTime? Date { get; set; }
        }


        public class UnitSelectTest2
        {
            [SqlSugar.SugarColumn(IsOnlyIgnoreUpdate = true)]
            public decimal? DcNull { get; set; }
            public decimal Dc { get; set; }
            public int? IntNull { get; set; }
            [SqlSugar.SugarColumn(IsPrimaryKey = true)]
            public decimal Int { get; set; }
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
