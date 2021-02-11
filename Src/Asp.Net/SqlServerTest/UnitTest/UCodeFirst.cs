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
            var db = Db;
            db.Aop.OnLogExecuting = (s, p) =>
            {
                Console.WriteLine(s);
            };
            Db.CodeFirst.InitTables<UnitCodeFirstpks3>();
        }
        [SqlSugar.SugarTable("UnitCodeFirstpks31","备注"  )]
        public class UnitCodeFirstpks3
        {
            public int id { get; set; }
            public string name2 { get; set; }


 
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
