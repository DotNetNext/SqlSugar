using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    internal class _a4_SplitTable
    {
        // Method for initialization and testing of split tables
        // 用于初始化和测试分表的方法
        public static void Init()
        {
            // Obtain a new instance of SqlSugarClient
            // 获取 SqlSugarClient 的新实例
            SqlSugarClient db = DbHelper.GetNewDb();


            // Entities change the synchronization table structure
            // 实体变化同步表结构
            db.CodeFirst.SplitTables().InitTables<SplitTableDemo>();

            // Insert records into the split table and create table
            // 向分表插入记录并创建表
            db.Insertable(new SplitTableDemo() { Name = "jack", Time = DateTime.Now }).SplitTable().ExecuteCommand();
            db.Insertable(new SplitTableDemo() { Name = "jack2", Time = DateTime.Now.AddDays(-11) }).SplitTable().ExecuteCommand();

            // Query records from the split table within a specified date range
            // 在指定日期范围内从分表查询记录
            var list = db.Queryable<SplitTableDemo>()
                .Where(it=>it.Name!=null)
                .SplitTable(DateTime.Now.Date.AddYears(-1), DateTime.Now)
                .ToList();

            // Update records from the split table
            // 从分表更新记录
            var updateList = list.Take(2).ToList();
            db.Updateable(updateList).SplitTable().ExecuteCommand();

            // Delete records from the split table
            // 从分表删除记录
            db.Deleteable(updateList).SplitTable().ExecuteCommand();
        }

        // Entity class representing the split table
        // 代表分表的实体类
        [SplitTable(SplitType.Day)] // Specify the split type as "Day"
        // 指定分表类型为“Day”
        [SqlSugar.SugarTable("SplitTableDemo_{year}{month}{day}")] // Specify the table name pattern
        // 指定表名模式
        public class SplitTableDemo
        {
            [SugarColumn(IsPrimaryKey = true)] // Specify primary key
            // 指定主键
            public Guid Pk { get; set; }
            public string Name { get; set; }

            [SugarColumn(IsNullable = true)]
            [SplitField] // Mark the field as a split field
            // 将字段标记为分表字段
            public DateTime Time { get; set; }
        }
    }
}