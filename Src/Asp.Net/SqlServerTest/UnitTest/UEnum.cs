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
            String();
            Int();
        }
        private static void String()
        {
            var db = Db;

            db.CurrentConnectionConfig.MoreSettings = new SqlSugar.ConnMoreSettings
            {
                TableEnumIsString = true
            };

            db.CodeFirst.InitTables<Unit00Z1string1>();
            db.Insertable(new Unit00Z1string1() { type = UnitType.a, type2 = null }).ExecuteCommand();
            List<UnitType> ids = new List<UnitType>() {
             UnitType.a,
             UnitType.b
            };
            var x = db.Queryable<Unit00Z1string1>()
                    .Where(it => it.type == UnitType.b)
                .Where(it=>it.type2== UnitType.b)
                .Where(it=> ids.Contains(it.type)).Select(it => new
            {
                x = it.type,
                x2 = it.type2
            }).ToList();

            var x2 = db.Queryable<Unit00Z1string1>().ToList();
            db.Updateable<Unit00Z1string1>().SetColumns(it => it.type2 == UnitType.b)
                .Where(it => true).ExecuteCommand();
            var x3 = db.Queryable<Unit00Z1string1>().ToList();
            foreach (var item in x3)
            {
                item.type2 = null;
            }
            db.Updateable<Unit00Z1string1>(x3).WhereColumns(it => it.type).ExecuteCommand();
            var x4 = db.Queryable<Unit00Z1string1>().ToList();

            db.Updateable<Unit00Z1string1>().SetColumns(it => new Unit00Z1string1 {
                type=UnitType.a,
                type2 = UnitType.b
            })
           .Where(it => true).ExecuteCommand();
        }
        private static void Int()
        {
            Db.CodeFirst.InitTables<Unit00Z11C12>();
            Db.Insertable(new Unit00Z11C12() { type = UnitType.a, type2 = null }).ExecuteCommand();
            var x = Db.Queryable<Unit00Z11C12>().Select(it => new
            {
                x = it.type,
                x2 = it.type2
            }).ToList();

            var x2 = Db.Queryable<Unit00Z11C12>().ToList();
            Db.Updateable<Unit00Z11C12>().SetColumns(it => it.type2 == UnitType.b)
                .Where(it => true).ExecuteCommand();
            var x3 = Db.Queryable<Unit00Z11C12>().ToList();
            foreach (var item in x3)
            {
                item.type2 = null;
            }
            Db.Updateable<Unit00Z11C12>(x3).WhereColumns(it => it.type).ExecuteCommand();
            var x4 = Db.Queryable<Unit00Z11C12>().ToList();
        }

        public class Unit00Z11C12
        {
            [SqlSugar.SugarColumn(ColumnDataType = "int", IsNullable = false)]
            public UnitType type { get; set; }
            [SqlSugar.SugarColumn(ColumnDataType ="int",IsNullable =true)]
            public UnitType? type2 { get; set; }
        }

        public class Unit00Z1string1
        {
            [SqlSugar.SugarColumn(ColumnDataType = "varchar(50)", IsNullable = false)]
            public UnitType type { get; set; }
            [SqlSugar.SugarColumn(ColumnDataType = "varchar(50)", IsNullable = true)]
            public UnitType? type2 { get; set; }
        }
        public enum UnitType {
             a=-1,
             b=2,
             c=3
        }
    }
}
