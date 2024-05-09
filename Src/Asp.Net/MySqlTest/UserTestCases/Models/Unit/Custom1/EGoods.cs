using SqlSugar;

namespace HONORCSData
{

    /// <summary>
    /// 商品主表 
    /// </summary>
    [SugarTable("goods")]
    public class EGoods
    {
        /// <summary>
        /// 商品 
        /// </summary>
        public EGoods()
        {
        }
        private int _GoodsId;
        /// <summary>
        /// 商品id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnName = "goods_id")]
        public int GoodsId { get => _GoodsId; set => _GoodsId = value; }
        private string _GoodsNo;
        /// <summary>
        /// 商品编号
        /// </summary>
        [SugarColumn( ColumnName = "goods_no")]
        public string GoodsNo { get => _GoodsNo; set => _GoodsNo = value?.Trim(); }

        private string _BarCode;
        /// <summary>
        /// 条码
        /// </summary> 
        [SugarColumn(ColumnName = "bar_code")]
        public string BarCode { get => _BarCode; set => _BarCode = value?.Trim(); }

        private string _Description;
        /// <summary>
        /// 描述
        /// </summary> 
        [SugarColumn(ColumnName = "description")]
        public string Description { get => _Description; set => _Description = value?.Trim(); }

        private decimal _RetailPrice;
        /// <summary>
        /// 零售价
        /// </summary> 
        [SugarColumn(ColumnName = "retail_price")]
        public decimal RetailPrice { get => _RetailPrice; set => _RetailPrice = value; }

        private decimal _DeliveryPrice;
        /// <summary>
        /// 送货价
        /// </summary> 
        [SugarColumn(ColumnName = "delivery_price")]
        public decimal DeliveryPrice { get => _DeliveryPrice; set => _DeliveryPrice = value; }

        private decimal _MemberPrice;
        /// <summary>
        /// 会员价
        /// </summary> 
        [SugarColumn(ColumnName = "member_price")]
        public decimal MemberPrice { get => _MemberPrice; set => _MemberPrice = value; }

        private decimal _WholesalePrice;
        /// <summary>
        /// 批发价
        /// </summary> 
        [SugarColumn(ColumnName = "wholesale_price")]
        public decimal WholesalePrice { get => _WholesalePrice; set => _WholesalePrice = value; }

        private decimal _ReceiptPrice;
        /// <summary>
        /// 发票价
        /// </summary> 
        [SugarColumn(ColumnName = "receipt_price")]
        public decimal ReceiptPrice { get => _ReceiptPrice; set => _ReceiptPrice = value; }

        private decimal _InternetPrice;
        /// <summary>
        /// 网络价
        /// </summary> 
        [SugarColumn(ColumnName = "internet_price")]
        public decimal InternetPrice { get => _InternetPrice; set => _InternetPrice = value; }

        private decimal _FriendshipPrice;
        /// <summary>
        /// 友情价
        /// </summary> 
        [SugarColumn(ColumnName = "friendship_price")]
        public decimal FriendshipPrice { get => _FriendshipPrice; set => _FriendshipPrice = value; }

        private decimal _SpecialPrice;
        /// <summary>
        /// 特别价
        /// </summary> 
        [SugarColumn(ColumnName = "special_price")]
        public decimal SpecialPrice { get => _SpecialPrice; set => _SpecialPrice = value; }

        private decimal _PromotionPrice;
        /// <summary>
        /// 促销价
        /// </summary> 
        [SugarColumn(ColumnName = "promotion_price")]
        public decimal PromotionPrice { get => _PromotionPrice; set => _PromotionPrice = value; }

        private decimal _PurchasePrice;
        /// <summary>
        /// 进货价
        /// </summary> 
        [SugarColumn(ColumnName = "purchase_price")]
        public decimal PurchasePrice { get => _PurchasePrice; set => _PurchasePrice = value; }

        private string _SpanishName;
        /// <summary>
        /// 西文名称
        /// </summary> 
        [SugarColumn(ColumnName = "spanish_name")]
        public string SpanishName { get => _SpanishName; set => _SpanishName = value?.Trim(); }

        private string _Initials;
        /// <summary>
        /// 首字母
        /// </summary> 
        [SugarColumn(ColumnName = "initials")]
        public string Initials { get => _Initials; set => _Initials = value?.Trim(); }

