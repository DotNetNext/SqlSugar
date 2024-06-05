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
            Db.CodeFirst.InitTables<UnitArrayTestaa>();
            Db.Updateable(new UnitArrayTestaa() { Name="a", ids=new int[] { 1,2} }).IgnoreColumns(true).ExecuteCommand();
            Db.Queryable<UnitArrayTestaa>()
                .OrderBy(it=>it.Id)
                .Where(it => SqlFunc.JsonArrayAny(it.ids, 1)).ToList();

            Db.CodeFirst.InitTables<UnitArrayLongtest1>();
            Db.DbMaintenance.TruncateTable<UnitArrayLongtest1>();
            Db.Insertable(new UnitArrayLongtest1()
            { 
                ids = new int[] { 1, 2 },
                Name="a"
            }).ExecuteCommand(); 
            var x=Db.Queryable<UnitArrayLongtest1>()
                .Where(it => SqlFunc.PgsqlArrayContains(it.ids , 1))
                .ToList();
        }
    }

    public class UnitArrayTestaa
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string Name { get; set;}
        [SqlSugar.SugarColumn(IsJson =true)]
        public int[] ids { get; set; }
    }
    public class UnitArrayLongtest1
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        [SqlSugar.SugarColumn(IsArray = true,ColumnDataType ="int4[]")]
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
