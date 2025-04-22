using SqlSugar;
using SqlSugar.TDengine;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace TDengineTest
{
    public partial class ORMTest
    {
        public static void BulkCopy(ISqlSugarClient db) 
        {
            //建表
            db.CodeFirst.InitTables<BulkDemo2>();

            //插入
            TDengineFastBuilder.SetTags(db, (tag,stable) => $"{stable}_{tag}", "Tag1");//设置子表格式和Tag字段 
            db.Fastest<BulkDemo2>().BulkCopy(new List<BulkDemo2>(){
                new BulkDemo2()
                {
                        Boolean = true,
                        Tag1 = "a",
                        Ts = DateTime.Now.AddMilliseconds(1)//没启用微秒纳秒时间不能一样

                 },
                 new BulkDemo2()
                {
                        Boolean = false,
                        Tag1 = "a",
                        Ts = DateTime.Now.AddSeconds(1)

                 },
                 new BulkDemo2()
                 {
                        Boolean = true,
                        Tag1 = "b",
                        Ts = DateTime.Now.AddMilliseconds(333)
                }});
            var list= db.Queryable<BulkDemo2>().ToList();
        }

        [STableAttribute(STableName = "BulkDemo2", Tag1 = nameof(Tag1))]
        public class BulkDemo2
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true)]
            public DateTime Ts { get; set; }
            public bool Boolean { get; set; }
            public string Tag1 { get; set; }
        }
    }
}
