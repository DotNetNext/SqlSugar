using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public partial class NewUnitTest
    {
        public static void CodeFirst()
        {
            if (Db.DbMaintenance.IsAnyTable("UnitCodeTest1", false))
                Db.DbMaintenance.DropTable("UnitCodeTest1");
            Db.CodeFirst.InitTables<UnitCodeTest1>();
            Db.CodeFirst.InitTables<Unitasdfa1>();
            Db.Insertable(new Unitasdfa1() { t1 = 1, t2 = 1 }).ExecuteCommand();
            var x = Db.Queryable<Unitasdfa1>().ToList();
        }
        public class Unitasdfa1
        {
            [SqlSugar.SugarColumn(ColumnDataType = "number(8,2)")]
            public decimal t2 { get; set; }

            [SqlSugar.SugarColumn(ColumnDataType ="number(8,2)")]
            public decimal? t1 { get; set; }
        }
        public class UnitCodeTest1
        {
            [SqlSugar.SugarColumn(IndexGroupNameList = new string[] { "group1" })]
            public int Id { get; set; }
            [SqlSugar.SugarColumn(DefaultValue="sysdate", IndexGroupNameList =new string[] {"group1" } )]
            public DateTime? CreateDate { get; set; }
        }
    }
}
