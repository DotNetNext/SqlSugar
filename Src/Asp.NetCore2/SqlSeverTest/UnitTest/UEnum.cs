using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public partial class NewUnitTest
    {
        public static void Enum()
        {
            Db.CodeFirst.InitTables<Unit00Z11C12>();
            Db.Insertable(new Unit00Z11C12() { type = UnitType.a, type2 = null }).ExecuteCommand();
            var x=Db.Queryable<Unit00Z11C12>().Select(it=>new {
                x=it.type,
                x2=it.type2
            }).ToList();

            var x2 = Db.Queryable<Unit00Z11C12>().ToList();
            Db.Updateable<Unit00Z11C12>().SetColumns(it => it.type2 == UnitType.b)
                .Where(it=>true).ExecuteCommand();
            var x3 = Db.Queryable<Unit00Z11C12>().ToList();
            foreach (var item in x3)
            {
                item.type2 = null;
            }
            Db.Updateable<Unit00Z11C12>(x3).WhereColumns(it=>it.type).ExecuteCommand();
            var x4 = Db.Queryable<Unit00Z11C12>().ToList();
        }
        public class Unit00Z11C12
        {
            [SqlSugar.SugarColumn(ColumnDataType = "int", IsNullable = false)]
            public UnitType type { get; set; }
            [SqlSugar.SugarColumn(ColumnDataType ="int",IsNullable =true)]
            public UnitType? type2 { get; set; }
        }
        public enum UnitType {
             a=-1,
             b=2,
             c=3
        }
    }
}
