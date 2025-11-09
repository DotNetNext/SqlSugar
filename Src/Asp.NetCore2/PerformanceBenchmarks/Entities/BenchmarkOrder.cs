using SqlSugar;
using System;

namespace PerformanceBenchmarks.Entities
{
    /// <summary>
    /// Benchmark order entity
    /// 基准测试订单实体
    /// </summary>
    [SugarTable("BenchmarkOrder")]
    public class BenchmarkOrder
    {
        /// <summary>
        /// Order ID (Primary Key)
        /// 订单ID（主键）
        /// </summary>
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int OrderId { get; set; }

        /// <summary>
        /// Customer ID (Foreign Key)
        /// 客户ID（外键）
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Order number
        /// 订单号
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = false)]
        public string OrderNumber { get; set; }

        /// <summary>
        /// Order date
        /// 订单日期
        /// </summary>
        public DateTime OrderDate { get; set; }

        /// <summary>
        /// Total amount
        /// 总金额
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Order status
        /// 订单状态
        /// </summary>
        [SugarColumn(Length = 20)]
        public string Status { get; set; }

        /// <summary>
        /// Shipping address
        /// 配送地址
        /// </summary>
        [SugarColumn(Length = 200, IsNullable = true)]
        public string ShippingAddress { get; set; }

        /// <summary>
        /// Created date
        /// 创建日期
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Updated date
        /// 更新日期
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public DateTime? UpdatedDate { get; set; }

        /// <summary>
        /// Navigation property to customer
        /// 客户导航属性
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        [Navigate(NavigateType.OneToOne, nameof(CustomerId))]
        public BenchmarkCustomer Customer { get; set; }

        /// <summary>
        /// Navigation property to order items
        /// 订单项导航属性
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        [Navigate(NavigateType.OneToMany, nameof(BenchmarkOrderItem.OrderId))]
        public System.Collections.Generic.List<BenchmarkOrderItem> OrderItems { get; set; }
    }
}
