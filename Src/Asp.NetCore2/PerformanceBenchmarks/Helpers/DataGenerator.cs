using PerformanceBenchmarks.Entities;
using System;
using System.Collections.Generic;

namespace PerformanceBenchmarks.Helpers
{
    /// <summary>
    /// Test data generator helper
    /// 测试数据生成器辅助类
    /// </summary>
    public static class DataGenerator
    {
        private static readonly Random _random = new Random();

        /// <summary>
        /// Generate customer list
        /// 生成客户列表
        /// </summary>
        /// <param name="count">Number of customers / 客户数量</param>
        /// <returns>Customer list / 客户列表</returns>
        public static List<BenchmarkCustomer> GenerateCustomers(int count)
        {
            var customers = new List<BenchmarkCustomer>();
            var cities = new[] { "Beijing", "Shanghai", "Guangzhou", "Shenzhen", "Hangzhou", "Chengdu" };
            var countries = new[] { "China", "USA", "Japan", "Germany", "France" };

            for (int i = 1; i <= count; i++)
            {
                customers.Add(new BenchmarkCustomer
                {
                    CustomerName = $"Customer {i}",
                    Email = $"customer{i}@test.com",
                    Phone = $"{_random.Next(100, 999)}-{_random.Next(100, 999)}-{_random.Next(1000, 9999)}",
                    Address = $"{_random.Next(1, 999)} Main Street",
                    City = cities[_random.Next(cities.Length)],
                    Country = countries[_random.Next(countries.Length)],
                    RegistrationDate = DateTime.Now.AddDays(-_random.Next(1, 365)),
                    IsActive = _random.Next(0, 2) == 1
                });
            }

            return customers;
        }

        /// <summary>
        /// Generate product list
        /// 生成产品列表
        /// </summary>
        /// <param name="count">Number of products / 产品数量</param>
        /// <returns>Product list / 产品列表</returns>
        public static List<BenchmarkProduct> GenerateProducts(int count)
        {
            var products = new List<BenchmarkProduct>();
            var categories = new[] { "Electronics", "Clothing", "Food", "Books", "Sports" };

            for (int i = 1; i <= count; i++)
            {
                products.Add(new BenchmarkProduct
                {
                    ProductName = $"Product {i}",
                    ProductCode = $"PRD{i:D5}",
                    Category = categories[_random.Next(categories.Length)],
                    UnitPrice = Math.Round((decimal)(_random.NextDouble() * 1000), 2),
                    StockQuantity = _random.Next(0, 1000),
                    Description = $"This is a description for product {i}. It contains detailed information about the product features and specifications.",
                    IsAvailable = _random.Next(0, 10) > 1, // 90% available
                    CreatedDate = DateTime.Now.AddDays(-_random.Next(1, 365))
                });
            }

            return products;
        }

        /// <summary>
        /// Generate order list
        /// 生成订单列表
        /// </summary>
        /// <param name="count">Number of orders / 订单数量</param>
        /// <param name="customerCount">Number of customers / 客户数量</param>
        /// <returns>Order list / 订单列表</returns>
        public static List<BenchmarkOrder> GenerateOrders(int count, int customerCount)
        {
            var orders = new List<BenchmarkOrder>();
            var statuses = new[] { "Pending", "Processing", "Completed", "Cancelled" };

            for (int i = 1; i <= count; i++)
            {
                var orderDate = DateTime.Now.AddDays(-_random.Next(1, 365));
                orders.Add(new BenchmarkOrder
                {
                    CustomerId = _random.Next(1, customerCount + 1),
                    OrderNumber = $"ORD{DateTime.Now.Year}{i:D6}",
                    OrderDate = orderDate,
                    TotalAmount = Math.Round((decimal)(_random.NextDouble() * 10000), 2),
                    Status = statuses[_random.Next(statuses.Length)],
                    ShippingAddress = $"{_random.Next(1, 999)} Shipping Avenue, Floor {_random.Next(1, 20)}",
                    CreatedDate = orderDate,
                    UpdatedDate = orderDate.AddDays(_random.Next(1, 30))
                });
            }

            return orders;
        }

        /// <summary>
        /// Generate order item list
        /// 生成订单项列表
        /// </summary>
        /// <param name="orderId">Order ID / 订单ID</param>
        /// <param name="productCount">Number of products / 产品数量</param>
        /// <param name="itemsPerOrder">Items per order / 每个订单的项数</param>
        /// <returns>Order item list / 订单项列表</returns>
        public static List<BenchmarkOrderItem> GenerateOrderItems(int orderId, int productCount, int itemsPerOrder)
        {
            var items = new List<BenchmarkOrderItem>();

            for (int i = 0; i < itemsPerOrder; i++)
            {
                var quantity = _random.Next(1, 10);
                var unitPrice = Math.Round((decimal)(_random.NextDouble() * 500), 2);
                var discount = Math.Round((decimal)(_random.NextDouble() * 0.3), 2); // 0-30% discount

                items.Add(new BenchmarkOrderItem
                {
                    OrderId = orderId,
                    ProductId = _random.Next(1, productCount + 1),
                    Quantity = quantity,
                    UnitPrice = unitPrice,
                    Discount = discount,
                    TotalPrice = Math.Round(quantity * unitPrice * (1 - discount), 2)
                });
            }

            return items;
        }

        /// <summary>
        /// Generate random string
        /// 生成随机字符串
        /// </summary>
        /// <param name="length">String length / 字符串长度</param>
        /// <returns>Random string / 随机字符串</returns>
        public static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var result = new char[length];
            
            for (int i = 0; i < length; i++)
            {
                result[i] = chars[_random.Next(chars.Length)];
            }

            return new string(result);
        }

        /// <summary>
        /// Generate random email
        /// 生成随机邮箱
        /// </summary>
        /// <returns>Random email / 随机邮箱</returns>
        public static string GenerateRandomEmail()
        {
            var domains = new[] { "gmail.com", "yahoo.com", "outlook.com", "test.com", "example.com" };
            var username = GenerateRandomString(8).ToLower();
            var domain = domains[_random.Next(domains.Length)];
            
            return $"{username}@{domain}";
        }

        /// <summary>
        /// Generate random phone number
        /// 生成随机电话号码
        /// </summary>
        /// <returns>Random phone number / 随机电话号码</returns>
        public static string GenerateRandomPhone()
        {
            return $"{_random.Next(100, 999)}-{_random.Next(100, 999)}-{_random.Next(1000, 9999)}";
        }
    }
}
