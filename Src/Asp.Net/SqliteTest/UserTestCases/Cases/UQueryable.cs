using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

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

            Db.CodeFirst.InitTables<UnitAbc121>();
            Db.Insertable(new UnitAbc121() { name = "a", uid = null }).ExecuteCommand();
            Db.Insertable(new UnitAbc121() { name = "a", uid = Guid.NewGuid() }).ExecuteCommand();
            var list10 = Db.Queryable<UnitAbc121>().ToList();

            var count = Db.Queryable<Order>()
                .Where(z => z.Id == SqlFunc.Subqueryable<Order>()
                .GroupBy(x => x.Id).Select(x => x.Id))
                .Count();

            if (count != Db.Queryable<Order>().Count())
            {
                throw new Exception("unit error");
            }

            List<IConditionalModel> conModels = new List<IConditionalModel>();
            conModels.Add(new ConditionalModel()
            {
                FieldName = "name",
                FieldValue = "1",
                CustomConditionalFunc = new MyConditional()
            });
            conModels.Add(new ConditionalModel() { FieldName = "id", ConditionalType = ConditionalType.Like, FieldValue = "1" });
            var list8 = Db.Queryable<Order>().Where(conModels).ToList();
            Db.Queryable<Order>()
                .Select(it => new
                {
                    time = SqlFunc.Subqueryable<OrderItem>()
                     .Where(s => s.OrderId == it.Id)
                     .Select(s => SqlFunc.IF(s.CreateTime <= SqlFunc.DateAdd(it.CreateTime, 15, DateType.Minute)).Return(1).End(0)) 
                }).ToList();
            Db.CodeFirst.InitTables<sign_rule, sign_info>();
            var list9 = Db.Queryable<sign_rule>()
              .Where(it => it.skey == "30-30")
              .Select(item => new
              {
                  type = SqlFunc.Subqueryable<sign_info>().Where(s => s.sid == item.sid && s.mno == "123456789" && s.uno == "7d8b1d8752a7c3b3").Any()
              })
              .ToListAsync().GetAwaiter().GetResult();
        }

        public class MyConditional : ICustomConditionalFunc
        {
            public KeyValuePair<string,SugarParameter[]> GetConditionalSql(ConditionalModel conditionalModel,int index)
            {
                var parameterName= "@myp" + index;
                SugarParameter[] pars = new SugarParameter[] 
                {
                     new SugarParameter(parameterName, conditionalModel.FieldValue )
                };
                //自已处理字符串安全，也可以使用我自带的
                return new KeyValuePair<string, SugarParameter[]>
                    ($" { conditionalModel.FieldName.ToCheckRegexW() } = {parameterName}", pars);
            }
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
        ///签到规则
        ///</summary>
        [SugarTable("sign_rule")]
        public class sign_rule
        {
            public sign_rule()
            {
            }

            /// <summary>
            /// 描述 : 关键标识（可重复） 
            /// 空值 : False
            /// 默认 : 
            /// </summary>
            [Display(Name = "关键标识（可重复）")]
            public string skey { get; set; }

            /// <summary>
            /// 描述 : 名称 
            /// 空值 : True
            /// 默认 : 
            /// </summary>
            [Display(Name = "名称")]
            public string name { get; set; }

            /// <summary>
            /// 描述 : 前端显示校验使用 
            /// 空值 : True
            /// 默认 : 
            /// </summary>
            [Display(Name = "前端显示校验使用")]
            public string check_json { get; set; }

            /// <summary>
            /// 描述 : 签到规则 
            /// 空值 : True
            /// 默认 : 
            /// </summary>
            [Display(Name = "签到规则")]
            public string rule_json { get; set; }

            /// <summary>
            /// 描述 : 开始时间 
            /// 空值 : True
            /// 默认 : 
            /// </summary>
            [Display(Name = "开始时间")]
            public DateTime? stime { get; set; }

            /// <summary>
            /// 描述 : 结束时间 
            /// 空值 : True
            /// 默认 : 
            /// </summary>
            [Display(Name = "结束时间")]
            public DateTime? etime { get; set; }

            /// <summary>
            /// 描述 : 更新时间 
            /// 空值 : True
            /// 默认 : 
            /// </summary>
            [Display(Name = "更新时间")]
            public DateTime? updatetime { get; set; }

            /// <summary>
            /// 描述 : 自增ID 
            /// 空值 : False
            /// 默认 : 
            /// </summary>
            [Display(Name = "自增ID")]
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int sid { get; set; }

            /// <summary>
            /// 描述 : 签到按钮图片 
            /// 空值 : True
            /// 默认 : 
            /// </summary>
            [Display(Name = "签到按钮图片")]
            public string imgurl { get; set; }

        }
        ///<summary>
        ///签到详情
        ///</summary>
        [SugarTable("sign_info")]
        public class sign_info
        {
            public sign_info()
            {
            }

            /// <summary>
            /// 描述 :  
            /// 空值 : False
            /// 默认 : 
            /// </summary>
            [Display(Name = "")]
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int signid { get; set; }

            /// <summary>
            /// 描述 : 与user表uno对应 
            /// 空值 : False
            /// 默认 : 
            /// </summary>
            [Display(Name = "与user表uno对应")]
            public string uno { get; set; }

            /// <summary>
            /// 描述 : 会议编号 
            /// 空值 : False
            /// 默认 : 
            /// </summary>
            [Display(Name = "会议编号")]
            public string mno { get; set; }

            /// <summary>
            /// 描述 : 姓名 
            /// 空值 : True
            /// 默认 : 
            /// </summary>
            [Display(Name = "姓名")]
            public string name { get; set; }

            /// <summary>
            /// 描述 : 电话 
            /// 空值 : True
            /// 默认 : 
            /// </summary>
            [Display(Name = "电话")]
            public string phone { get; set; }

            /// <summary>
            /// 描述 : 性别 
            /// 空值 : True
            /// 默认 : 
            /// </summary>
            [Display(Name = "性别")]
            public string sex { get; set; }

            /// <summary>
            /// 描述 : 生日 
            /// 空值 : True
            /// 默认 : 
            /// </summary>
            [Display(Name = "生日")]
            public string birthday { get; set; }

            /// <summary>
            /// 描述 : 身份证 
            /// 空值 : True
            /// 默认 : 
            /// </summary>
            [Display(Name = "身份证")]
            public string idcard { get; set; }

            /// <summary>
            /// 描述 : 学历 
            /// 空值 : True
            /// 默认 : 
            /// </summary>
            [Display(Name = "学历")]
            public string education { get; set; }

            /// <summary>
            /// 描述 : 医院 
            /// 空值 : True
            /// 默认 : 
            /// </summary>
            [Display(Name = "医院")]
            public string hospital { get; set; }

            /// <summary>
            /// 描述 : 医院所在省份城市 
            /// 空值 : True
            /// 默认 : 
            /// </summary>
            [Display(Name = "医院所在省份城市")]
            public string province_city { get; set; }

            /// <summary>
            /// 描述 : 科室 
            /// 空值 : True
            /// 默认 : 
            /// </summary>
            [Display(Name = "科室")]
            public string department { get; set; }

            /// <summary>
            /// 描述 : 职称 
            /// 空值 : True
            /// 默认 : 
            /// </summary>
            [Display(Name = "职称")]
            public string title { get; set; }

            /// <summary>
            /// 描述 : 是否授予学分 
            /// 空值 : True
            /// 默认 : 
            /// </summary>
            [Display(Name = "是否授予学分")]
            public string iscredit { get; set; }

            /// <summary>
            /// 描述 : 最后更新时间 
            /// 空值 : True
            /// 默认 : 
            /// </summary>
            [Display(Name = "最后更新时间")]
            public DateTime? updatetime { get; set; }

            /// <summary>
            /// 描述 : 规则ID（后台获取） 
            /// 空值 : False
            /// 默认 : 
            /// </summary>
            [Display(Name = "规则ID（后台获取）")]
            public int sid { get; set; }

        }
    }
}