        private string _ChineseName;
        /// <summary>
        /// 中文名称
        /// </summary> 
        [SugarColumn(ColumnName = "chinese_name")]
        public string ChineseName { get => _ChineseName; set => _ChineseName = value?.Trim(); }

        /// <summary>
        /// 商品名称 西文中文组合
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string GoodsName { get => string.Format("{0}[{1}]", _SpanishName, _ChineseName); }

        private string _ChinesePinyin;
        /// <summary>
        /// 拼音
        /// </summary> 
        [SugarColumn(ColumnName = "chinese_pinyin")]
        public string ChinesePinyin { get => _ChinesePinyin; set => _ChinesePinyin = value?.Trim(); }

        private decimal _PackageNumber;
        /// <summary>
        /// 整包数量
        /// </summary> 
        [SugarColumn(ColumnName = "package_number")]
        public decimal PackageNumber { get => _PackageNumber; set => _PackageNumber = value; }

        private decimal _BoxNumber;
        /// <summary>
        /// 整箱数量
        /// </summary> 
        [SugarColumn(ColumnName = "box_number")]
        public decimal BoxNumber { get => _BoxNumber; set => _BoxNumber = value; }

        private double _WeightCapacity;
        /// <summary>
        /// 重量容量
        /// </summary> 
        [SugarColumn(ColumnName = "weight_capacity")]
        public double WeightCapacity { get => _WeightCapacity; set => _WeightCapacity = value; }

        private bool _IsOnlyPerUnit;
        /// <summary>
        /// 是否只按单位
        /// </summary> 
        [SugarColumn(ColumnName = "is_only_per_unit")]
        public bool IsOnlyPerUnit { get => _IsOnlyPerUnit; set => _IsOnlyPerUnit = value; }

        private int _GoodsClassId;
        /// <summary>
        /// 商品类别编号
        /// </summary> 
        [SugarColumn(ColumnName = "goods_class_id")]
        public int GoodsClassId { get => _GoodsClassId; set => _GoodsClassId = value; }

        /// <summary>
        /// 商品类别名称
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string GoodsClass { get; set; }

        private string _UnitName;
        /// <summary>
        /// 单位名称
        /// </summary> 
        [SugarColumn(ColumnName = "unit_name")]
        public string UnitName { get => _UnitName; set => _UnitName = value?.Trim(); }

        private System.DateTime? _GoodsAddDate;
        /// <summary>
        /// 商品添加日期
        /// </summary> 
        [SugarColumn(ColumnName = "goods_add_date")]
        public System.DateTime? GoodsAddDate { get => _GoodsAddDate; set => _GoodsAddDate = value ?? default(System.DateTime); }

        private decimal _Discount;
        /// <summary>
        /// 折扣
        /// </summary> 
        [SugarColumn(ColumnName = "discount")]
        public decimal Discount { get => _Discount; set => _Discount = value; }

        private decimal _DiscountPrice;
        /// <summary>
        /// 折扣价
        /// </summary> 
        [SugarColumn(ColumnName = "discount_price")]
        public decimal DiscountPrice { get => _DiscountPrice; set => _DiscountPrice = value; }

        private decimal _ReceiptPercentage;
        /// <summary>
        /// 发票百分比
        /// </summary> 
        [SugarColumn(ColumnName = "receipt_percentage")]
        public decimal ReceiptPercentage { get => _ReceiptPercentage; set => _ReceiptPercentage = value; }

        private System.DateTime? _ExpiryDate;
        /// <summary>
        /// 到期日期
        /// </summary> 
        [SugarColumn(ColumnName = "expiry_date")]
        public System.DateTime? ExpiryDate { get => _ExpiryDate; set => _ExpiryDate = value ?? default(System.DateTime); }

        private string _SupplierId;
        /// <summary>
        /// 供应商编号
        /// </summary> 
        [SugarColumn(ColumnName = "supplier_id")]
        public string SupplierId { get => _SupplierId; set => _SupplierId = value?.Trim(); }

        private int _IsGift;
        /// <summary>
        /// 是否有赠品
        /// </summary> 
        [SugarColumn(ColumnName = "is_gift")]
        public int IsGift { get => _IsGift; set => _IsGift = value; }

        private int _IsPrivate;
        /// <summary>
        /// 是否私营
        /// </summary> 
        [SugarColumn(ColumnName = "is_private")]
        public int IsPrivate { get => _IsPrivate; set => _IsPrivate = value; }

