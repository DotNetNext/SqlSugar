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
            Db.CodeFirst.InitTables<UnitCodeTest111>();
            Db.CodeFirst.InitTables<UnitIndextest>();
        }
        [SqlSugar.SugarIndex("UnitIndextestIndex", nameof(UnitIndextest.Table), SqlSugar.OrderByType.Asc)]
        public class UnitIndextest
        {
            public string Table { get; set; }
            public string Id { get; set; }
        }
        public class UnitCodeTest1
        {
            [SqlSugar.SugarColumn(IndexGroupNameList = new string[] { "group1" })]
            public int Id { get; set; }
            [SqlSugar.SugarColumn(DefaultValue="now()", IndexGroupNameList =new string[] {"group1" } )]
            public DateTime? CreateDate { get; set; }
        }
        [SqlSugar.SugarIndex("UCTINDEX",
                      nameof(UnitCodeTest111.CreateDate),SqlSugar.OrderByType.Desc,
                      nameof(UnitCodeTest111.CreateDate2), SqlSugar.OrderByType.Desc,
                      nameof(UnitCodeTest111.CreateDate3), SqlSugar.OrderByType.Desc
            )]
        public class UnitCodeTest111
        {
 
            public int Id { get; set; }
          
            public DateTime? CreateDate { get; set; }
            public DateTime? CreateDate2 { get; set; }
            public DateTime? CreateDate3 { get; set; }
        }
    }
}
