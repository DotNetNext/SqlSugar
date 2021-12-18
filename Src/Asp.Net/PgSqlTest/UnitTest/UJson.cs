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
        }
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
