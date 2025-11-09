using SqlSugar;
using System;

namespace PerformanceBenchmarks.Entities
{
    /// <summary>
    /// Benchmark customer entity
    /// 基准测试客户实体
    /// </summary>
    [SugarTable("BenchmarkCustomer")]
    public class BenchmarkCustomer
    {
        /// <summary>
        /// Customer ID (Primary Key)
        /// 客户ID（主键）
        /// </summary>
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int CustomerId { get; set; }

        /// <summary>
        /// Customer name
        /// 客户名称
        /// </summary>
        [SugarColumn(Length = 100, IsNullable = false)]
        public string CustomerName { get; set; }

        /// <summary>
        /// Email address
        /// 电子邮件地址
        /// </summary>
        [SugarColumn(Length = 100, IsNullable = true)]
        public string Email { get; set; }

        /// <summary>
        /// Phone number
        /// 电话号码
        /// </summary>
        [SugarColumn(Length = 20, IsNullable = true)]
        public string Phone { get; set; }

        /// <summary>
        /// Address
        /// 地址
        /// </summary>
        [SugarColumn(Length = 200, IsNullable = true)]
        public string Address { get; set; }

        /// <summary>
        /// City
        /// 城市
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = true)]
        public string City { get; set; }

        /// <summary>
        /// Country
        /// 国家
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = true)]
        public string Country { get; set; }

        /// <summary>
        /// Registration date
        /// 注册日期
        /// </summary>
        public DateTime RegistrationDate { get; set; }

        /// <summary>
        /// Is active
        /// 是否激活
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Navigation property to orders
        /// 订单导航属性
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        [Navigate(NavigateType.OneToMany, nameof(BenchmarkOrder.CustomerId))]
        public System.Collections.Generic.List<BenchmarkOrder> Orders { get; set; }
    }
}