        private System.Boolean? _IsPriceByVolume;
        /// <summary>
        /// 是否以量定价
        /// </summary> 
        [SugarColumn(ColumnName = "is_price_by_volume")]
        public System.Boolean? IsPriceByVolume { get { return this._IsPriceByVolume; } set { this._IsPriceByVolume = value ?? default(System.Boolean); } }

        private System.Boolean? _IsDiscountByQuantity;
        /// <summary>
        /// 是否以量定折扣
        /// </summary> 
        [SugarColumn(ColumnName = "is_discount_by_quantity")]
        public System.Boolean? IsDiscountByQuantity { get { return this._IsDiscountByQuantity; } set { this._IsDiscountByQuantity = value ?? default(System.Boolean); } }
 
        private decimal _Price1;
        /// <summary>
        /// 价格1
        /// </summary> 
        [SugarColumn(ColumnName = "price1")]
        public decimal Price1 { get => _Price1; set => _Price1 = value; }

        private decimal _Discount1;
        /// <summary>
        /// 折扣1
        /// </summary> 
        [SugarColumn(ColumnName = "discount1")]
        public decimal Discount1 { get => _Discount1; set => _Discount1 = value; }

        private decimal _Number1;
        /// <summary>
        /// 数量1
        /// </summary> 
        [SugarColumn(ColumnName = "number1")]
        public decimal Number1 { get => _Number1; set => _Number1 = value; }

        private decimal _Price2;
        /// <summary>
        /// 价格2
        /// </summary> 
        [SugarColumn(ColumnName = "price2")]
        public decimal Price2 { get => _Price2; set => _Price2 = value; }

        private decimal _Discount2;
        /// <summary>
        /// 折扣2
        /// </summary> 
        [SugarColumn(ColumnName = "discount2")]
        public decimal Discount2 { get => _Discount2; set => _Discount2 = value; }

        private decimal _Number2;
        /// <summary>
        /// 数量2
        /// </summary> 
        [SugarColumn(ColumnName = "number2")]
        public decimal Number2 { get => _Number2; set => _Number2 = value; }

        private decimal _Price3;
        /// <summary>
        /// 价格3
        /// </summary> 
        [SugarColumn(ColumnName = "price3")]
        public decimal Price3 { get => _Price3; set => _Price3 = value; }

        private decimal _Discount3;
        /// <summary>
        /// 折扣3
        /// </summary> 
        [SugarColumn(ColumnName = "discount3")]
        public decimal Discount3 { get => _Discount3; set => _Discount3 = value; }

        private decimal _Number3;
        /// <summary>
        /// 数量3
        /// </summary> 
        [SugarColumn(ColumnName = "number3")]
        public decimal Number3 { get => _Number3; set => _Number3 = value; }

        private decimal _Price4;
        /// <summary>
        /// 价格4
        /// </summary> 
        [SugarColumn(ColumnName = "price4")]
        public decimal Price4 { get => _Price4; set => _Price4 = value; }

        private decimal _Discount4;
        /// <summary>
        /// 折扣4
        /// </summary> 
        [SugarColumn(ColumnName = "discount4")]
        public decimal Discount4 { get => _Discount4; set => _Discount4 = value; }

        private decimal _Number4;
        /// <summary>
        /// 数量4
        /// </summary> 
        [SugarColumn(ColumnName = "number4")]
        public decimal Number4 { get => _Number4; set => _Number4 = value; }

        private decimal _Price5;
        /// <summary>
        /// 价格5
        /// </summary> 
        [SugarColumn(ColumnName = "price5")]
        public decimal Price5 { get => _Price5; set => _Price5 = value; }

        private decimal _Discount5;
        /// <summary>
        /// 折扣5
        /// </summary> 
        [SugarColumn(ColumnName = "discount5")]
        public decimal Discount5 { get => _Discount5; set => _Discount5 = value; }

        private decimal _Number5;
        /// <summary>
        /// 数量5
        /// </summary> 
        [SugarColumn(ColumnName = "number5")]
        public decimal Number5 { get => _Number5; set => _Number5 = value; }

        private decimal _Price6;
        /// <summary>
        /// 价格6
        /// </summary> 
        [SugarColumn(ColumnName = "price6")]
        public decimal Price6 { get => _Price6; set => _Price6 = value; }

