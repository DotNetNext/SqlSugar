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
            Db.CodeFirst.InitTables<UnitByteArray>();
            Db.Insertable(new UnitByteArray()
            {
                Data = new byte[] { 1, 2, 123, 31, 1 }
            }).ExecuteCommand();
            var list2=Db.Queryable<UnitByteArray>().ToDataTable();
        }

        public class UnitByteArray 
        {
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
