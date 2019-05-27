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
            Db.Insertable(new JsonTest() {  Id=1, OrderX = new Order { Id = 1, Name = "order1" } }).ExecuteCommand();
            var list = Db.Queryable<JsonTest>().ToList();
            UValidate.Check("order1", list.First().OrderX.Name, "Json");
            Db.Updateable(new JsonTest() { Id = 1, OrderX = new Order { Id = 2, Name = "order2" } }).ExecuteCommand();
            list= Db.Queryable<JsonTest>().ToList();
            UValidate.Check("order2", list.First().OrderX.Name, "Json");
            var list2 = Db.Queryable<JsonTest>().ToList();
        }
    }


    public class JsonTest
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public int Id { get; set; }
        [SqlSugar.SugarColumn(ColumnDataType = "varchar2(4000)", IsJson = true)]
        public Order OrderX { get; set; }
    }
}
