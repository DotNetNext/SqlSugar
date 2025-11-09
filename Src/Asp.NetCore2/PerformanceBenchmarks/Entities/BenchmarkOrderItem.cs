using SqlSugar;

namespace PerformanceBenchmarks.Entities
{
    /// <summary>
    /// Benchmark order item entity
    /// 基准测试订单项实体
    /// </summary>
    [SugarTable("BenchmarkOrderItem")]
    public class BenchmarkOrderItem
    {
        /// <summary>
        /// Order item ID (Primary Key)
        /// 订单项ID（主键）
        /// </summary>
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int OrderItemId { get; set; }

        /// <summary>
        /// Order ID (Foreign Key)
        /// 订单ID（外键）
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// Product ID (Foreign Key)
        /// 产品ID（外键）
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Quantity
        /// 数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Unit price
        /// 单价
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Discount
        /// 折扣
        /// </summary>
        public decimal Discount { get; set; }

        /// <summary>
        /// Total price
        /// 总价
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// Navigation property to order
        /// 订单导航属性
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        [Navigate(NavigateType.OneToOne, nameof(OrderId))]
        public BenchmarkOrder Order { get; set; }

        /// <summary>
        /// Navigation property to product
        /// 产品导航属性
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        [Navigate(NavigateType.OneToOne, nameof(ProductId))]
        public BenchmarkProduct Product { get; set; }
    }
}
