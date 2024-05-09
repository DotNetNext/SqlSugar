using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class Unitadsfayasdfaaay
    {
        [SugarTable("test_sqlsugar_table")]
        public class TestSqlsugarTable
        {
            [SugarColumn(IsPrimaryKey = true)] public int id { get; set; }
            [SugarColumn(IsArray = true, ColumnDataType = "int[]")] public int[] arr_col { get; set; }
            [SugarColumn(IsJson = true, ColumnDataType = "jsonb")] public List<int> json_col { get; set; } = new();
            public byte[] byte_col { get; set; }
            public int int_col { get; set; }
        }

        public static void Init() 
        { 
            TestInsert().GetAwaiter().GetResult();
        }
  
        public static async Task TestInsert()
        {
            var db = NewUnitTest.Db;
            await InitDb();
            List<TestSqlsugarTable> list = new();
            for (int i = 0; i < 10; i++)
            {
                list.Add(new()
                {
                    id = i + 1,
                    arr_col = new int[] { i + 11, i + 12, i + 13 },
                    json_col = new List<int> { i + 21, i + 22, i + 23 },
                    byte_col = new byte[] { 100, 200, 3 },
                    int_col = 1
                });
            }

            //string sql1 = db.Insertable(list[0]).ToSqlString();

            //string sql = db.Insertable(list).ToSqlString();

            //var rst1 = await db.Insertable(list).ExecuteCommandAsync();
            db.CurrentConnectionConfig.SlaveConnectionConfigs = new List<SlaveConnectionConfig>()
            {
                new SlaveConnectionConfig(){ ConnectionString=Config.ConnectionString2 }
            };
            var rst1 = await db.Fastest<TestSqlsugarTable>().BulkMergeAsync(list);

            Console.WriteLine(rst1);

            var list4 = await db.Queryable<TestSqlsugarTable>().ToListAsync();
        }


        //public static async Task Read()
        //{
        //    var list = await db.Queryable<TestSqlsugar>().ToListAsync();
        //}

        public static async Task InitDb()
        {
            var db = NewUnitTest.Db;
            await db.Ado.ExecuteCommandAsync("DROP TABLE IF EXISTS test_sqlsugar_table");
            if (!db.DbMaintenance.IsAnyTable("test_sqlsugar_table", false))
            {
                db.CodeFirst.InitTables(typeof(TestSqlsugarTable));
            } 
        }

    }
    [SugarTable("test_sqlsugar_table")]
    public class TestSqlsugarTable
    {
        [SugarColumn(IsPrimaryKey = true)] public int id { get; set; }
        [SugarColumn(IsArray = true, ColumnDataType = "int[]")] public int[] arr_col { get; set; }
        [SugarColumn(IsJson = true, ColumnDataType = "jsonb")] public List<int> json_col { get; set; } = new();
        public byte[] byte_col { get; set; }
        public int int_col { get; set; }
    }
}
