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
            Db.CodeFirst.InitTables<TESTA1>();
            var xx = Db.DbMaintenance.GetColumnInfosByTableName("TESTA1", false);
            Db.DbMaintenance.AddColumn("TESTA1", new DbColumnInfo()
            {
                DbColumnName = "aaa" + new Random().Next(2, 99999),
                PropertyName = "aaa" + new Random().Next(2, 99999),
                TableName = "TESTA1",
                IsNullable = true,
                DataType = "varchar",
                Length = 11
            });
            // db.Utilities.RemoveCacheAll();
            var xxxxx = Db.DbMaintenance.GetColumnInfosByTableName("TESTA1", false);
        }
        public class TESTA1
        {
            public string X { get; set; }
        }
        public class UnitCodeTest1
        {
            public int Id { get; set; }
            public DateTime? CreateDate { get; set; }
        }
    }
}
