using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugarTest
{

    [SugarTable("Special", TableDescription = "专题", IsDisabledDelete = true)]
    public class Special : IDeleted
    {
        /// <summary>
        /// 非自增ID
        /// </summary>
        [SugarColumn(ColumnDescription = "ID", IsPrimaryKey = true)]
        public long Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [SugarColumn(ColumnDescription = "名称")]
        public string Name { get; set; }

        /// <summary>
        /// 默认假删除
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [SugarColumn(ColumnDescription = "默认假删除", DefaultValue = "false")]
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 删除用户ID
        /// </summary>
        [SugarColumn(ColumnDescription = "删除用户ID", IsOnlyIgnoreInsert = true, IsNullable = true)]
        public long? DeletedUserId { get; set; }

        /// <summary>
        /// 删除用户
        /// </summary>
        [SugarColumn(ColumnDescription = "删除用户", IsNullable = true, IsOnlyIgnoreInsert = true)]
        public string? DeletedUserName { get; set; }

        /// <summary>
        /// 删除时间
        /// </summary>
        [SugarColumn(ColumnDescription = "删除时间", IsOnlyIgnoreInsert = true, IsNullable = true)]
        public DateTimeOffset? DeletedTime { get; set; }
    }
}