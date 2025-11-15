using SqlSugar;
using System;
using System.Collections.Generic;

using System.Threading.Tasks;

namespace OrmTest
{
    internal class Unitsadsfasdfys
    {
        public static void Init()
        {
            var _db = DbHelper.GetNewDb();

            // 执行你的查询（已修复 WithNoLock）
            var brandIds = new List<int> { 1 }; // 可变条件
            _db.CodeFirst.InitTables<Category, SKU, Product>();
            var result = _db.Queryable<Category>()  
                .Where((c ) =>
                    SqlFunc.Subqueryable<SKU>()
                        .WithNoLock()
                        .InnerJoin<Product>(
                            (s, p) => s.ProductId == p.ProductId
                        ) 
                        .Where((s, p) => s.IsCanPurchase == true && s.IsSalesmanBuy == false && s.Valid == false) 
                        .Any()
                ) 
                .With(SqlWith.NoLock) // Category 表加 NOLOCK
                .ToList();
            var sqlObj = _db.Queryable<Category>()
                        .Where((c) =>
                            SqlFunc.Subqueryable<SKU>()
                                .WithNoLock()
                                .InnerJoin<Product>(
                                    (s, p) => s.ProductId == p.ProductId
                                )
                                .Where((s, p) => s.IsCanPurchase == true && s.IsSalesmanBuy == false && s.Valid == false)
                                .Any()
                        )
                        .With(SqlWith.NoLock) // Category 表加 NOLOCK
                        .ToSqlString();
            if (!sqlObj.Contains(" [UnitsdsdfaProduct] [p] WITH(NOLOCK) ")) { throw new Exception("unit error"); }
        }

        // ======================
        // 实体类定义
        // ======================

        [SugarTable("UnitdafaCategory")]
        public class Category
        {
            [SugarColumn(IsPrimaryKey = true)]
            public int CategoryId { get; set; }
            public string CategoryName { get; set; }
            public int BrandId { get; set; }
            public bool IsEnable { get; set; }
            public bool Valid { get; set; }
        }

        [SugarTable("UnitsdgssBrand")]
        public class Brand
        {
            [SugarColumn(IsPrimaryKey = true)]
            public int BrandId { get; set; }
            public string BrandName { get; set; }
            public bool IsEnable { get; set; }
            public bool Valid { get; set; }
        }

        [SugarTable("UnitsdsdfaProduct")]
        public class Product
        {
            [SugarColumn(IsPrimaryKey = true)]
            public int ProductId { get; set; }
            public int CategoryId { get; set; }
            public int BrandId { get; set; }
            public string ProductName { get; set; }
            public bool Valid { get; set; }
        }

        [SugarTable("UnitaadfafaSKU")]
        public class SKU
        {
            [SugarColumn(IsPrimaryKey = true)]
            public int SkuId { get; set; }
            public int ProductId { get; set; }
            public bool IsCanPurchase { get; set; }
            public bool IsSalesmanBuy { get; set; }
            public bool Valid { get; set; }
        }

        // ======================
        // 输出 DTO
        // ======================
        public class SaleCategoryOutputDto
        {
            public int CategoryId { get; set; }
            public string CategoryName { get; set; }
            public int BrandId { get; set; }
        }

      
    }
}
