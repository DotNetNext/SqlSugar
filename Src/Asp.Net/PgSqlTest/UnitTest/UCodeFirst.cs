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
            Db.CodeFirst.InitTables<UnitCodeTest22>();
            Db.Insertable(new UnitCodeTest22()).ExecuteCommand();
            Db.DbMaintenance.TruncateTable<UnitCodeTest22>();
            Db.CodeFirst.InitTables<UnitCodeTest3>();
            Db.Insertable(new UnitCodeTest22() {  id=1}).ExecuteCommand();
        }
 
        public class UnitCodeTest22 {
             [SqlSugar.SugarColumn(IsNullable =true)]
             public decimal? id { get; set; }
        }
        [SqlSugar.SugarTable("UnitCodeTest22")]
        public class UnitCodeTest3
        {
            [SqlSugar.SugarColumn(IsNullable = false)]
            public int id { get; set; }
        }

        public class UnitCodeTest1
        {
            [SqlSugar.SugarColumn(IndexGroupNameList = new string[] { "group1" })]
            public int Id { get; set; }
            [SqlSugar.SugarColumn(DefaultValue= "now()", IndexGroupNameList =new string[] {"group1" } )]
            public DateTime? CreateDate { get; set; }
        }
    }
}
