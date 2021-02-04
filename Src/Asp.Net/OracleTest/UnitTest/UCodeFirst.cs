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
            Demo2();
            Demo3();
        }

        private static void Demo2()
        {
            Db.CodeFirst.InitTables<UnitCodeTest1>();
            Db.CodeFirst.InitTables<Unitasdfa1>();
            Db.Insertable(new Unitasdfa1() { t1 = 1, t2 = 1 }).ExecuteCommand();
            var x = Db.Queryable<Unitasdfa1>().ToList();
            if (x.First().t1 == 0 || x.First().t2 == 0)
            {
                throw new Exception("UnitCodeTest1");
            }
        }
        private static void Demo3()
        {
            Db.CodeFirst.InitTables<Unitasdfa2>();
            Db.CodeFirst.InitTables<Unitasdfa2>();
            Db.Insertable(new Unitasdfa2() { t1 = 1, t2 = 1 }).ExecuteCommand();
            var x = Db.Queryable<Unitasdfa2>().ToList();
            if (x.First().t1 == 0 || x.First().t2 == 0)
            {
                throw new Exception("UnitCodeTest1");
            }
        }

        public class Unitasdfa1
        {
            [SqlSugar.SugarColumn(ColumnDataType = "number(8,2)")]
            public decimal t2 { get; set; }

            [SqlSugar.SugarColumn(ColumnDataType ="number(8,2)")]
            public decimal? t1 { get; set; }
        }
        public class Unitasdfa2
        {
            [SqlSugar.SugarColumn(ColumnDataType = "number(8,3)")]
            public decimal t2 { get; set; }

            [SqlSugar.SugarColumn(ColumnDataType = "number(8,3)")]
            public decimal? t1 { get; set; }
        }
        public class UnitCodeTest1
        {
            [SqlSugar.SugarColumn(IndexGroupNameList = new string[] { "group1" })]
            public int Id { get; set; }
            [SqlSugar.SugarColumn(DefaultValue="sysdate", IndexGroupNameList =new string[] {"group1" } )]
            public DateTime? CreateDate { get; set; }
        }
    }
}
