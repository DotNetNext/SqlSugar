using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public partial class NewUnitTest
    {
        public static void Queryable()
        {

            var pageindex = 1;
            var pagesize = 10;
            var total = 0;
            var totalPage = 0;
            var list = Db.Queryable<Order>().ToPageList(pageindex, pagesize, ref total, ref totalPage);

            //Db.CodeFirst.InitTables(typeof(CarType));
            //Db.Updateable<CarType>()
            //      .SetColumns(it => new CarType { State = SqlSugar.SqlFunc.IIF(it.State == true, false, true) }).Where(it => true)
            //   .ExecuteCommand();

            //Db.CodeFirst.InitTables(typeof(TestTree));
            //Db.DbMaintenance.TruncateTable<TestTree>();
            //Db.Ado.ExecuteCommand("insert testtree values(hierarchyid::GetRoot(),geography :: STGeomFromText ('POINT(55.9271035250276 -3.29431266523898)',4326),'name')");
            //var list2 = Db.Queryable<TestTree>().ToList();

            Db.CodeFirst.InitTables<UnitGuidTable>();
            Db.Queryable<UnitGuidTable>().Where(it => it.Id.HasValue).ToList();

            Db.Queryable<Order>().Where(it => SqlSugar.SqlFunc.Equals(it.CreateTime.Date, it.CreateTime.Date)).ToList();

            var sql = Db.Queryable<UnitSelectTest>().Select(it => new UnitSelectTest()
            {

                DcNull = it.Dc,
                Dc = it.Int
            }).ToSql().Key;
            UValidate.Check(sql, "SELECT  `Dc` AS `DcNull` , `Int` AS `Dc`  FROM `UnitSelectTest`", "Queryable");

            sql = Db.Updateable<UnitSelectTest2>(new UnitSelectTest2()).ToSql().Key;
            UValidate.Check(sql, @"UPDATE `UnitSelectTest2`  SET
           `Dc`=@Dc,`IntNull`=@IntNull  WHERE `Int`=@Int", "Queryable");

            sql = Db.Queryable<Order>().IgnoreColumns(it => it.CreateTime).ToSql().Key;
            UValidate.Check(sql, "SELECT `Id`,`Name`,`Price`,`CustomId` FROM `Order` ", "Queryable");
            sql = Db.Queryable<Order>().IgnoreColumns(it => new { it.Id, it.Name }).ToSql().Key;
            UValidate.Check(sql, "SELECT `Price`,`CreateTime`,`CustomId` FROM `Order` ", "Queryable");
            sql = Db.Queryable<Order>().IgnoreColumns("id").ToSql().Key;
            UValidate.Check(sql, "SELECT `Name`,`Price`,`CreateTime`,`CustomId` FROM `Order` ", "Queryable");

            var cts = IEnumerbleContains.Data();
            var list2=Db.Queryable<Order>()
                    .Where(p => /*ids.*/cts.Select(c => c.Id).Contains(p.Id)).ToList();

            var cts2 = IEnumerbleContains.Data().ToList(); ;
            var list3 = Db.Queryable<Order>()
                    .Where(p => /*ids.*/cts2.Select(c => c.Id).Contains(p.Id)).ToList();


            var list4 = Db.Queryable<Order>()
                .Where(p => new List<int> { 1, 2, 3 }.Where(b => b > 1).Contains(p.Id)).ToList();

            Db.CodeFirst.InitTables<UnitTest3>();
            var list5 = Db.Queryable<UnitTest3>().Where(it => SqlSugar.SqlFunc.ToString(it.Date.Value.Year) == "1").ToList();
            var list6 = Db.Queryable<UnitTest3>().Where(it => it.Date.Value.Year == 1).ToList();
            var list7 = Db.Queryable<UnitTest3>().Where(it => it.Date.Value.Date == DateTime.Now.Date).ToList();


            SaleOrder saleOrderInfo = new SaleOrder();
            Db.CodeFirst.InitTables<SaleOrder>();
            var result = Db.GetSimpleClient<SaleOrder>().Update(o => new SaleOrder()
            {
                OrderStatus = 1,
                CheckMan = saleOrderInfo.CheckMan,
                CheckTime = DateTime.Now
            }, o => o.OrderSn == saleOrderInfo.OrderSn && o.OrderStatus != 1);

            Db.CodeFirst.InitTables<UnitAbc121>();
            Db.Insertable(new UnitAbc121() {  name="a",uid=null }).ExecuteCommand();
            Db.Insertable(new UnitAbc121() { name = "a", uid=Guid.NewGuid() }).ExecuteCommand();
            var list10= Db.Queryable<UnitAbc121>().ToList();
        }


        public class UnitAbc121
        {
            [SugarColumn(IsNullable =true)]
            public Guid? uid { get; set; }
            public string name { get; set; }
        }
        public static class IEnumerbleContains
        {
            public static IEnumerable<Order> Data()
            {
                for (int i = 0; i < 100; i++)
                {
                    yield return new Order
                    {
                        Id = i,
                    };
                }
            }
        }
        [SugarTable("UnitSaleOrder")]
        public class SaleOrder 
        {
            public SaleOrder()
            {
                SaleDate = DateTime.Now;
                Team = 1;
                AddTime = DateTime.Now;
                OrderStatus = 0;
                Points = 0;
                PayPoints = 0;
                PointsExchangeMoney = decimal.Zero;
                IsPushMessage = false;
                CostAmount = decimal.Zero;
                OrderAmount = decimal.Zero;
                RealOrderAmount = decimal.Zero;
                AccountsDueAmount = decimal.Zero;
                SettleType = 0;
                IsPushMessage = false;
            }

            /// <summary>
            /// 订单号
            /// </summary>
            public string OrderSn { get; set; }

            /// <summary>
            /// 客户编号
            /// </summary>
            public string CustomerNo { get; set; }


            /// <summary>
            /// 收货人姓名
            /// </summary>
            public string CustomerName { get; set; }

            /// <summary>
            /// 成本总金额
            /// </summary>
            public decimal CostAmount { get; set; }

            /// <summary>
            /// 订单总金额
            /// </summary>
            public decimal OrderAmount { get; set; }

            /// <summary>
            /// 实收金额（整单优惠后）
            /// </summary>
            public decimal RealOrderAmount { get; set; }

            /// <summary>
            /// 销货日期
            /// </summary>
            public DateTime SaleDate { get; set; }

            /// <summary>
            /// 下单时间
            /// </summary>
            public DateTime AddTime { get; set; }

            /// <summary>
            /// 媒体资源投放ID 
            /// </summary>
            public string IndustryCode { get; set; }

            public string IndustryName { get; set; }

            /// <summary>
            /// 备注
            /// </summary>
            public string Remark { get; set; }

            /// <summary>
            /// 班组
            /// </summary>
            public int Team { get; set; }

            /// <summary>
            /// 销售员编号
            /// </summary>
            public string SellerNo { get; set; }

            /// <summary>
            /// 销售员姓名
            /// </summary>
            public string SellerName { get; set; }

            /// <summary>
            /// 操作人ID
            /// </summary>
            public virtual string HandlerCode { get; set; }

            /// <summary>
            /// 操作者
            /// </summary>
            public string Handler { get; set; }

            /// <summary>
            /// 发货仓库代号
            /// </summary>
            public string StoreCode { get; set; }

            /// <summary>
            /// 发货仓库名称
            /// </summary>
            public string StoreName { get; set; }

            /// <summary>
            /// 销货店铺渠道代号
            /// </summary>
            public string ShopChannelCode { get; set; }

            /// <summary>
            /// 销货店铺渠道名称
            /// </summary>
            public string ShopChannelName { get; set; }

            /// <summary>
            /// 订单产品数
            /// </summary>
            public int GoodsNum { get; set; }

            /// <summary>
            /// 礼品数量
            /// </summary>
            public int GiftNum { get; set; }

            /// <summary>
            /// 对应预订单号
            /// </summary>
            public string CustomerOrderSn { get; set; }

            /// <summary>
            /// 订单赠送积分
            /// </summary>
            public int Points { get; set; }

            /// <summary>
            /// 应收款金额
            /// </summary>
            public decimal AccountsDueAmount { get; set; }

            /// <summary>
            /// 来自预约单号
            /// </summary>
            public string ReserationOrderSn { get; set; }


            /// <summary>
            /// 订单状态 0为未审核 1为已审核
            /// </summary>
            public int OrderStatus { get; set; }

            /// <summary>
            /// 审核人
            /// </summary>
            public string CheckMan { get; set; }

            /// <summary>
            /// 审核时间
            /// </summary>
            public DateTime? CheckTime { get; set; }

            /// <summary>
            /// 结算类型 0为非金工石（零售） 1为金工石 
            /// </summary>
            public int SettleType { get; set; }

            /// <summary>
            /// 使用积分
            /// </summary>
            public int PayPoints { get; set; }

            /// <summary>
            /// 积分抵现金额
            /// </summary>
            public decimal PointsExchangeMoney { get; set; }

            /// <summary>
            /// 是否已推送微信消息
            /// </summary>
            public bool IsPushMessage { get; set; }

        }

        public class SaleOrderBaseInfo
        {
            public int GoodsNum { get; set; }

            public int GiftNum { get; set; }

            public decimal OrderAmount { get; set; }

        }


        public class UnitTest3
        {
            public DateTime? Date { get; set; }
        }


        public class UnitSelectTest2
        {
            [SqlSugar.SugarColumn(IsOnlyIgnoreUpdate = true)]
            public decimal? DcNull { get; set; }
            public decimal Dc { get; set; }
            public int? IntNull { get; set; }
            [SqlSugar.SugarColumn(IsPrimaryKey = true)]
            public decimal Int { get; set; }
        }

        public class UnitSelectTest
        {
            public decimal? DcNull { get; set; }
            public decimal Dc { get; set; }
            public int? IntNull { get; set; }
            public decimal Int { get; set; }
        }

        public class UnitGuidTable
        {
            public Guid? Id { get; set; }
        }
    }
}
