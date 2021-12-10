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

           
          //  Db.CodeFirst.InitTables<UnitTest0012>();
            Db.Insertable(new UnitTest0012() { ID = "a" }).ExecuteCommand();
            //Db.Insertable(new UnitTest0012() { ID = "b" }).ExecuteCommand();
            var x1 = Db.Ado.GetDataTable("SELECT  ID  FROM  UNITTEST0012     ", new SugarParameter("Const0", "a", System.Data.DbType.AnsiStringFixedLength));

     
            //Db.CurrentConnectionConfig.MoreSettings=new ConnMoreSettings { DisableNvarchar = true };
            var x = Db.Ado.GetDataTable("SELECT  ID  FROM  UNITTEST0012   WHERE ID = :Const0  ",new SugarParameter("Const0", "a",System.Data.DbType.AnsiStringFixedLength));
        }

        public class UnitTest0012
        {
            [SugarColumn(ColumnDataType ="char(10)")]
            public string ID { get; set; }
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
