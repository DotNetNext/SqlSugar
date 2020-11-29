using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HONORCSData.Goods
{
    /// <summary>
    /// 商品种类 
    /// </summary>
    [SugarTable("goods_class")]
    public class EGoodsClass
    {
        private System.Int32 _GoodsClassId;
        /// <summary>
        /// 商品种类编号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true,ColumnName = "goods_class_id")]
        public System.Int32 GoodsClassId { get { return this._GoodsClassId; } set { this._GoodsClassId = value; } }

        private System.String _SpanishName;
        /// <summary>
        /// 西文名称
        /// </summary> 
        [SugarColumn(ColumnName = "spanish_name")]
        public System.String SpanishName { get { return this._SpanishName; } set { this._SpanishName = value?.Trim(); } }

        private System.String _ChineseName;
        /// <summary>
        /// 中文名称
        /// </summary> 
        [SugarColumn(ColumnName = "chinese_name")]
        public System.String ChineseName { get { return this._ChineseName; } set { this._ChineseName = value?.Trim(); } }

        private System.Int16 _IvaId;
        /// <summary>
        /// iva编号
        /// </summary> 
        [SugarColumn(ColumnName = "iva_id")]
        public System.Int16 IvaId { get { return this._IvaId; } set { this._IvaId = value; } }


        private System.String _GoodsClassNo;
        /// <summary>
        /// 编号
        /// </summary> 
        [SugarColumn(ColumnName = "goods_class_no")]
        public System.String GoodsClassNo { get { return this._GoodsClassNo; } set { this._GoodsClassNo = value?.Trim(); } }

        private System.Decimal _Discount;
        /// <summary>
        /// 折扣
        /// </summary> 
        [SugarColumn(ColumnName = "discount")]
        public System.Decimal Discount { get { return this._Discount; } set { this._Discount = value; } }

        private System.Decimal _ReceiptPercentage;
        /// <summary>
        /// 发票百分比
        /// </summary> 
        [SugarColumn(ColumnName = "receipt_percentage")]
        public System.Decimal ReceiptPercentage { get { return this._ReceiptPercentage; } set { this._ReceiptPercentage = value; } }

        private System.Boolean _IsOnlyPerUnit;
        /// <summary>
        /// 是否仅每单位
        /// </summary> 
        [SugarColumn(ColumnName = "is_only_per_unit")]
        public System.Boolean IsOnlyPerUnit { get { return this._IsOnlyPerUnit; } set { this._IsOnlyPerUnit = value; } }

        private System.String _UnitName;
        /// <summary>
        /// 单位名称
        /// </summary> 
        [SugarColumn(ColumnName = "unit_name")]
        public System.String UnitName { get { return this._UnitName; } set { this._UnitName = value?.Trim(); } }

        private System.Boolean _IsCountEarning;
        /// <summary>
        /// 是否计算收益
        /// </summary> 
        [SugarColumn(ColumnName = "is_count_earning")]
        public System.Boolean IsCountEarning { get { return this._IsCountEarning; } set { this._IsCountEarning = value; } }

        private System.Boolean _IsCountStock;
        /// <summary>
        /// 是否计算库存
        /// </summary> 
        [SugarColumn(ColumnName = "is_count_stock")]
        public System.Boolean IsCountStock { get { return this._IsCountStock; } set { this._IsCountStock = value; } }

        private System.Boolean _IsProhibitedChangeDiscount;
        /// <summary>
        /// 是否禁止更改折扣
        /// </summary> 
        [SugarColumn(ColumnName = "is_prohibited_change_discount")]
        public System.Boolean IsProhibitedChangeDiscount { get { return this._IsProhibitedChangeDiscount; } set { this._IsProhibitedChangeDiscount = value; } }

        private System.Boolean _IsGift;
        /// <summary>
        /// 是否为赠品
        /// </summary> 
        [SugarColumn(ColumnName = "is_gift")]
        public System.Boolean IsGift { get { return this._IsGift; } set { this._IsGift = value; } }

        private System.Boolean _IsPrivate;
        /// <summary>
        /// 是否私营
        /// </summary> 
        [SugarColumn(ColumnName = "is_private")]
        public System.Boolean IsPrivate { get { return this._IsPrivate; } set { this._IsPrivate = value; } }

        private System.Int16 _GoodsStorehouse;
        /// <summary>
        /// 商品仓库
        /// </summary> 
        [SugarColumn(ColumnName = "goods_storehouse")]
        public System.Int16 GoodsStorehouse { get { return this._GoodsStorehouse; } set { this._GoodsStorehouse = value; } }

        private System.Decimal _RetailPriceProfitPercentage;
        /// <summary>
        /// 零售价利润百分比
        /// </summary> 
        [SugarColumn(ColumnName = "retail_price_profit_percentage")]
        public System.Decimal RetailPriceProfitPercentage { get { return this._RetailPriceProfitPercentage; } set { this._RetailPriceProfitPercentage = value; } }

        private System.Decimal _DeliveryPriceProfitPercentage;
        /// <summary>
        /// 送货价收益百分比
        /// </summary> 
        [SugarColumn(ColumnName = "delivery_price_profit_percentage")]
        public System.Decimal DeliveryPriceProfitPercentage { get { return this._DeliveryPriceProfitPercentage; } set { this._DeliveryPriceProfitPercentage = value; } }

        private System.Decimal _MemberPriceProfitPercentage;
        /// <summary>
        /// 会员价百分比
        /// </summary> 
        [SugarColumn(ColumnName = "member_price_profit_percentage")]
        public System.Decimal MemberPriceProfitPercentage { get { return this._MemberPriceProfitPercentage; } set { this._MemberPriceProfitPercentage = value; } }

        private System.Decimal _WholesalePriceProfitPercentage;
        /// <summary>
        /// 批发价收益百分比
        /// </summary> 
        [SugarColumn(ColumnName = "wholesale_price_profit_percentage")]
        public System.Decimal WholesalePriceProfitPercentage { get { return this._WholesalePriceProfitPercentage; } set { this._WholesalePriceProfitPercentage = value; } }

        private System.Decimal _ReceiptPriceProfitPercentage;
        /// <summary>
        /// 发票价格收益百分比
        /// </summary> 
        [SugarColumn(ColumnName = "receipt_price_profit_percentage")]
        public System.Decimal ReceiptPriceProfitPercentage { get { return this._ReceiptPriceProfitPercentage; } set { this._ReceiptPriceProfitPercentage = value; } }

        private System.Decimal _InternetPriceProfitPercentage;
        /// <summary>
        /// 网络价格收益百分比
        /// </summary> 
        [SugarColumn(ColumnName = "internet_price_profit_percentage")]
        public System.Decimal InternetPriceProfitPercentage { get { return this._InternetPriceProfitPercentage; } set { this._InternetPriceProfitPercentage = value; } }

        private System.Decimal _FriendshipPriceProfitPercentage;
        /// <summary>
        /// 友情价收益百分比
        /// </summary> 
        [SugarColumn(ColumnName = "friendship_price_profit_percentage")]
        public System.Decimal FriendshipPriceProfitPercentage { get { return this._FriendshipPriceProfitPercentage; } set { this._FriendshipPriceProfitPercentage = value; } }

        private System.Decimal _SpecialPriceProfitPercentage;
        /// <summary>
        /// 特别价格收益百分比
        /// </summary> 
        [SugarColumn(ColumnName = "special_price_profit_percentage")]
        public System.Decimal SpecialPriceProfitPercentage { get { return this._SpecialPriceProfitPercentage; } set { this._SpecialPriceProfitPercentage = value; } }

        private System.Decimal _PromotionPriceProfitPercentage;
        /// <summary>
        /// 促销价格收益百分比
        /// </summary> 
        [SugarColumn(ColumnName = "promotion_price_profit_percentage")]
        public System.Decimal PromotionPriceProfitPercentage { get { return this._PromotionPriceProfitPercentage; } set { this._PromotionPriceProfitPercentage = value; } }

        private System.Boolean _IsLock;
        /// <summary>
        /// 是否锁定
        /// </summary> 
        [SugarColumn(ColumnName = "is_lock")]
        public System.Boolean IsLock { get { return this._IsLock; } set { this._IsLock = value; } }

        private System.String _Remark;
        /// <summary>
        /// 备注
        /// </summary> 
        [SugarColumn(ColumnName = "remark")]
        public System.String Remark { get { return this._Remark; } set { this._Remark = value?.Trim(); } }

        private System.Int32 _ParentId;
        /// <summary>
        /// 上级分类id
        /// </summary> 
        [SugarColumn(ColumnName = "parent_id")]
        public System.Int32 ParentId { get { return this._ParentId; } set { this._ParentId = value; } }

        private System.String _IsEnablePoints;
        /// <summary>
        /// 是否启用积分:1 启用 0 不启用
        /// </summary> 
        [SugarColumn(ColumnName = "is_enable_points")]
        public System.String IsEnablePoints { get { return this._IsEnablePoints; } set { this._IsEnablePoints = value?.Trim(); } }

        private System.String _PointsRule;
        /// <summary>
        /// 积分规则
        /// </summary> 
        [SugarColumn(ColumnName = "points_rule")]
        public System.String PointsRule { get { return this._PointsRule; } set { this._PointsRule = value?.Trim(); } }

        private System.String _ValuationMethod;
        /// <summary>
        /// 计价方式：1 包装 2 称重
        /// </summary> 
        [SugarColumn(ColumnName = "valuation_method")]
        public System.String ValuationMethod { get { return this._ValuationMethod; } set { this._ValuationMethod = value?.Trim(); } }

        private System.String _DelFlag;
        /// <summary>
        /// 删除标记：1 删除 0 未删除
        /// </summary> 
        [SugarColumn(ColumnName = "del_flag")]
        public System.String DelFlag { get { return this._DelFlag; } set { this._DelFlag = value?.Trim(); } }

        private System.String _IsShowOnCshier = "1";
        /// <summary>
        /// 收银前台是否显示:1 显示 0 不显示
        /// </summary> 
        [SugarColumn(ColumnName = "is_show_on_cashier")]
        public System.String IsShowOnCshier { get { return this._IsShowOnCshier; } set { this._IsShowOnCshier = value?.Trim(); } }

        private System.Int32 _TenantId;
        /// <summary>
        /// 所属租户
        /// </summary> 
        [SugarColumn(ColumnName = "tenant_id")]
        public System.Int32 TenantId { get { return this._TenantId; } set { this._TenantId = value; } }
    }
    public class EGoodsClassTree : EGoodsClass
    {
        /// <summary>
        /// 子集
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public List<EGoodsClassTree> Childrens { get; set; }
        /// <summary>
        /// 是否选中
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public bool IsSelected { get; set; } = false;
    }
}
