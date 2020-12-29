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
            Db.CodeFirst.InitTables<UnitCodeTest2222>();
            Db.Insertable(new UnitCodeTest2222()
            {
                Id = 1,
                Id2 = 2,
                Id3 = 3,
                Id4=4
            }).ExecuteCommand();
            var list = Db.Queryable<UnitCodeTest2222>().ToList();
        }
        public class UnitCodeTest1
        {
            [SqlSugar.SugarColumn(IndexGroupNameList = new string[] { "group1" })]
            public int Id { get; set; }
            [SqlSugar.SugarColumn(DefaultValue="now()", IndexGroupNameList =new string[] {"group1" } )]
            public DateTime? CreateDate { get; set; }
        }
        public class UnitCodeTest2222
        {
            public uint Id { get; set; }
            public ulong Id2 { get; set; }
            public ushort Id3 { get; set; }
            public uint? Id4 { get; set; }
        }
    }
}
