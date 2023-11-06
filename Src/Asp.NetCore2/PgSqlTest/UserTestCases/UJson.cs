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
            Db.Updateable(new UnitJsonTest() { Id = Db.Queryable<UnitJsonTest>().First().Id, Order = new Order { Id = 2, Name = "order2" } }).ExecuteCommand();
            list= Db.Queryable<UnitJsonTest>().ToList();
            UValidate.Check("order2", list.First().Order.Name, "Json");
            var list2 = Db.Queryable<UnitJsonTest>().ToList();
            Db.CodeFirst.InitTables<UnitArrayTest>();
            Db.Updateable(new UnitArrayTest() { Name="a" }).IgnoreColumns(true).ExecuteCommand();
        }
    }

    public class UnitArrayTest
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string Name { get; set;}
        [SqlSugar.SugarColumn(IsArray =true)]
        public int[] ids { get; set; }
    }

    public class UnitJsonTest
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        [SqlSugar.SugarColumn(ColumnDataType = "varchar(4000)", IsJson = true)]
        public Order Order { get; set; }
    }
}
