using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    internal class UnitSplitadfaf1
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<SplitTableDemo>();

            db.Insertable(new SplitTableDemo() { Name = "a", Time = DateTime.Now, Pk = Guid.NewGuid() })
                .SplitTable().ExecuteCommand();

            var list=db.Queryable<SplitTableDemo>().SplitTable()
               .Select<SplitTableDemoDto>().ToList();
            if (list.First().Time == DateTime.MinValue)
            {
                throw new Exception("unit eror");
            }
        }
        // Entity class representing the split table
        // 代表分表的实体类
        [SplitTable(SplitType.Day)] // Specify the split type as "Day"
        // 指定分表类型为“Day”
        [SqlSugar.SugarTable("UnitSplit1231_{year}{month}{day}")] // Specify the table name pattern
        // 指定表名模式
        public class SplitTableDemo
        {
            [SugarColumn(IsPrimaryKey = true)] // Specify primary key
            // 指定主键
            public Guid Pk { get; set; }
            [SugarColumn(IsNullable = true, ColumnName = "x_name")]
            public string Name { get; set; }

            [SugarColumn(IsNullable = true,ColumnName ="x_time")]
            [SplitField] // Mark the field as a split field
            // 将字段标记为分表字段
            public DateTime Time { get; set; }
        }
        public class SplitTableDemoDto
        {
            public DateTime Time { get; set; }

        }
    }
}
