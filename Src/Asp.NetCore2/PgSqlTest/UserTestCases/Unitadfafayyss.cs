using System;
using System.Linq;
using System.Linq.Expressions;
using SqlSugar;

namespace OrmTest 
{
    public class Unitdfaysss
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;
            //建表 
            db.CodeFirst.InitTables<Test001>();


            //清空表
            db.DbMaintenance.TruncateTable<Test001>();

            //第一次插入测试数据，
            var result = db.Insertable(new Test001() { Id = 1, Type = DemoType.示例 }).SplitTable().ExecuteCommand();//用例代码

            // 获取所有拆分表
            var tables = db.SplitHelper(typeof(Test001)).GetTables();
            db.Aop.OnLogExecuting = (x, y) =>
            {
                Console.WriteLine(x);

            };
            DemoType? type = null;
            // 更新所有拆分表
            foreach (var item in tables)
            {
                db.Updateable<Test001>().AS(item.TableName)//使用分表名
                   .SetColumns(x => new Test001 { Type = type })
                   .Where(x => x.Id == 1)
                   .ExecuteCommandAsync();
            }

            var xx = db.Queryable<Test001>().SplitTable().ToList();
            if (xx.First().Type != null)
            {
                throw new Exception("unit test");
            }
        }

    }
    //建类
    [SplitTable(SplitType.Year)]//按年度分表 （自带分表支持 年、季、月、周、日）
    [SugarTable("test001_{year}{month}{day}")]
    public class Test001
    {
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; }
        [SugarColumn(IsNullable = true)]
        public DemoType? Type { get; set; }
    }
    public enum DemoType
    {
        测试,
        示例
    }
}
