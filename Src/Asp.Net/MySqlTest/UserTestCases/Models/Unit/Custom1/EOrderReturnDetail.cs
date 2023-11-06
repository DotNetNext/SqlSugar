using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HONORCSData.Order
{
    /// <summary>
    /// 退货详情
    /// </summary>
    [SugarTable("order_return_detail")]
    public class EOrderReturnDetail
    {
        /// <summary>
        /// 退货详情
        /// </summary>
        public EOrderReturnDetail()
        {
        }

        private System.Int32 _OrderReturnId;
        /// <summary>
        /// 退货单编号
        /// </summary> 
        [SugarColumn(ColumnName = "order_return_id")]
        public System.Int32 OrderReturnId { get { return this._OrderReturnId; } set { this._OrderReturnId = value; } }

        private System.String _GoodsNo;
        /// <summary>
        /// 商品编号
        /// </summary> 
        [SugarColumn(ColumnName = "goods_no")]
        public System.String GoodsNo { get { return this._GoodsNo; } set { this._GoodsNo = value; } }

        private System.String _BarCode;
        /// <summary>
        /// 条码
        /// </summary> 
        [SugarColumn(ColumnName = "bar_code")]
        public System.String BarCode { get { return this._BarCode; } set { this._BarCode = value; } }

        private System.Boolean _IsBlanceBarCode;
        /// <summary>
        /// 是否对称条码：1 是 0 否
        /// </summary> 
        [SugarColumn(ColumnName = "is_blance_bar_code")]
        public System.Boolean IsBlanceBarCode { get { return this._IsBlanceBarCode; } set { this._IsBlanceBarCode = value; } }

        private System.String _SpanishName;
        /// <summary>
        /// 西文名称
        /// </summary> 
        [SugarColumn(ColumnName = "spanish_name")]
        public System.String SpanishName { get { return this._SpanishName; } set { this._SpanishName = value; } }

        private System.String _ChineseName;
        /// <summary>
        /// 中文名称
        /// </summary> 
        [SugarColumn(ColumnName = "chinese_name")]
        public System.String ChineseName { get { return this._ChineseName; } set { this._ChineseName = value; } }

        private System.Decimal? _Price;
        /// <summary>
        /// 价格
        /// </summary> 
        [SugarColumn(ColumnName = "price")]
        public System.Decimal? Price { get { return this._Price; } set { this._Price = value; } }

        private System.Decimal? _Number;
        /// <summary>
        /// 数量
        /// </summary> 
        [SugarColumn(ColumnName = "number")]
        public System.Decimal? Number { get { return this._Number; } set { this._Number = value; } }

        private System.Decimal? _Iva;
        /// <summary>
        /// iva
        /// </summary> 
        [SugarColumn(ColumnName = "iva")]
        public System.Decimal? Iva { get { return this._Iva; } set { this._Iva = value; } }

        private System.Decimal? _Req;
        /// <summary>
        /// req
        /// </summary> 
        [SugarColumn(ColumnName = "req")]
        public System.Decimal? Req { get { return this._Req; } set { this._Req = value; } }

        private System.Int32? _IvaId;
        /// <summary>
        /// iva_id
        /// </summary> 
        [SugarColumn(ColumnName = "iva_id")]
        public System.Int32? IvaId { get { return this._IvaId; } set { this._IvaId = value; } }

        private System.Decimal? _Discount;
        /// <summary>
        /// 折扣
        /// </summary> 
        [SugarColumn(ColumnName = "discount")]
        public System.Decimal? Discount { get { return this._Discount; } set { this._Discount = value; } }

        private System.Boolean? _IsProhibitedChangeDiscount;
        /// <summary>
        /// 是否禁止更改折扣：1 是 0 否
        /// </summary> 
        [SugarColumn(ColumnName = "is_prohibited_change_discount")]
        public System.Boolean? IsProhibitedChangeDiscount { get { return this._IsProhibitedChangeDiscount; } set { this._IsProhibitedChangeDiscount = value; } }

        private System.Decimal? _CostPrice;
        /// <summary>
        /// 成本价
        /// </summary> 
        [SugarColumn(ColumnName = "cost_price")]
        public System.Decimal? CostPrice { get { return this._CostPrice; } set { this._CostPrice = value; } }

        private System.String _Commentary;
        /// <summary>
        /// 批注
        /// </summary> 
        [SugarColumn(ColumnName = "commentary")]
        public System.String Commentary { get { return this._Commentary; } set { this._Commentary = value; } }

        private System.Boolean? _IsTemporary;
        /// <summary>
        /// 是否是临时的：1 是 0 否
        /// </summary> 
        [SugarColumn(ColumnName = "is_temporary")]
        public System.Boolean? IsTemporary { get { return this._IsTemporary; } set { this._IsTemporary = value; } }

        private System.Int32? _OrderId;
        /// <summary>
        /// 订单id
        /// </summary> 
        [SugarColumn(ColumnName = "order_id")]
        public System.Int32? OrderId { get { return this._OrderId; } set { this._OrderId = value; } }

        private System.Decimal? _PackageAmount;
        /// <summary>
        /// 包装数量(包数)
        /// </summary> 
        [SugarColumn(ColumnName = "package_amount")]
        public System.Decimal? PackageAmount { get { return this._PackageAmount; } set { this._PackageAmount = value; } }

        private System.Decimal? _Total;
        /// <summary>
        /// 合计
        /// </summary> 
        [SugarColumn(ColumnName = "total")]
        public System.Decimal? Total { get { return this._Total; } set { this._Total = value; } }

        private System.String _Remark;
        /// <summary>
        /// 备注
        /// </summary> 
        [SugarColumn(ColumnName = "remark")]
        public System.String Remark { get { return this._Remark; } set { this._Remark = value; } }

        private System.String _DelFlag;
        /// <summary>
        /// 删除标记：1 删除 0 未删除
        /// </summary> 
        [SugarColumn(ColumnName = "del_flag")]
        public System.String DelFlag { get { return this._DelFlag; } set { this._DelFlag = value; } }

        private System.String _UnitName;
        /// <summary>
        /// 单位名称
        /// </summary> 
        [SugarColumn(ColumnName = "unit_name")]
        public System.String UnitName { get { return this._UnitName; } set { this._UnitName = value; } }

        private System.String _PurchaseSpec;
        /// <summary>
        /// 进货规格
        /// </summary> 
        [SugarColumn(ColumnName = "purchase_spec")]
        public System.String PurchaseSpec { get { return this._PurchaseSpec; } set { this._PurchaseSpec = value; } }

        private System.Int32 _Id;
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, ColumnName = "id")]
        public System.Int32 Id { get { return this._Id; } set { this._Id = value; } }

        private System.Int32 _TenantId;
        /// <summary>
        /// 所属租户id
        /// </summary> 
        [SugarColumn(ColumnName = "tenant_id")]
        public System.Int32 TenantId { get { return this._TenantId; } set { this._TenantId = value; } }


        [SugarColumn(IsIgnore = true)]
        public int Index { get; set; }
    }
}
