using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OrmTest
{
    internal class USelectDTO
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<ProductStockFlow, ProductStock>();

            //5.1.4.86 不报错
            //5.1.4.94 报错

            var flowQuery = db.Queryable<ProductStockFlow>()
                .GroupBy(s => new { s.ProductId })
                .Select(s => new
                {
                    s.ProductId,
                    LastOutTime = SqlFunc.AggregateMax(s.CreateTime)
                });

            var data = db.Queryable<ProductStock>()
                .LeftJoin(flowQuery, (s, f) => s.ProductId == f.ProductId)
                .Where(s => s.StockQty > 0)
                .Select((s, f) => new ProductStockMonitorDto() {  }, true)
                .ToList();

            var data2 = db.Queryable<ProductStock>()
           .LeftJoin(flowQuery, (s, f) => s.ProductId == f.ProductId)
           .Where(s => s.StockQty > 0)
           .Select<ProductStockMonitorDto>()
           .ToList();
        }
    }
    public class ProductStockFlow
    {
        [SugarColumn(IsNullable = false, IsPrimaryKey = true, IsIdentity = false)]
        public long Id { get; set; }

        public long ProductId { get; set; }

        /// <summary>
        /// 产品货号
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 发生数量
        /// </summary>
        [SugarColumn(Length = 18, DecimalDigits = 4)]
        public decimal SettleQty { get; set; }

        /// <summary>
        /// 发生类型
        /// </summary>
        public string SettleType { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Remark { get; set; }

        public DateTime CreateTime { get; set; }
    }

    public class ProductStock
    {
        [SugarColumn(IsNullable = false, IsPrimaryKey = true, IsIdentity = false)]
        public long Id { get; set; }

        public long ProductId { get; set; }

        /// <summary>
        /// 产品货号
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 库存数量
        /// </summary>
        [SugarColumn(Length = 18, DecimalDigits = 4)]
        public decimal StockQty { get; set; }
    }

    public class ProductStockMonitorDto
    {
        public long ProductId { get; set; }

        /// <summary>
        /// 产品货号
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 库存数量
        /// </summary>
        public decimal StockQty { get; set; }

        /// <summary>
        /// 最后出库时间
        /// </summary>
        public DateTime? LastOutTime { get; set; }
    }
}
