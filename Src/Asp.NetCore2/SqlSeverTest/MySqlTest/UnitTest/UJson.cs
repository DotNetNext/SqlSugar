using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public partial class NewUnitTest
    {

        public static void Json()
        {
            Db.CodeFirst.InitTables<UnitJsonTest>();
            Db.DbMaintenance.TruncateTable<UnitJsonTest>();
            Db.Insertable(new UnitJsonTest() { Order = new Order { Id = 1, Name = "order1" } }).ExecuteCommand();
            var list = Db.Queryable<UnitJsonTest>().ToList();
            UValidate.Check("order1", list.First().Order.Name, "Json");
            Db.Updateable(new UnitJsonTest() { Id = 1, Order = new Order { Id = 2, Name = "order2" } }).ExecuteCommand();
            list = Db.Queryable<UnitJsonTest>().ToList();
            UValidate.Check("order2", list.First().Order.Name, "Json");

            Db.Updateable<UnitJsonTest>().SetColumns(x => new UnitJsonTest { Order = new Order { Id = 2, Name = "order3" } }).Where(x => x.Id == 1).ExecuteCommand();
            list = Db.Queryable<UnitJsonTest>().ToList();
            UValidate.Check("order3", list.First().Order.Name, "Json");

            var list2 = Db.Queryable<UnitJsonTest>().ToList();
        }
    }


    public class UnitJsonTest
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        [SqlSugar.SugarColumn(ColumnDataType = "varchar(4000)", IsJson = true)]
        public Order Order { get; set; }
    }
}
