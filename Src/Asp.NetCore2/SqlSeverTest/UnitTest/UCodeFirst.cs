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
            Db.CodeFirst.InitTables<UnitCodeFirstpks2>();
            if (Db.DbMaintenance.IsAnyTable("UnitCodeTest1", false))
                Db.DbMaintenance.DropTable("UnitCodeTest1");
            Db.CodeFirst.InitTables<UnitCodeTest1>();
            Db.CodeFirst.InitTables<UnitCodeFirstpks2>();
            if (Db.DbMaintenance.IsAnyTable("UnitCodeFirst131", false))
                Db.DbMaintenance.DropTable("UnitCodeFirst131");
            Db.CodeFirst.InitTables<UnitCodeFirst131>();
            Db.Insertable(new UnitCodeFirst131() { Id = 1 }).ExecuteCommand();
            Db.CodeFirst.InitTables<UNITCODEFIRST131>();
            Db.CodeFirst.InitTables<UNITCOdEFIRST131>();
            Db.CodeFirst.InitTables<UnitIndextest>();
            Db.CodeFirst.InitTables<UnitXXXYYYZZZ>();
            Db.CodeFirst.InitTables<unitxxxyyyzzz>();
        }

        public class UnitXXXYYYZZZ
        {
            public string Id { get; set; }
            [SqlSugar.SugarColumn(DefaultValue ="")]
            public string Id2 { get; set; }
        }
        public class unitxxxyyyzzz
        {
            public string Id { get; set; } 
        }

        [SqlSugar.SugarIndex("UnitIndextestIndex", nameof(UnitIndextest.Table),SqlSugar.OrderByType.Asc)]
        public class UnitIndextest
        {
            public string Table { get; set; }
            public string Id { get; set; }
        }
        public class UnitCodeFirst131 
        {
            public int Id { get; set; } 
        }
        public class UNITCODEFIRST131
        {
            public int Id { get; set; }
            [SqlSugar.SugarColumn(DefaultValue ="a")]
            public string Name { get; set; }
        }
        public class UNITCOdEFIRST131
        {
            public int Id { get; set; }
            [SqlSugar.SugarColumn(DefaultValue = "a")]
            public string Name { get; set; }
            [SqlSugar.SugarColumn(DefaultValue = "getdate()")]
            public DateTime dt { get; set; }
        }
        public class UnitCodeFirstpks2
        {
            [SqlSugar.SugarColumn(IsPrimaryKey =true)]
            public string id { get; set; }
            [SqlSugar.SugarColumn(IsPrimaryKey = true)]
            public string id2 { get; set; }
        }
        public class UnitCodeTest1
        {
            [SqlSugar.SugarColumn(IndexGroupNameList = new string[] { "group1" })]
            public int Id { get; set; }
            [SqlSugar.SugarColumn(DefaultValue="getdate()", IndexGroupNameList =new string[] {"group1" } )]
            public DateTime? CreateDate { get; set; }
        }
    }
}
