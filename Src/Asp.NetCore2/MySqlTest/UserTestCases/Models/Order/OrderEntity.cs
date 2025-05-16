using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using ERP1.Entity;
using SqlSugar;

namespace ERP1.Entity
{
    [Serializable]
    [SugarTable("t_order")]
    public class OrderEntity: TeamBasePart
    {
        /// <summary>订单号</summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = false)]
        public string SaleNo { set; get; }

        /// <summary>发货团队</summary>
        public string WarehouseTeamId { set; get; }

        /// <summary>客户Id</summary>
        public string CustomerId { set; get; }

        /// <summary>订单类型</summary>
        public string OrderType { set; get; }

        /// <summary>订单来源</summary>
        public string OrderSource { set; get; }

        /// <summary>商品数量</summary>
        public Int32 ProductNum { set; get; }

        /// <summary>订单时间</summary>
        public DateTime OrderTime { set; get; }

        /// <summary>支付时间</summary>
        public DateTime PaidTime { set; get; }

        /// <summary>订单币种</summary>
        public string Currency { set; get; }

        /// <summary>订单金额</summary>
        public Decimal Amount { set; get; }

        /// <summary>订单折扣（统一填写负数）</summary>
        private Decimal discount;
        public Decimal Discount
        {
            set { if (value > 0) { throw new ArgumentException("订单折扣不能大于0"); } this.discount = value; }
            get { return discount; }
        }

        /// <summary>订单运费</summary>
        public Decimal LogisticeAmt { set; get; }

        /// <summary>订单状态</summary>
        public string OrderStatus { set; get; }

        /// <summary>发货状态</summary>
        public string DeliveryStatus { set; get; }

        /// <summary>异常状态</summary>
        public string AbnormalStatus { set; get; }

        /// <summary>是否需要质检</summary>
        public string? NeedQC { set; get; }

        /// <summary>质检图上传状态</summary>
        public string? QCPhotoStatus { set; get; }

        /// <summary>质检状态</summary>
        public string? QCStatus { set; get; }

        /// <summary>客户备注</summary>
        public string? CustomerRemark { set; get; }

        /// <summary>内部备注</summary>
        public string? InnerRemark { set; get; }

        /// <summary>创建时间</summary>
        public DateTime AddTime { set; get; }

        #region 客户信息
        /// <summary>客户信息</summary>
        [SugarColumn(IsIgnore = true)]
        [Navigate(NavigateType.OneToOne, nameof(ERP1.Entity.OrderEntity.CustomerId), nameof(ERP1.Entity.CustomerEntity.CustomerId))]
        public CustomerEntity CustomerEntity { get; set; }
        #endregion

        #region 订单款项
        /// <summary>订单款项(支付)</summary>
        [SugarColumn(IsIgnore = true)]
        public OrderAmtEntity OrderAmtEntity_Order { get; set; }

        /// <summary>订单款项(售后)</summary>
        [SugarColumn(IsIgnore = true)]
        public List<OrderAmtEntity> OrderAmtEntity_Service { get; set; }
        #endregion

        /// <summary>订单收货地址</summary>
        [SugarColumn(IsIgnore = true)]
        [Navigate(NavigateType.OneToMany, nameof(AddressEntity.RefId))]
        public List<AddressEntity> OrderAddressEntities { get; set; }

        /// <summary>订单收货地址</summary>
        [SugarColumn(IsIgnore = true)]
        public AddressEntity OrderAddressEntitie
        { 
            get 
            {
                if (this.OrderAddressEntities?.Count > 0)
                {
                    return this.OrderAddressEntities[0];
                }
                return null;
            } 
        }

        /// <summary>订单商品</summary>
        [SugarColumn(IsIgnore = true)]
        [Navigate(NavigateType.OneToMany, nameof(OrderProductEntity.SaleNo))]
        public List<OrderProductEntity> OrderProductEntities { get; set; }


        /// <summary>订单款项</summary>
        [SugarColumn(IsIgnore = true)]
        [Navigate(NavigateType.OneToMany, nameof(ERP1.Entity.OrderAmtEntity.SaleNo))]
        public List<OrderAmtEntity> OrderAmtEntitys { get; set; }

        /// <summary>订单发货单</summary>
        [SugarColumn(IsIgnore = true)]
        [Navigate(NavigateType.OneToMany, nameof(ERP1.Entity.OrderDeliveryEntity.SaleNo))]
        public List<OrderDeliveryEntity> OrderDeliveryEntitys { get; set; }

       

        /// <summary>订单币种名称</summary>
        [SugarColumn(IsIgnore = true)]
        public string CurrencyName { set; get; }

        /// <summary>订单币种符号</summary>
        [SugarColumn(IsIgnore = true)]
        public string CurrencySymbol { set; get; }

        /// <summary>采购金额</summary>
        [SugarColumn(IsIgnore = true)]
        public decimal PurchaseAmount { set; get; }

    }

    /// <summary>
    /// 店先生
    /// </summary>
    public class MrShopPlus
    {
        /// <summary>
        /// 店先生url
        /// </summary>
        public string Url { get; set; }
    }

    /// <summary>
    /// 万全运
    /// </summary>
    public class Wangquanyun
    {
        /// <summary>
        /// 万全运url
        /// </summary>
        public string Url { get; set; }
    }





    public class TeamBasePart
    {
        /// <summary>业务团队</summary>
        public string BusinessTeamId { set; get; }

        /// <summary>业务网站</summary>
        public string WebsiteId { set; get; }

        /// <summary>客户经理</summary>
        public string? BusinesserId { set; get; }

        /// <summary>业务团队名称</summary>
        [SugarColumn(IsIgnore = true)]
        public string BusinessTeamName { set; get; }

        /// <summary>网站名称</summary>
        [SugarColumn(IsIgnore = true)]
        public string WebsiteName { set; get; }

        /// <summary>客户经理名称</summary>
        [SugarColumn(IsIgnore = true)]
        public string BusinesserName { set; get; }


    }
}

