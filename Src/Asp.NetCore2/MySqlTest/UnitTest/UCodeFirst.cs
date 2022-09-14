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
            Db.CodeFirst.InitTables<UnitCodeTest111>();
            Db.CodeFirst.InitTables<UnitIndextest>();


            Db.CodeFirst.InitTables(typeof(TestListJson));
            Db.DbMaintenance.TruncateTable<TestListJson>();
            Db.Insertable(new TestListJson { Id = 123, NameList = new List<string> { "123abc" } }).ExecuteCommand();
            var xxx = Db.Queryable<TestListJson>().Where(x => x.Id == 123).Select(x => new TestListJson
            {
                Id = x.Id,
                NameList = x.NameList }).First();
            if (xxx.NameList == null)
            {
                throw new Exception("unit error");
            }
            Db.CodeFirst.InitTables<Testlong2>();
            Db.DbMaintenance.TruncateTable<Testlong2>();
            Db.Insertable<Testlong2>(new Testlong2() { Id= 1 }).ExecuteReturnEntityAsync().GetAwaiter().GetResult();
            if (Db.Queryable<Testlong2>().Any(x => x.Id == 1) == false) throw new Exception("unit error");
        }

        [SugarTable("Testlong2")]
        public class Testlong2
        {
            [SugarColumn(IsPrimaryKey = true)]
            public long Id { get; set; }

           
        }


        [SugarTable("testjson")]
        public class TestListJson
        {
            [SugarColumn(IsPrimaryKey = true)]
            public int Id { get; set; }

            [SugarColumn(ColumnName = "name_list", IsJson = true, ColumnDataType = "json")]
            public List<string> NameList { get; set; }
        }
        [SqlSugar.SugarIndex("UnitIndextestIndex", nameof(UnitIndextest.Table), SqlSugar.OrderByType.Asc)]
        public class UnitIndextest
        {
            public string Table { get; set; }
            public string Id { get; set; }
        }
        public class UnitCodeTest1
        {
            [SqlSugar.SugarColumn(IndexGroupNameList = new string[] { "group1" })]
            public int Id { get; set; }
            [SqlSugar.SugarColumn(DefaultValue="now()", IndexGroupNameList =new string[] {"group1" } )]
            public DateTime? CreateDate { get; set; }
        }
        [SqlSugar.SugarIndex("UCTINDEX",
                      nameof(UnitCodeTest111.CreateDate),SqlSugar.OrderByType.Desc,
                      nameof(UnitCodeTest111.CreateDate2), SqlSugar.OrderByType.Desc,
                      nameof(UnitCodeTest111.CreateDate3), SqlSugar.OrderByType.Desc
            )]
        public class UnitCodeTest111
        {
 
            public int Id { get; set; }
          
            public DateTime? CreateDate { get; set; }
            public DateTime? CreateDate2 { get; set; }
            public DateTime? CreateDate3 { get; set; }
        }
    }
}
