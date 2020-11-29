using SqlSugar;

namespace HONORCSData
{
    /// <summary>
    /// 商品品牌
    /// </summary>
    [SugarTable("goods_brand")]
    public class EGoodsBrand
    {
        /// <summary>
        /// 
        /// </summary>
        public EGoodsBrand()
        {
        }

        private System.Int32 _BrandId;
        /// <summary>
        /// 自增编号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnName = "brand_id")]
        public System.Int32 BrandId { get { return this._BrandId; } set { this._BrandId = value; } }

        private System.String _BrandCnName;
        /// <summary>
        /// 中文名称
        /// </summary> 
        [SugarColumn(ColumnName = "brand_cn_name")]
        public System.String BrandCnName { get { return this._BrandCnName; } set { this._BrandCnName = value?.Trim(); } }

        private System.String _BranchSpanishName;
        /// <summary>
        /// 西文名称
        /// </summary> 
        [SugarColumn(ColumnName = "branch_spanish_name")]
        public System.String BranchSpanishName { get { return this._BranchSpanishName; } set { this._BranchSpanishName = value?.Trim(); } }


        private System.String _BrandNo;
        /// <summary>
        /// 编号
        /// </summary> 
        [SugarColumn(ColumnName = "brand_no")]
        public System.String BrandNo { get { return this._BrandNo; } set { this._BrandNo = value?.Trim(); } }

        private System.String _BrandStatus = "1";
        /// <summary>
        /// 状态：1 正常 0 禁用
        /// </summary> 
        [SugarColumn(ColumnName = "brand_status")]
        public System.String BrandStatus { get { return this._BrandStatus; } set { this._BrandStatus = value?.Trim(); } }

        private System.String _DelFlag = "0";
        /// <summary>
        /// 是否删除：1 删除 0 未删除
        /// </summary> 
        [SugarColumn(ColumnName = "del_flag")]
        public System.String DelFlag { get { return this._DelFlag; } set { this._DelFlag = value?.Trim(); } }

        private int _TenantId;
        /// <summary>
        /// 所属租户
        /// </summary> 
        [SugarColumn(ColumnName = "tenant_id")]
        public int TenantId { get => _TenantId; set => _TenantId = value; }
    }
}
