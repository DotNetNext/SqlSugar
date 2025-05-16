using System;
using System.Collections.Generic;
using SqlSugar;

namespace ERP1.Entity
{
    [Serializable]
    [SugarTable("t_OrderDeliveryProduct")]
    public class OrderDeliveryProductEntity
    {
        #region # 数据库字段 #

        /// <summary>发货单商品Id</summary>
        public string DeliveryProductId { set; get; }

        /// <summary>发货单号</summary>
        public string DeliveryNo { set; get; }

        /// <summary>序号</summary>
        public Int32 Sequence { set; get; }

        /// <summary>订单商品Id</summary>
        public string OrderProductId { set; get; }

        /// <summary>商品数量</summary>
        public Int32 ProductNum { set; get; }

        /// <summary>单价¥</summary>
        public Decimal ProductPrice { set; get; }

        #endregion
        /// <summary>发货单商品编号</summary>
        [SugarColumn(IsIgnore = true)]
        public string DeliveryProductNo { get { return $"{DeliveryNo}-{Sequence}"; } }

        /// <summary>订单商品</summary>
        [SugarColumn(IsIgnore = true)]
        public OrderProductEntity OrderProductEntity { get; set; }

        /// <summary>发货单信息</summary>
        [SugarColumn(IsIgnore = true)]
        public OrderDeliveryEntity OrderDeliveryEntity { get; set; }

        /// <summary>客户信息</summary>
        [SugarColumn(IsIgnore = true)]
        public CustomerEntity CustomerEntity { get; set; }

        /// <summary>订单收货地址</summary>
        [SugarColumn(IsIgnore = true)]
        public AddressEntity OrderAddressEntitie { get; set; }

        /// <summary>采购价¥</summary>
        [SugarColumn(IsIgnore = true)]
        public decimal PurchaseAmount { get; set; }
    }

    public class OrderDeliveryEntity_P: OrderDeliveryEntity
    {

    }
}
