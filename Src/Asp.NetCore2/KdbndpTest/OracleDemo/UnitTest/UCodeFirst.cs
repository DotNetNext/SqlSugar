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
        public static void CodeFirst()
        {
            if (Db.DbMaintenance.IsAnyTable("UnitCodeTest1", false))
                Db.DbMaintenance.DropTable("UnitCodeTest1");
            Db.CodeFirst.InitTables<UnitCodeTest1>();
            Db.CodeFirst.InitTables<UnitCodeTest1>();
        }
        [SugarIndex("IndexUnitCodeTest1_CreateDate", nameof(CreateDate),OrderByType.Desc)]
        public class UnitCodeTest1
        {
            public int Id { get; set; }
            [SqlSugar.SugarColumn(DefaultValue= "now()"  )]
            public DateTime? CreateDate { get; set; }
        }
    }
}
