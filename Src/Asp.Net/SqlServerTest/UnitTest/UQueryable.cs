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
            //      .SetColumns(it => new CarType { State =SqlSugar.SqlFunc.IIF(it.State==true,false,true) }).Where(it=>true)
            //   .ExecuteCommand();

            //Db.CodeFirst.InitTables(typeof(TestTree));
            //Db.DbMaintenance.TruncateTable<TestTree>();
            //Db.Ado.ExecuteCommand("insert testtree values(hierarchyid::GetRoot(),'name')");
            //var list2 = Db.Queryable<TestTree>().ToList();
        }
    }
}
