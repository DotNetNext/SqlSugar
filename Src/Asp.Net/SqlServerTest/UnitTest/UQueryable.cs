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

            Db.CodeFirst.InitTables<GuidTable>();
            Db.Queryable<GuidTable>().Where(it => it.Id.HasValue).ToList();

            Db.Queryable<Order>().Where(it => SqlSugar.SqlFunc.Equals(it.CreateTime.Date, it.CreateTime.Date)).ToList();
        }


        public class GuidTable
        {
            public Guid? Id { get; set; }
        }
    }
}
