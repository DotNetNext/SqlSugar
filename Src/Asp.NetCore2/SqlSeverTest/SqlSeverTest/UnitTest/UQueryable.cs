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
        public static Unit_SYS_USER UserLoginInfo => new Unit_SYS_USER() { XH = "10010" };
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
            UValidate.Check(sql, "SELECT  [Dc] AS [DcNull] , [Int] AS [Dc]  FROM [UnitSelectTest]", "Queryable");

            sql = Db.Updateable<UnitSelectTest2>(new UnitSelectTest2()).ToSql().Key;
            UValidate.Check(sql, @"UPDATE [UnitSelectTest2]  SET
           [Dc]=@Dc,[IntNull]=@IntNull  WHERE [Int]=@Int", "Queryable");

            sql = Db.Queryable<Order>().IgnoreColumns(it => it.CreateTime).ToSql().Key;
            UValidate.Check(sql, "SELECT [Id],[Name],[Price],[CustomId] FROM [Order] ", "Queryable");
            sql = Db.Queryable<Order>().IgnoreColumns(it => new { it.Id, it.Name }).ToSql().Key;
            UValidate.Check(sql, "SELECT [Price],[CreateTime],[CustomId] FROM [Order] ", "Queryable");
            sql = Db.Queryable<Order>().IgnoreColumns("id").ToSql().Key;
            UValidate.Check(sql, "SELECT [Name],[Price],[CreateTime],[CustomId] FROM [Order] ", "Queryable");

            var cts = IEnumerbleContains.Data();
            var list2 = Db.Queryable<Order>()
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

            var ids = Enumerable.Range(1, 11).ToList();
            var list8 = Db.Queryable<Order>().Where(it => SqlFunc.ContainsArrayUseSqlParameters(ids, it.Id)).ToList();

            var result2 = Db.Queryable<Unit_SYS_USER>().Where(o => o.XH == UserLoginInfo.XH).Select(o => o.XH).ToSql();

            var x = Db.Queryable<BoolTest1>().Select(it => new BoolTest2()
            {
                a = it.a
            }).ToSql();
            UValidate.Check(x.Key, "SELECT  [a] AS [a]  FROM [BoolTest1] ", "Queryable");
            x = Db.Queryable<BoolTest2>().Select(it => new BoolTest1()
            {
                a = it.a.Value
            }).ToSql();
            UValidate.Check(x.Key, "SELECT  [a] AS [a]  FROM [BoolTest2] ", "Queryable");

            var db = Db;
            db.CodeFirst.InitTables<UserInfo, UserIpRuleInfo>();
            db.Deleteable<UserInfo>().ExecuteCommand();
            db.Deleteable<UserIpRuleInfo>().ExecuteCommand();
            db.Insertable(new UserInfo()
            {
                Id = 1,
                Password = "123",
                UserName = "admin"
            }).ExecuteCommand();
            db.Insertable(new UserIpRuleInfo()
            {
                Addtime = DateTime.Now,
                UserName = "a",
                Id = 11,
                UserId = 1,
                Description = "xx",
                IpRange = "1",
                RuleType = 1
            }).ExecuteCommand();
            var vmList = db.Queryable<UserInfo, UserIpRuleInfo>(
                (m1, m2) => m1.Id == m2.UserId
            ).Where((m1, m2) => m1.Id > 0).Select((m1, m2) => new UserIpRuleInfo()
            {

                IpRange = m2.IpRange,
                Addtime = m2.Addtime,
                RuleType = m2.RuleType,
            }).ToList();
            if (string.IsNullOrEmpty(vmList.First().IpRange))
            {
                throw new Exception("Queryable");
            }

            Db.Insertable(new Order() { CreateTime=DateTime.Now, CustomId=1, Name="a",Price=1 }).ExecuteCommand();
            var sa = Db.SqlQueryable<Order>("SELECT * FroM [ORDER] where id in (@id) ");
            sa.AddParameters(new List<SugarParameter>() {
                new SugarParameter("id",new int[]{ 1})
             });
            int i = 0;
            var salist= sa.ToPageList(1,2,ref i);

            db.CodeFirst.InitTables<UnitBytes11>();
            db.Insertable(new UnitBytes11() { bytes = null, name = "a" }).ExecuteCommand();
            db.Insertable(new UnitBytes11() { bytes=new byte[] { 1,2} , name="a"}).ExecuteCommand();
            var bytes = db.Queryable<UnitBytes11>().Select(it => new
            {
                b = it.bytes,
                name="a"
            }).ToList();
        }


        
        public class UnitBytes11
        {
            [SugarColumn(Length =200,IsNullable =true)]
            public byte[] bytes { get; set; }
            public string name{ get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        [SugarTable("users")]
        public class UserInfo
        {
            /// <summary>
            /// 
            /// </summary>
            public int Id { get; set; }

            /// <summary>
            /// 
            /// </summary>

            public string UserName { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string Password { get; set; }

        }
        /// <summary>
        ///
        ///</summary>
        [Serializable]
        [SugarTable("user_ip_rules")]
        public class UserIpRuleInfo
        {
            /// <summary>
            /// 自增Id
            /// </summary>
            public int Id { get; set; }


            /// <summary>
            /// 用户Id
            /// </summary>
            [SugarColumn(ColumnName = "user_id")]
            public int UserId { get; set; }

            /// <summary>
            /// 用户名
            /// </summary>
            [SugarColumn(IsIgnore = true)]
            public string UserName { get; set; }

            /// <summary>
            /// IP地址或范围
            /// </summary>
            [SugarColumn(ColumnName = "ip_range")]
            public string IpRange { get; set; }


            /// <summary>
            /// 规则类型 0-黑名单 1-白名单
            /// </summary>
            [SugarColumn(ColumnName = "rule_type")]
            public int RuleType { get; set; }


            /// <summary>
            /// 描述/备注
            /// </summary>
            public string Description { get; set; }


            /// <summary>
            /// 添加时间
            /// </summary>
            public DateTime Addtime { get; set; }

        }
        /// <summary>
        /// 系统用户表实体模型类
        /// </summary>
        [SugarTable("Unit_SYS_USER")]

        public class Unit_SYS_USER
        {
            /// <summary>
            /// 序号
            /// </summary>
            private string _XH;

            /// <summary>
            /// 序号【主键唯一标识，自动生成】
            /// </summary>
            [SugarColumn(ColumnName = "XH",
                ColumnDataType = "VARCHAR2",
                IsPrimaryKey = true,
                IsNullable = false,
                Length = 50,
                ColumnDescription = "序号【主键唯一标识，自动生成】")]
            public string XH
            {
                get
                {
                    return _XH;
                }
                set
                {
                    _XH = value;
                }
            }
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

    internal class BoolTest1
    {
        public BoolTest1()
        {
        }

        public bool a { get; set; }
    }

    public class BoolTest2
    {
        public bool? a { get; set; }
    }
}
