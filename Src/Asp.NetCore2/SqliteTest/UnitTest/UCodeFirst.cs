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
