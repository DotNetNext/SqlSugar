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
            Db.CodeFirst.InitTables<JsonTest>();
            Db.DbMaintenance.TruncateTable<JsonTest>();
            Db.Insertable(new JsonTest() { Order = new Order { Id = 1, Name = "order1" } }).ExecuteCommand();
            var list = Db.Queryable<JsonTest>().ToList();
            UValidate.Check("order1", list.First().Order.Name, "Json");
            var x=Db.Updateable(new JsonTest() { Id = Db.Queryable<JsonTest>().Max(it => it.Id), Order = new Order { Id = Db.Queryable<JsonTest>().Max(it=>it.Id), Name = "order2" } }).ExecuteCommand();
            list= Db.Queryable<JsonTest>().ToList();
            UValidate.Check("order2", list.First().Order.Name, "Json");
            var list2 = Db.Queryable<JsonTest>().ToList();
        }
    }


    public class JsonTest
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        [SqlSugar.SugarColumn(ColumnDataType = "varchar(4000)", IsJson = true)]
        public Order Order { get; set; }
    }
}
