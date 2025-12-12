using Microsoft.Identity.Client;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class Unitafdafas 
    {
        public static void Init() 
        {
            Initdb().GetAwaiter().GetResult();
            TestInsert4().GetAwaiter().GetResult();
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<UnitWatchData>();
            db.Insertable(new List<UnitWatchData>()
            {
                new UnitWatchData(){ Id=1, WatchName="a", WatchValue=new byte[]{ 1} },
                  new UnitWatchData(){ Id=1, WatchName="a", WatchValue=new byte[]{ 1} }
            }).ExecuteCommand();
        }
        public static async Task TestInsert4()
        {
            var db = NewUnitTest.Db;
            List<TestRemoveCache> list = new();
            for (int i = 0; i < 2; i++)
            {
                list.Add(new()
                {
                    arr_col = new int[] { i + 1, i + 2, i + 3 },
                    json_col = new List<int> { i + 1, i + 2, i + 3 },
                    byte_col = new byte[] { 1,2,3},
                    int_col = 1
                });
            }
            //# 插入超过2条数据时 byte[] 字段值变了
            long rst1 = await db.Insertable(list).ExecuteCommandAsync();

            Console.WriteLine(rst1);

            //# 这里是已入库的值 byte_col 与插入前不一致
            var list4 = await db.Queryable<TestRemoveCache>().ToListAsync();

            if (string.Join("", list.First().byte_col ) != "123") 
            {
                throw new Exception("unit error");
            }
            list[0].byte_col = new byte[] { 10, 19, 16, 144, 78, 26, 5, 49, 45, 49, 45, 51, 40, 1, 66, 0, 72, 1, 80, 232, 7 };
            var  sql =   db.Insertable(list).ToSqlString();
            db.Ado.ExecuteCommand(sql);
            var xx=db.Queryable<TestRemoveCache>().ToList();
        }

        /// <summary>
        /// 初始化数据表
        /// </summary>
        /// <returns></returns>
        public static async Task Initdb()
        {
            var db = NewUnitTest.Db;
            await db.Ado.ExecuteCommandAsync("DROP TABLE IF EXISTS test_remove_cache");
            if (!db.DbMaintenance.IsAnyTable("test_remove_cache", false))
            {
                db.CodeFirst.InitTables(typeof(TestRemoveCache));
            }
        }
    }

    [SugarTable("test_remove_cache")]
    public class TestRemoveCache
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)] public int id { get; set; }
        [SugarColumn(IsArray = true, ColumnDataType = "int[]")] public int[] arr_col { get; set; }
        [SugarColumn(IsJson = true, ColumnDataType = "jsonb")] public List<int> json_col { get; set; } = new();
        public byte[] byte_col { get; set; }
        public int int_col { get; set; }
    }
    public class UnitWatchData

    {

        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnName = "Id")]

        public long Id { get; set; }



        public string WatchName { get; set; }



        [SugarColumn(ColumnDataType = "bytea")]

        public byte[] WatchValue { get; set; }

    }
}