        private decimal _Discount6;
        /// <summary>
        /// 折扣6
        /// </summary> 
        [SugarColumn(ColumnName = "discount6")]
        public decimal Discount6 { get => _Discount6; set => _Discount6 = value; }

        private string _WarehouseSite;
        /// <summary>
        /// 仓库位置
        /// </summary> 
        [SugarColumn(ColumnName = "warehouse_site")]
        public string WarehouseSite { get => _WarehouseSite; set => _WarehouseSite = value?.Trim(); }

        private string _GoodsGrade;
        /// <summary>
        /// 商品等级
        /// </summary> 
        [SugarColumn(ColumnName = "goods_grade")]
        public string GoodsGrade { get => _GoodsGrade; set => _GoodsGrade = value?.Trim(); }

        private decimal _MinStock;
        /// <summary>
        /// 库存下限
        /// </summary> 
        [SugarColumn(ColumnName = "min_stock")]
        public decimal MinStock { get => _MinStock; set => _MinStock = value; }

        private decimal _MaxStock;
        /// <summary>
        /// 库存上限
        /// </summary> 
        [SugarColumn(ColumnName = "max_stock")]
        public decimal MaxStock { get => _MaxStock; set => _MaxStock = value; }

        private bool _IsContainImage;
        /// <summary>
        /// 有图案
        /// </summary> 
        [SugarColumn(ColumnName = "is_contain_image")]
        public bool IsContainImage { get => _IsContainImage; set => _IsContainImage = value; }

        private string _GoodsImageMd5;
        /// <summary>
        /// 图案的消息摘要
        /// </summary> 
        [SugarColumn(ColumnName = "goods_image_md5")]
        public string GoodsImageMd5 { get => _GoodsImageMd5; set => _GoodsImageMd5 = value?.Trim(); }

        private System.DateTime? _ImageUpdateTime;
        /// <summary>
        /// 修改绘图时间
        /// </summary> 
        [SugarColumn(ColumnName = "image_update_time")]
        public System.DateTime? ImageUpdateTime { get => _ImageUpdateTime; set => _ImageUpdateTime = value ?? default(System.DateTime); }
        //private Goods.EGoodsImage _GoodsImage = new Goods.EGoodsImage();
        ///// <summary>
        ///// 图片
        ///// </summary> 
        //[SugarColumn(IsIgnore = true)]
        //public Goods.EGoodsImage GoodsImage { get=>_GoodsImage; set => _GoodsImage = value; } 

        private bool _IsProhibitedChangeDiscount;
        /// <summary>
        /// 是否禁止更改折扣
        /// </summary> 
        [SugarColumn(ColumnName = "is_prohibited_change_discount")]
        public bool IsProhibitedChangeDiscount { get => _IsProhibitedChangeDiscount; set => _IsProhibitedChangeDiscount = value; }

        private bool _PrintSign;
        /// <summary>
        /// 打印标记
        /// </summary> 
        [SugarColumn(ColumnName = "print_sign")]
        public bool PrintSign { get => _PrintSign; set => _PrintSign = value; }

        private System.DateTime? _PrintSignTime;
        /// <summary>
        /// 打印标记时间
        /// </summary> 
        [SugarColumn(ColumnName = "print_sign_time")]
        public System.DateTime? PrintSignTime { get => _PrintSignTime; set => _PrintSignTime = value ?? default(System.DateTime); }

        private bool _IsAddByAttachment;
        /// <summary>
        /// 是否添加为附件
        /// </summary> 
        [SugarColumn(ColumnName = "is_add_by_attachment")]
        public bool IsAddByAttachment { get => _IsAddByAttachment; set => _IsAddByAttachment = value; }

        private string _MultipleBarcode;
        /// <summary>
        /// 多条码
        /// </summary> 
        [SugarColumn(ColumnName = "multiple_barcode")]
        public string MultipleBarcode { get => _MultipleBarcode; set => _MultipleBarcode = value?.Trim(); }

        private string _PartialConsultation;
        /// <summary>
        /// 部分查询
        /// </summary> 
        [SugarColumn(ColumnName = "partial_consultation")]
        public string PartialConsultation { get => _PartialConsultation; set => _PartialConsultation = value?.Trim(); }

        private System.DateTime? _StartDate;
        /// <summary>
        /// 起始日期
        /// </summary> 
        [SugarColumn(ColumnName = "start_date")]
        public System.DateTime? StartDate { get => _StartDate; set => _StartDate = value ?? default(System.DateTime); }

