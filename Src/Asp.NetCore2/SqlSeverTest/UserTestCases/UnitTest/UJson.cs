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
            list= Db.Queryable<UnitJsonTest>().ToList();
            UValidate.Check("order2", list.First().Order.Name, "Json");
            var list2 = Db.Queryable<UnitJsonTest>().ToList();
            Db.Updateable<UnitJsonTest>()
                .SetColumns(it => it.Order, new Order() { Name = "abc" })
                .Where(it => true)
                .ExecuteCommand();
            var name=Db.Queryable<UnitJsonTest>().First().Order.Name;
            if (name != "abc") throw new Exception("unit error");
        }
    }


    public class UnitJsonTest
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        [SqlSugar.SugarColumn(ColumnDataType = "varchar(max)", IsJson = true)]
        public Order Order { get; set; }
    }
}
