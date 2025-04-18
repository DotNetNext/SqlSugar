using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    internal class Unitsdfadsfsys
    {
        // Method for initialization and testing of split tables
        // 用于初始化和测试分表的方法
        public static void Init()
        {
            // Obtain a new instance of SqlSugarClient
            // 获取 SqlSugarClient 的新实例
            SqlSugarClient db = NewUnitTest.Db;
            db.Aop.DataExecuting = (oldValue, entityInfo) =>
            {
                if (entityInfo.PropertyName == "Name") 
                {
                    entityInfo.SetValue(oldValue + "1");
                }
            };

            db.CodeFirst.InitTables<Unitsdfay>();
            db.DbMaintenance.TruncateTable<Unitsdfay>();

            // 实体变化同步表结构
            db.CodeFirst.SplitTables().InitTables<SplitTableDemo>();

            db.DbMaintenance.TruncateTable<SplitTableDemo>();

            db.Insertable(new SplitTableDemo() { Name = "jack", Time = DateTime.Now }).SplitTable().ExecuteCommand();

            db.Insertable( 
            new SplitTableDemo() { Name = "lilei", Time = DateTime.Now } ).SplitTable().ExecuteCommandAsync().GetAwaiter().GetResult();

            // Query records from the split table within a specified date range
            // 在指定日期范围内从分表查询记录
            var list = db.Queryable<SplitTableDemo>()
                .SplitTable()
                .ToList();

            if (list.First(it=>it.Name.StartsWith("jack")).Name != "jack1"|| list.Last(it=>it.Name.StartsWith("li")).Name != "lilei1") 
            {
                throw new Exception("unit error");
            }

            db.Insertable(new Unitsdfay() { Name = "a" }).ExecuteCommand();
            if (db.Queryable<Unitsdfay>().First().Name != "a1")
            {
                throw new Exception("unit error");
            }
            db.CodeFirst.InitTables<UnitDAFREA>();
            var r=db.DbMaintenance.GetColumnInfosByTableName("UnitDAFREA", false).First().ColumnDescription;
            if (r != "a") 
            { 
                throw new Exception("unit error");
            }
            db.CodeFirst.InitTables<UNITDAFREA>();
            r = db.DbMaintenance.GetColumnInfosByTableName("UnitDAFREA", false).First().ColumnDescription;
            if (r != "b")
            {
                throw new Exception("unit error");
            }
        }

        public class UnitDAFREA 
        {
            [SugarColumn(ColumnDescription ="a")]
           public string a { get; set; }
        }
        public class UNITDAFREA
        {
            [SugarColumn(ColumnDescription = "b")]
            public string a { get; set; }
        }

        public class Unitsdfay 
        {
            public string Name { get; set; }
        }

        // Entity class representing the split table
        // 代表分表的实体类
        [SplitTable(SplitType.Day)] // Specify the split type as "Day"
        // 指定分表类型为“Day”
        [SqlSugar.SugarTable("UnitlsdafaitTableDemo_{year}{month}{day}")] // Specify the table name pattern
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