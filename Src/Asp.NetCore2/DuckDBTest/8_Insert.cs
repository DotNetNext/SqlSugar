using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    internal class _8_Insert
    {
        public static void Init()
        {
            var db = DbHelper.GetNewDb();

            // 初始化实体表格（Initialize entity tables）
            db.CodeFirst.InitTables<StudentWithIdentity, StudentWithSnowflake>();

            // Use Case 1: 返回插入行数（Return the number of inserted rows）
            var rowCount = db.Insertable(new StudentWithIdentity() { Name = "name" }).ExecuteCommand();

            // Use Case 2: 插入数据并返回自增列（Insert data and return the auto-incremented column）
            var identity = db.Insertable(new StudentWithIdentity() { Name = "name2" }).ExecuteReturnIdentity();

            // Use Case 3: 返回雪花ID（Return the snowflake ID）
            var snowflakeId = db.Insertable(new StudentWithSnowflake() { Name = "name" }).ExecuteReturnSnowflakeId();

            // Use Case 4: 强制设置表名别名（Forcefully set table name alias）
            db.Insertable(new StudentWithIdentity() { Name = "name2" }).AS("StudentWithIdentity").ExecuteCommand();

            // Use Case 5: 批量插入实体（非参数化插入）（Batch insert entities (non-parameterized)）
            var list = db.Queryable<StudentWithIdentity>().Take(2).ToList();
            db.Insertable(list).ExecuteCommand();
            db.Insertable(list).PageSize(1000).ExecuteCommand();

            // Use Case 6: 参数化内部分页插入（Parameterized internal pagination insert）
            db.Insertable(list).UseParameter().ExecuteCommand();

            // Use Case 7: 大数据写入（示例代码，请根据实际情况调整）（Bulk data insertion - Example code, adjust based on actual scenario）
            var listLong = new List<StudentWithSnowflake>() {
            new StudentWithSnowflake() { Name = "name",Id=SnowFlakeSingle.Instance.NextId() },
            new StudentWithSnowflake() { Name = "name",Id=SnowFlakeSingle.Instance.NextId()}
            };
            db.Fastest<StudentWithSnowflake>().BulkCopy(listLong);
        }

        // 实体类：带自增主键（Entity class: With auto-increment primary key）
        [SugarTable("StudentWithIdentity08")]
        public class StudentWithIdentity
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        // 实体类：带雪花主键（Entity class: With snowflake primary key）
        [SugarTable("StudentWithSnowflake08")]
        public class StudentWithSnowflake
        {
            [SugarColumn(IsPrimaryKey = true)]
            public long Id { get; set; }
            public string Name { get; set; }
        }
    }
}