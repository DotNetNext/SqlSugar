using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OrmTest._8_Insert;

namespace OrmTest
{
    internal class _9_Update
    {
        /// <summary>
        /// 初始化更新方法（Initialize update methods）
        /// </summary>
        internal static void Init()
        {
            var db = DbHelper.GetNewDb();

            // 初始化实体表格（Initialize entity tables）
            db.CodeFirst.InitTables<StudentWithSnowflake>();

            // 创建一个需要更新的实体对象（Create an entity object to be updated）
            var updateObj = new StudentWithSnowflake() { Id = 1, Name = "order1" };

            // 创建需要批量更新的实体对象列表（Create a list of entity objects to be updated in bulk）
            var updateObjs = new List<StudentWithSnowflake> {
                 new StudentWithSnowflake() { Id = 11, Name = "order11", Date=DateTime.Now },
                 new StudentWithSnowflake() { Id = 12, Name = "order12", Date=DateTime.Now }
            };

            /***************************根据实体更新 (Update based on entity)***************************/

            // 更新单个实体对象（Update a single entity object）
            var result = db.Updateable(updateObj).ExecuteCommand();

            // 批量更新实体对象列表（Update a list of entity objects in bulk）
            var result20 = db.Updateable(updateObjs).ExecuteCommand();
            var result21 = db.Updateable(updateObjs).PageSize(500).ExecuteCommand();

            // 更新实体对象，忽略指定列（Update entity object, ignoring specific columns）
            var result3 = db.Updateable(updateObj).IgnoreColumns(it => new { it.Remark }).ExecuteCommand();

            // 更新实体对象的指定列（Update specific columns of the entity object）
            var result4 = db.Updateable(updateObj).UpdateColumns(it => new { it.Name, it.Date  }).ExecuteCommand();

            // 如果没有主键，按照指定列更新实体对象（If there is no primary key, update entity object based on specified columns）
            var result5 = db.Updateable(updateObj).WhereColumns(it => new { it.Id }).ExecuteCommand();

            // 如果字段值为NULL，不进行更新（Do not update columns with NULL values）
            var result6 = db.Updateable(updateObj).IgnoreNullColumns().ExecuteCommand();

            // 忽略为NULL和默认值的列进行更新（Ignore columns with NULL and default values during update）
            var result7 = db.Updateable(updateObj)
                          .IgnoreColumns(ignoreAllNullColumns: true, ignoreAllDefaultValue:true)
                          .ExecuteCommand();

            // 使用最快的方式批量更新实体对象列表（Bulk update a list of entity objects using the fastest method）
            var result8 = db.Fastest<StudentWithSnowflake>().BulkUpdate(updateObjs);

            /***************************表达式更新 (Expression Update)***************************/

            // 使用表达式更新实体对象的指定列（Update specific columns of the entity object using expressions）
            var result71 = db.Updateable<StudentWithSnowflake>()
                             .SetColumns(it => new StudentWithSnowflake() { Name = "a", Date = DateTime.Now })
                             .Where(it => it.Id == 11)
                             .ExecuteCommand();

            // 使用表达式更新实体对象的指定列（Update specific columns of the entity object using expressions）
            var result81 = db.Updateable<StudentWithSnowflake>()
                             .SetColumns(it => it.Name == "Name" + "1")
                             .Where(it => it.Id == 1)
                             .ExecuteCommand();
        }

        // 实体类：带雪花主键（Entity class: With snowflake primary key）
        [SugarTable("StudentWithSnowflake09")]
        public class StudentWithSnowflake
        {
            [SugarColumn(IsPrimaryKey = true)]
            public long Id { get; set; }
            public string Name { get; set; }
            public DateTime Date { get; set; }
            [SugarColumn(IsNullable = true)]
            public string Remark { get; set; }
        }
    }
}