        private int _IsLock;
        /// <summary>
        /// 是否锁定:1 锁定(下架） 0 未锁定（在售）
        /// </summary> 
        [SugarColumn(ColumnName = "is_lock")]
        public int IsLock { get => _IsLock; set => _IsLock = value; }

        private int _SpecialSign;
        /// <summary>
        /// 特殊标识
        /// </summary> 
        [SugarColumn(ColumnName = "special_sign")]
        public int SpecialSign { get => _SpecialSign; set => _SpecialSign = value; }

        private string _Remark;
        /// <summary>
        /// 备注
        /// </summary> 
        [SugarColumn(ColumnName = "remark")]
        public string Remark { get => _Remark; set => _Remark = value?.Trim(); }

        private string _DelFlag = "0";
        /// <summary>
        /// 删除标记：1 删除 0 非删除
        /// </summary> 
        [SugarColumn(ColumnName = "del_flag")]
        public string DelFlag { get => _DelFlag; set => _DelFlag = value?.Trim(); }

        private int? _BrandId;
        /// <summary>
        /// 商品品牌id
        /// </summary> 
        [SugarColumn(ColumnName = "brand_id")]
        public int? BrandId { get => _BrandId; set => _BrandId = value ?? default(int); }

        /// <summary>
        /// 商品品牌名称
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string Brand { get; set; }

        private string _Origin;
        /// <summary>
        /// 产地
        /// </summary> 
        [SugarColumn(ColumnName = "origin")]
        public string Origin { get => _Origin; set => _Origin = value?.Trim(); }

        private string _ValuationMethod;
        /// <summary>
        /// 计价方式：1 包装 2 称重
        /// </summary> 
        [SugarColumn(ColumnName = "valuation_method")]
        public string ValuationMethod { get => _ValuationMethod; set => _ValuationMethod = value?.Trim(); }

        private string _SyncOnlineShop;
        /// <summary>
        /// 是否同步网店:1 同步 0 不同步
        /// </summary> 
        [SugarColumn(ColumnName = "sync_online_shop")]
        public string SyncOnlineShop { get => _SyncOnlineShop; set => _SyncOnlineShop = value?.Trim(); }

        private string _PackageSale;
        /// <summary>
        /// 是否整包销售:1 是 0 否
        /// </summary> 
        [SugarColumn(ColumnName = "package_sale")]
        public string PackageSale { get => _PackageSale; set => _PackageSale = value?.Trim(); }

        private string _IsEnablePoints;
        /// <summary>
        /// 是否启用积分：1 启用 0 不启用
        /// </summary> 
        [SugarColumn(ColumnName = "is_enable_points")]
        public string IsEnablePoints { get => _IsEnablePoints; set => _IsEnablePoints = value?.Trim(); }

        private string _PointsRule;
        /// <summary>
        /// 积分规则
        /// </summary> 
        [SugarColumn(ColumnName = "points_rule")]
        public string PointsRule { get => _PointsRule; set => _PointsRule = value?.Trim(); }

        private string _PurchaseSpec;
        /// <summary>
        /// 进货规格
        /// </summary> 
        [SugarColumn(ColumnName = "purchase_spec")]
        public string PurchaseSpec { get => _PurchaseSpec; set => _PurchaseSpec = value?.Trim(); }

        private System.Boolean? _IsIntervalPrice;
        /// <summary>
        /// 是否启用区间价格:1 启用 0 不启用
        /// </summary> 
        [SugarColumn(ColumnName = "is_interval_price")]
        public System.Boolean? IsIntervalPrice { get { return this._IsIntervalPrice; } set { this._IsIntervalPrice = value ?? default(System.Boolean); } }

        private System.Boolean? _IsCountEarning;
        /// <summary>
        /// 是否计算利润（毛利润）：1 启用 0 不启用
        /// </summary> 
        [SugarColumn(ColumnName = "is_count_earning")]
        public System.Boolean? IsCountEarning { get { return this._IsCountEarning; } set { this._IsCountEarning = value ?? default(System.Boolean); } }

        private int _TenantId;
        /// <summary>
        /// 所属租户
        /// </summary> 
        [SugarColumn(ColumnName = "tenant_id")]
        public int TenantId { get => _TenantId; set => _TenantId = value; }

        [SugarColumn(IsIgnore = true)]
        public decimal StockNum { get; set; }
    }
}
