using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KdbndpTest.OracleDemo.UnitTest;

namespace OrmTest
{
    public partial class NewUnitTest
    {

        public static void Json()
        {
            var db = Db;
            if (db.CurrentConnectionConfig.ConnectionString.Contains("59322")) 
            {
                db.CurrentConnectionConfig.MoreSettings = new SqlSugar.ConnMoreSettings()
                {
                    DatabaseModel = SqlSugar.DbType.MySql
                };
            }
            db.CodeFirst.InitTables<UnitJsonTest>();
            db.DbMaintenance.TruncateTable<UnitJsonTest>();
            db.Insertable(new UnitJsonTest() { Order = new Order { Id = 1, Name = "order1" } }).ExecuteCommand();
            var list = db.Queryable<UnitJsonTest>().ToList();
            UValidate.Check("order1", list.First().Order.Name, "Json");
            db.Updateable(new UnitJsonTest() { Id = Db.Queryable<UnitJsonTest>().First().Id, Order = new Order { Id = 2, Name = "order2" } }).ExecuteCommand();
            list= db.Queryable<UnitJsonTest>().ToList();
            UValidate.Check("order2", list.First().Order.Name, "Json");
            var list2 = db.Queryable<UnitJsonTest>().ToList();
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
