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
            if (Db.DbMaintenance.IsAnyTable("User", false))
                Db.DbMaintenance.DropTable("User");
            Db.CodeFirst.InitTables<User>();
            Db.CodeFirst.InitTables<UnitDateOffsetTimex>();
            Db.DbMaintenance.TruncateTable<UnitDateOffsetTimex>();
            Db.Insertable(new UnitDateOffsetTimex() { offsetTime = DateTimeOffset.Now }).ExecuteCommand();
            Db.Insertable(new List<UnitDateOffsetTimex>() {
                new UnitDateOffsetTimex() { offsetTime = DateTimeOffset.Now },
                new UnitDateOffsetTimex() { offsetTime = DateTimeOffset.Now }}).ExecuteCommand();
            var dt=Db.Ado.GetDataTable("select * from UnitDateOffsetTimex");
        }

        public class UnitDateOffsetTimex
        {
            public DateTimeOffset offsetTime { get; set; }
        }
        public class User
        {
            [SugarColumn(IndexGroupNameList = new string[] { "index" })]
            public int key { get; set; }
            [SugarColumn(UniqueGroupNameList = new string[] { "index" })]
            public int key2 { get; set; }
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
