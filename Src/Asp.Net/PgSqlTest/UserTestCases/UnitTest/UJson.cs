using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
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
            Db.CodeFirst.InitTables<UnitArray2>();
            Db.Insertable(new UnitArray2() { MenuIds = new float[] { 1, 2 } }).ExecuteCommand();
            var x=Db.Queryable<UnitArray2>().ToList();
            Db.CodeFirst.InitTables<UnitArray311>();
            Db.Insertable(new UnitArray311()
            {
                 Text=new string[] {"a","a" }

            }).ExecuteCommand();
            Db.Updateable(new List<UnitArray311> {
            new UnitArray311()
            {
                Text = new string[] { "a12", "a2" },
                Id=1

            },
                  new UnitArray311()
            {
                Text = new string[] { "a1", "a1" },
                Id=2

            }
            }).ExecuteCommand();
            var xxx = Db.Queryable<UnitArray311>().ToList();

            Db.CodeFirst.InitTables<UnitUUID1XX>();
            Db.Insertable(new UnitUUID1XX() { ID = Guid.NewGuid() }).ExecuteCommand();
            var x1 = Db.Queryable<UnitUUID1XX>().ToList();
            Db.CodeFirst.InitTables<Unitaaar>();
            Db.Insertable(new Unitaaar() { arr = null }).ExecuteCommand();

            var order = new List<Order>() { new Order() { Id = 1 } };
            var db = Db;
            db.CodeFirst.InitTables<UnitJsonTestadsga1>();
            db.Insertable(new UnitJsonTestadsga1() { os = new List<Order>() }).ExecuteCommand();
            db.
                Updateable<UnitJsonTestadsga1>()
                .SetColumns(it => it.os == order)
                .Where(it => true)
                .ExecuteCommand();
            db.
             Updateable<UnitJsonTestadsga1>()
             .SetColumns(it => new UnitJsonTestadsga1() { os = order })
             .Where(it => true)
             .ExecuteCommand();

            db.CodeFirst.InitTables<Unitaaar2>();
            db.Insertable(new Unitaaar2() { arr = new string[] { "a", "c" } }).ExecuteCommand();
            var list14 = db.Queryable<Unitaaar2>()
                .Select(it=>new { 
                 x=SqlFunc.JsonIndex(it.arr, 0),
                 x2 = SqlFunc.JsonIndex(it.arr, 1)
                }).ToList();
            if (list14.First().x != "a"&& list14.Last().x != "c") 
            {
                throw new Exception("unit error");
            }
            db.CodeFirst.InitTables<UnitJsonTestAlter>();
            db.Insertable(new UnitJsonTestAlter() { Order1 = new Order() { } }).ExecuteCommand();
            db.CodeFirst.InitTables<UnitJsonTestAlter>();
            db.DbMaintenance.DropTable<UnitJsonTestAlter>();
        }
    }
    [SugarTable("UnitJsonTest_a7aaasss")]
    public class UnitJsonTestAlter  
    {
        [SqlSugar.SugarColumn(IsPrimaryKey =true,IsIdentity =true)]
        public int id { get; set; }
        [SugarColumn(IsJson = true,DefaultValue ="cast('{}' as json )")]
        public Order Order1 { get; set; }
    }
    [SugarTable("UnitJsonTest_a7aaasss")]
    public class UnitJsonTestAlteraaa
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int id { get; set; }
        [SugarColumn(IsJson = true, DefaultValue = "cast('{}' as json )")]
        public Order Order1 { get; set; }
        [SugarColumn(IsJson = true, DefaultValue = "cast('{}' as json )")]
        public Order Order2 { get; set; }
    }
    public class Unitaaar2
    {
        [SugarColumn(IsJson = true, IsNullable = true)]
        public string[] arr { get; set; }
    }
    public class UnitJsonTestadsga1
    {
        [SqlSugar.SugarColumn( IsJson = true)]
        public List<Order> os { get; set; }
    }
  
    public class Unitaaar 
    {
        [SugarColumn(ColumnDataType = "text []", IsArray = true, IsNullable = true)]
        public string[] arr { get; set; }
    }
    public class UnitUUID1XX
    {
        [SugarColumn(ColumnDataType ="uuid")]
        public Guid ID { get; set; }
    }
    public class UnitArray311 
    {
        [SugarColumn(IsArray =true,ColumnDataType ="text []" )]
        public string[] Text { get; set; }
        [SugarColumn(IsPrimaryKey =true,IsIdentity =true)]
        public int Id { get; set; }
    }
    public class UnitArray2 
    {
        [SugarColumn(ColumnDataType = "real []", IsArray = true)]
        public float[] MenuIds { get; set; }
    }
    public class UnitJsonTest
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        [SqlSugar.SugarColumn(ColumnDataType = "varchar(4000)", IsJson = true)]
        public Order Order { get; set; }
    }
}
