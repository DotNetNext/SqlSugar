using SqlSugar;
using System;

namespace PerformanceBenchmarks.Entities
{
    /// <summary>
    /// Benchmark product entity
    /// 基准测试产品实体
    /// </summary>
    [SugarTable("BenchmarkProduct")]
    public class BenchmarkProduct
    {
        /// <summary>
        /// Product ID (Primary Key)
        /// 产品ID（主键）
        /// </summary>
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int ProductId { get; set; }

        /// <summary>
        /// Product name
        /// 产品名称
        /// </summary>
        [SugarColumn(Length = 100, IsNullable = false)]
        public string ProductName { get; set; }

        /// <summary>
        /// Product code
        /// 产品代码
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = false)]
        public string ProductCode { get; set; }

        /// <summary>
        /// Category
        /// 类别
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = true)]
        public string Category { get; set; }

        /// <summary>
        /// Unit price
        /// 单价
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Stock quantity
        /// 库存数量
        /// </summary>
        public int StockQuantity { get; set; }

        /// <summary>
        /// Description
        /// 描述
        /// </summary>
        [SugarColumn(ColumnDataType = StaticConfig.CodeFirst_BigString, IsNullable = true)]
        public string Description { get; set; }

        /// <summary>
        /// Is available
        /// 是否可用
        /// </summary>
        public bool IsAvailable { get; set; }

        /// <summary>
        /// Created date
        /// 创建日期
        /// </summary>
        public DateTime CreatedDate { get; set; }
    }
}
