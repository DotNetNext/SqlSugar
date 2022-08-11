using HONORCSData;
using HONORCSData.Goods;
using HONORCSData.Order;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using HONORCSData.Order;
namespace OrmTest
{
    /// <summary>
    /// 采购明细
    /// </summary>
    public class PurchaseDetailModel
    {
        public int Index { get; set; }

        /// <summary>
        /// 录入日期
        /// </summary>
        public DateTime? EntryTime { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        public string No { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public int Type { get; set; }

        public string GoodsNo { get; set; }

        public string GoodsName { get; set; }


        public string GoodsCategory { get; set; }


        public string BrandName { get; set; }


        public decimal? Number { get; set; }


        public decimal? Price { get; set; }

        public decimal TotalPrice
        {
            get
            {
                if (Number == null || Price == null) return 0;
                return Number.Value * Price.Value;
            }
        }


    }
    public class CustomTest1
    {
        /// <summary>
        /// 采购明细汇总
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="res"></param>
        /// <returns></returns>
        public static async Task GetPurchaseDetailPageAsync(SqlSugarClient Db)
        {
            Db.CodeFirst.InitTables<EGoods, EGoodsBrand, EGoodsClass>();
            Db.CodeFirst.InitTables<EOrderAlbaran, EOrderAlbaranDetail, EOrderReturn, EOrderReturnDetail>();
            //入库单
            Db.Insertable(new EOrderAlbaranDetail()
            {
                AlbaranId = 1,
                BarCode = "a",
                ChineseName = "a",
                Commentary = "a",
                CostPrice = 1,
                DelFlag = "a",
                Discount = 1,
                GoodsBrandId = 1,
                GoodsBrandName = "a",
                GoodsClassId = 1,
                GoodsClassName = "a",
                GoodsNo = "a",
                Id = new Random().Next(0, 99999999),
                Index = 1,
                IsBlanceBarCode = true,
                IsProhibitedChangeDiscount = true,
                IsTemporary = true,
                Iva = 1,
                IvaId = 1,
                Number = 1,
                OrderId = 1,
                PackageAmount = 1,
                Price = 1,
                PurchaseSpec = "",
                Remark = "a",
                Req = 1,
                SpanishName = "a",
                TenantId = 1,
                Total = 1,
                UnitName = "a"
            }).ExecuteCommand();
            var query1 = Db.Queryable<EOrderAlbaranDetail, EOrderAlbaran, EGoods, EGoodsClass, EGoodsBrand>((d, h, g, c, b) => new JoinQueryInfos(
                JoinType.Left, h.AlbaranId == d.AlbaranId,
                JoinType.Left, d.GoodsNo == g.GoodsNo,
                JoinType.Left, g.GoodsClassId == c.GoodsClassId,
                JoinType.Left, g.BrandId == b.BrandId
                ))
            .Where((d, h, g, c, b) => h.AlbaranType == null)  //固定入库单
            .Select((d, h, g, c, b) => new PurchaseDetailModel
            {
                EntryTime = h.EntryTime,
                No = h.AlbaranNo,
                Type = 1,
                GoodsNo = d.GoodsNo,
                GoodsName = g.SpanishName,
                GoodsCategory = c.SpanishName,
                BrandName = b.BranchSpanishName,
                Number = d.Number,
                Price = d.Price
            });

            //退货单
            var query2 = Db.Queryable<EOrderReturnDetail, EOrderReturn, EGoods, EGoodsClass, EGoodsBrand>((d, h, g, c, b) => new JoinQueryInfos(
               JoinType.Left, h.OrderReturnId == d.OrderReturnId,
               JoinType.Left, d.GoodsNo == g.GoodsNo,
               JoinType.Left, g.GoodsClassId == c.GoodsClassId,
               JoinType.Left, g.BrandId == b.BrandId
               )).Where((d, h, g, c, b) => h.OrderReturnType == 1)  //固定入库单
            .Select((d, h, g, c, b) => new PurchaseDetailModel
            {
                EntryTime = h.EntryTime,
                No = h.OrderReturnNo,
                Type = 2,
                GoodsNo = d.GoodsNo,
                GoodsName = g.SpanishName,
                GoodsCategory = c.SpanishName,
                BrandName = b.BranchSpanishName,
                Number = d.Number,
                Price = d.Price
            });
            SqlSugar.RefAsync<int> totalNum = 0;

            var res2 = await query2.Clone().ToPageListAsync(1, 2, totalNum);
            var res = await Db.UnionAll(query1, query2).ToPageListAsync(1, 2, totalNum);

            var q = Db.Queryable<Order>().Where(it => it.Id == 1).Select(it => new { id=1});
            var q2 = q.Clone();
            var x = q.ToList();
            var y = q2.ToList();
            //var res = await Db.UnionAll<PurchaseDetailModel>(query1.Clone(), query2.Clone()).ToPageListAsync(1, 10, totalNum);
     
        }

    }
}
