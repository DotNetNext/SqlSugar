using SqlSugar;
using SqlSugar.DbConvert;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public partial class NewUnitTest
    {
        public UnitType xxx { get; set; }
        public static void Enum()
        {
            String(new NewUnitTest() { xxx = UnitType.a });
            Int();
        }
        private static void String(NewUnitTest x)
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
            var zz = UnitType.b;
            var x1 = db.Queryable<Unit00Z1string1>()
                    .Where(it => it.type == zz)
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
            db.Queryable<Unit00Z1string1>().WhereClass(new Unit00Z1string1()
            {

                type = UnitType.a,
                type2 = UnitType.b
            }).ToList();
            var type = UnitType.a;
            var p2=db.Updateable<Unit00Z1string1>().SetColumns(it => new Unit00Z1string1
            {
                type = type,
                type2 = type
            })
           .Where(it => true).ToSql();
            if (!(p2.Value.First().Value is string))
            {
                throw new Exception("unit error");
            }
             p2 = db.Updateable<Unit00Z1string1>().SetColumns(it => new Unit00Z1string1
            {
                type = x.xxx,
                type2 = type
            })
              .Where(it => true).ToSql();
            if (!(p2.Value.First().Value is string))
            {
                throw new Exception("unit error");
            }

            db.CurrentConnectionConfig.MoreSettings.TableEnumIsString = false;
            var p=db.Updateable<Unit00Z1String1>().SetColumns(it => new Unit00Z1String1
            {
                type = type,
                type2 = type
            })
          .Where(it => true).ToSql();
            if (!(p.Value.First().Value is string)) 
            {
                throw new Exception("unit error");
            }
            var type3 = UnitType.a;
            var list=db.Queryable<Unit00Z1String1>().Where(it => it.type == type).ToList();
            var list2 = db.Queryable<Unit00Z1String1>().Where(it => it.type == UnitType.a).ToList();
            var list21 = db.Queryable<Unit00Z1String1>().Where(it => it.type == x.xxx).ToList();
            List<UnitType> unitTypes = new List<UnitType>()
            {
                UnitType.a
            };
            List<UnitType?> unitTypes2 = new List<UnitType?>()
            {
                UnitType.a
            };
            Db.Queryable<Unit00Z1String1>()
                .Where(it=>""==type3.ToString()) 
                .Where(it => unitTypes2.Contains(it.type2))
                .Where(it => unitTypes.Contains(it.type)).ToList();
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

            Db.Queryable<Unit00Z11C12>().WhereClass(new Unit00Z11C12() { 
            
             type= UnitType.a,
             type2=UnitType.b
            }).ToList();
        }

        public class Unit00Z11C12
        {
            [SqlSugar.SugarColumn(ColumnDataType = "int", IsNullable = false)]
            public UnitType type { get; set; }
            [SqlSugar.SugarColumn(ColumnDataType ="int",IsNullable =true)]
            public UnitType? type2 { get; set; }
        }
        public class Unit00Z1String1
        {
            [SqlSugar.SugarColumn(ColumnDataType = "varchar(50)", IsNullable = false, SqlParameterDbType = typeof(EnumToStringConvert))]
            public UnitType type { get; set; }
            [SqlSugar.SugarColumn(ColumnDataType = "varchar(50)", IsNullable = true, SqlParameterDbType = typeof(EnumToStringConvert))]
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
