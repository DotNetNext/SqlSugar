using System;
using System.Collections.Generic;
using ERP1.Entity;
using SqlSugar;

namespace ERP1.Entity
{
    [Serializable]
    [SugarTable("t_orderproduct")]
    public class OrderProductEntity
    {
        #region # 数据库字段 #

        /// <summary>订单商品Id</summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = false)]
        public string OrderProductId { set; get; }

        /// <summary>订单号</summary>
        public string SaleNo { set; get; }

        /// <summary>序号</summary>
        public int Sequence { set; get; }

        /// <summary>名称</summary>
        public string Name { set; get; }

        /// <summary>产品批次名称</summary>
        public string? BatchName { set; get; }

        /// <summary>图片</summary>
        public string? ImagePath { set; get; }

        /// <summary>尺码(商城订单同步)</summary>
        public string Specifications { set; get; }

        /// <summary>颜色</summary>
        public string? Color { set; get; }

        /// <summary>产品批次尺码</summary>
        public string? Size { set; get; }

        /// <summary>来源</summary>
        public string? Source { set; get; }

        /// <summary>特殊需求</summary>
        public string? SpecialRequire { set; get; }

        /// <summary>是否需要质检</summary>
        public Boolean? NeedQC { set; get; }

        /// <summary>质检确认状态</summary>
        public string? QCStatus { set; get; }

        /// <summary>拍照时间</summary>
        public DateTime? TakePhotoTime { set; get; }

        /// <summary>质检时间</summary>
        public DateTime? QCTime { set; get; }

        /// <summary>商品类型</summary>
        public string ProductType { set; get; }

        /// <summary>品类</summary>
        public string? Category { set; get; }

        /// <summary>商品单价</summary>
        public Decimal ProductPrice { set; get; }

        /// <summary>采购单价¥</summary>
        public Decimal PurchasePrice { set; get; }

        /// <summary>商品数量</summary>
        public Int32 ProductNum { set; get; }

        /// <summary>已发货数</summary>
        public Int32 DeliveryNum { set; get; }

        /// <summary>取消发货数</summary>
        public Int32 CancelDeliveryNum { set; get; }

        /// <summary>售后次数</summary>
        public Int32 ServiceNum { set; get; }

        /// <summary>是否质检售后</summary>
        public Boolean? HasQCService { set; get; }

        /// <summary>备注</summary>
        public string? Remark { set; get; }

        /// <summary>商城订单商品Id(此字段主要用于后续同步物流信息到店先生)</summary>
        public string? ExtOrderProId { get; set; }

        #endregion
        /// <summary>订单信息</summary>
        [SugarColumn(IsIgnore = true)]
        public OrderEntity OrderEntity { get; set; }

      


    }
}
