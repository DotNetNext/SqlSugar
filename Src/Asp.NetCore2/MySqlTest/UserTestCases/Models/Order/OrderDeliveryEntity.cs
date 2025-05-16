using System;
using System.Collections.Generic;
using ERP1.Entity;
using SqlSugar;

namespace ERP1.Entity
{
    [Serializable]
    [SugarTable("t_OrderDelivery")]
    public class OrderDeliveryEntity
    {
        #region # 数据库字段 #

        /// <summary>发货单号</summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = false)]
        public string DeliveryNo { set; get; }

        /// <summary>订单号</summary>
        public string SaleNo { set; get; }

        /// <summary>揽收单号</summary>
        public string? ReceiveNo { set; get; }

        /// <summary>业务备注</summary>
        public string? Remark { set; get; }

        /// <summary>创建人Id</summary>
        public string AdderId { set; get; }

        /// <summary>创建时间</summary>
        public DateTime AddTime { set; get; }

        /// <summary>发货时间</summary>
        public DateTime? SendTime { set; get; }

        /// <summary>发货备注</summary>
        public string? DeliveryRemark { set; get; }

        /// <summary>关闭原因</summary>
        public string? CloseRemark { set; get; }

        /// <summary>货代公司</summary>
        public string Forwarder { set; get; }

        /// <summary>物流公司</summary>
        public string LogisticsProviderId { set; get; }

        /// <summary>物流单号</summary>
        public string? LogisticeNo { set; get; }

        /// <summary>物流费用¥</summary>
        public Decimal? LogisticsFee { set; get; }

        /// <summary>物流更新时间</summary>
        public DateTime? LastUpdateTime { set; get; }

        /// <summary>物流未更新天数</summary>
        public Int32? NotUpdateDays { set; get; }

        /// <summary>中转日期</summary>
        public DateTime? TransitDate { set; get; }

        /// <summary>中转耗时(天)</summary>
        public Int32? TransitSpent { set; get; }

        /// <summary>上网日期</summary>
        public DateTime? OnlineDate { set; get; }

        /// <summary>上网耗时(天)</summary>
        public Int32? OnlineSpent { set; get; }

        /// <summary>订单耗时(天)</summary>
        public Int32? OrderSpent { set; get; }

        /// <summary>运输耗时(天)</summary>
        public Int32? CarrySpent { set; get; }

        /// <summary>商城物流记录Id</summary>
        public string? ExtDeliveryNo { get; set; }

        /// <summary>同步结果描述</summary>
        public string? SyncMsg { get; set; }

        #endregion


        /// <summary>物流商名称</summary>
        [SugarColumn(IsIgnore = true)]
        public string? LogisticsProviderName { set; get; }

        /// <summary>货代公司名称</summary>
        [SugarColumn(IsIgnore = true)]
        public string? ForwarderName { set; get; }

        /// <summary>订单信息</summary>
        [SugarColumn(IsIgnore = true)]
        public OrderEntity OrderEntity { get; set; }

        /// <summary>发货商品数量</summary>
        [SugarColumn(IsIgnore = true)]
        public int? ProductNum { set; get; }

        /// <summary>发货商品信息</summary>
        [SugarColumn(IsIgnore = true)]
        [Navigate(NavigateType.OneToMany, nameof(ERP1.Entity.OrderDeliveryProductEntity.DeliveryNo))]
        public List<OrderDeliveryProductEntity> OrderDeliveryProductEntities { get; set; }

        /// <summary>发货地址</summary>
        [SugarColumn(IsIgnore = true)]
        [Navigate(NavigateType.OneToMany, nameof(AddressEntity.RefId))]
        public List<AddressEntity> OrderAddressEntities { get; set; }
        
        /// <summary>发货地址</summary>
        [SugarColumn(IsIgnore = true)]
        public AddressEntity OrderAddressEntity
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

        /// <summary>订单发货单物流单号历史</summary>
        [SugarColumn(IsIgnore = true)]
        [Navigate(NavigateType.OneToMany, nameof(ERP1.Entity.OrderDeliveryLogHisEntity.DeliveryNo))]
        public List<OrderDeliveryLogHisEntity> OrderDeliveryLogHisEntitys { get; set; }

    }

    /// <summary>货代公司信息</summary>
    public class Forwarder
    {
        /// <summary>地址</summary>
        public string Url { get; set; }
    }

    /// <summary>畅安达货代公司信息</summary>
    public class ChangandaForwarder: Forwarder
    {

    }

    /// <summary>啊督货代公司信息</summary>
    public class AduForwarder : Forwarder
    {

    }

    /// <summary>
    /// 售后申请，责任款项下拉发货单号、物流单号产品信息
    /// </summary>

    public class ProductInfoOfOrderDelivery
    {
        public List<ProductInfoOfDeliveryNo> ProductInfoOfDeliveryNos { get; set; }

        public List<ProductInfoOfLogisticeNo> ProductInfoOfLogisticeNos { get; set; }
    }

    /// <summary>
    /// 基础发货单商品信息
    /// </summary>
    public class BaseProductInfoOfOrderDelivery
    {
        public string Id { get; set; }

        public string ProductBatchName { get; set; }

        public string ProductColor { get; set; }

        public string ProductSize { get; set; }
    }

    /// <summary>
    /// 发货单号商品信息
    /// </summary>
    public class ProductInfoOfDeliveryNo: BaseProductInfoOfOrderDelivery
    {
        public string DeliveryNo { get; set; }
    }

    /// <summary>
    /// 物流单号商品信息
    /// </summary>
    public class ProductInfoOfLogisticeNo : BaseProductInfoOfOrderDelivery
    {
        public string LogisticeNo { get; set; }
    }
    

}
