using SqlSugar;
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
            var json = Db.Queryable<UnitJsonTest>().Select(it => SqlFunc.JsonField(it.Order, "Name")).ToList();
            list = Db.Queryable<UnitJsonTest>().ToList();
            UValidate.Check("order2", list.First().Order.Name, "Json");

            Db.Updateable<UnitJsonTest>().SetColumns(x => new UnitJsonTest { Order = new Order { Id = 2, Name = "order3" } }).Where(x => x.Id == 1).ExecuteCommand();
            list = Db.Queryable<UnitJsonTest>().ToList();
            UValidate.Check("order3", list.First().Order.Name, "Json");

            var db = Db;
            var list2 = Db.Queryable<UnitJsonTest>().ToList();
            db.CodeFirst.InitTables<UnitJsonArray>();
            db.DbMaintenance.TruncateTable<UnitJsonArray>();
            db.Insertable(new UnitJsonArray() { a = new int[] { 1, 2, 3 }, b = new string[] { "a", "b" } }).ExecuteCommand();
            db.Insertable(new UnitJsonArray() { a = new int[] { 5 }, b = new string[] { "c", "d" } }).ExecuteCommand();
            var isBool = db.Queryable<UnitJsonArray>().Any(it => SqlFunc.JsonArrayAny(it.a, 1));
            var isBool2 = db.Queryable<UnitJsonArray>().Any(it => SqlFunc.JsonArrayAny(it.a, 4));

            var isBool1 = db.Queryable<UnitJsonArray>().Any(it => SqlFunc.JsonArrayAny(it.b, "a"));
            var isBool22 = db.Queryable<UnitJsonArray>().Any(it => SqlFunc.JsonArrayAny(it.b, "e"));

            if (isBool == false || isBool2 == true || isBool1 == false || isBool22 == true)
            {
                throw new Exception("unit test");
            }
            db.CodeFirst.InitTables<UnitJsonTest2222>();
            db.Insertable(new UnitJsonTest2222()
            {
                A = new List<Order>() { new Order() { Id = 1, Name = "a" } }.ToList()
            }).ExecuteCommand();
            var isAny = db.Queryable<UnitJsonTest2222>().Any(it => SqlFunc.JsonListObjectAny(it.A, "Name", "a"));
            var isAny2 = db.Queryable<UnitJsonTest2222>().Any(it => SqlFunc.JsonListObjectAny(it.A, "Name", "b"));

            var isAny21 = db.Queryable<UnitJsonTest2222>().Any(it => SqlFunc.JsonListObjectAny(it.A, "Id", 1));
            var isAny22 = db.Queryable<UnitJsonTest2222>().Any(it => SqlFunc.JsonListObjectAny(it.A, "Id", 2));

            if (isAny == false || isAny21 == false || isAny2 == true || isAny22 == true)
            {
                throw new Exception("unit test");
            }
        }

        public class UnitJsonArray
        {
            [SqlSugar.SugarColumn(IsJson = true)]
            public int[] a { get; set; }
            [SqlSugar.SugarColumn(IsJson = true)]
            public string[] b { get; set; }
        }
        public class UnitJsonTest2222
        {
            [SqlSugar.SugarColumn(IsJson = true)]
            public List<Order> A { get; set; }
        }

        public class UnitJsonTest
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }
            [SqlSugar.SugarColumn(ColumnDataType = "varchar(4000)", IsJson = true)]
            public Order Order { get; set; }
        }
    }
}
