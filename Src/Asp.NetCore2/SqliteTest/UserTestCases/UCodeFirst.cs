using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace OrmTest
{
    public partial class NewUnitTest
    {
        public static void CodeFirst()
        {
            if (Db.DbMaintenance.IsAnyTable("UnitCodeTest1", false))
                Db.DbMaintenance.DropTable("UnitCodeTest1");
            Db.CodeFirst.InitTables<UnitCodeTest1>();
            Db.CodeFirst.InitTables<Test00111>();
            Db.DbMaintenance.TruncateTable<Test00111>();
            Db.Insertable(new Test00111()).ExecuteCommand();
            var list = Db.Queryable<Test00111>().ToList();
            Db.CodeFirst.InitTables<Test00111121>();
            Db.CodeFirst.InitTables<UnitByteArray1>();
            Db.Insertable(new UnitByteArray1()
            {
                Data = new byte[] { 1, 2, 123, 31, 1 }
            }).ExecuteCommand();
            var list2=Db.Queryable<UnitByteArray1>().ToDataTable();
            var x=Db.DbMaintenance.GetColumnInfosByTableName("`UnitByteArray1`", false);
            if (x[0].Length != 18 && x[0].Scale != 0) throw new Exception("unit test error");
            if (x[1].Length != 18 && x[1].Scale != 2) throw new Exception("unit test error");
            if (x[2].Length != 0 && x[2].Scale != 0) throw new Exception("unit test error");
            Db.CodeFirst.InitTables<UnitIndextest>();
            Db.CodeFirst.InitTables<UnitDropColumnTest>();
            Db.CodeFirst.InitTables<UNITDROPCOLUMNTEST>();
            var column= Db.DbMaintenance.GetColumnInfosByTableName("UNITDROPCOLUMNTEST", false);
            if (column.Count != 3) 
            {
                throw new Exception("unit error");
            }
          
            var db = Db;
            db.CurrentConnectionConfig.MoreSettings = new ConnMoreSettings()
            {
                 SqliteCodeFirstEnableDropColumn=true
            };
            db.CodeFirst.InitTables<UNITDROPCOLUMNTEST>();
            var column2 = db.DbMaintenance.GetColumnInfosByTableName("UNITDROPCOLUMNTEST", false);
            if (column2.Count != 2)
            {
                throw new Exception("unit error");
            }
        }

        public class UnitDropColumnTest
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Name2 { get; set; }
        }
        public class UNITDROPCOLUMNTEST
        {
            public int Id { get; set; }
            public string Name { get; set; } 
        }

        [SqlSugar.SugarIndex("UnitIndextestIndex", nameof(UnitIndextest.Table), SqlSugar.OrderByType.Asc)]
        public class UnitIndextest
        {
            public string Table { get; set; }
            public string Id { get; set; }
        }

        public class UnitByteArray1 
        {
            [SugarColumn(IsNullable = true, ColumnDataType = "varchar(18)")]
            public string Aa { get; set; }
            [SugarColumn(IsNullable =true, ColumnDataType = "decimal(18,2)")]
            public decimal A { get; set; }
            public byte[] Data { get; set; }    
        }
        public class Test00111121
        {
            [SugarColumn(IsPrimaryKey = true)]
            public string id { get; set; }
            [SugarColumn(IsPrimaryKey =true)]
            public string creater { get; set; }
        }
        public class Test00111
        {
            public int id { get; set; }
            [SugarColumn(ColumnDataType = "guid",IsNullable =true)]
            public Guid? creater { get; set; }
        }
        public class UnitCodeTest1
        {
            public int Id { get; set; }
            public DateTime? CreateDate { get; set; }
        }
    }
}
