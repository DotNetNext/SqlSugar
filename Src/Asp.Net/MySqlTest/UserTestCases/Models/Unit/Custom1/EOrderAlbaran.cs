using SqlSugar;

namespace HONORCSData.Order
{
    /// <summary>
    /// 出库单/入库单
    /// </summary>
    ///
    [SugarTable("order_albaran")]
    public class EOrderAlbaran
    {
        /// <summary>
        /// 出库单/入库单
        /// </summary>
        public EOrderAlbaran()
        {
        }

        private System.Int32 _AlbaranId;
        /// <summary>
        /// 主键id
        /// </summary>
        [SugarColumn(IsPrimaryKey =true, ColumnName = "albaran_id")]
        public System.Int32 AlbaranId { get { return this._AlbaranId; } set { this._AlbaranId = value; } }

        private System.Int32? _ServerAccountId;
        /// <summary>
        /// 服务器端账户id
        /// </summary> 
        [SugarColumn(ColumnName = "server_account_id")]
        public System.Int32? ServerAccountId { get { return this._ServerAccountId; } set { this._ServerAccountId = value; } }

        private System.String _ClientNo;
        /// <summary>
        /// 客户/供应商ID
        /// </summary> 
        [SugarColumn(ColumnName = "client_no")]
        public System.String ClientNo { get { return this._ClientNo; } set { this._ClientNo = value; } }

        private System.Boolean? _IsClientWithoutIva;
        /// <summary>
        /// 客户是否不含iva:1 含 0 不含
        /// </summary> 
        [SugarColumn(ColumnName = "is_client_without_iva")]
        public System.Boolean? IsClientWithoutIva { get { return this._IsClientWithoutIva; } set { this._IsClientWithoutIva = value; } }

        private System.Boolean? _IsClientWithoutIvaAndIvaInclude;
        /// <summary>
        /// ClienteSinIVA的IVAIncluido是否勾选:1 是 0 否
        /// </summary> 
        [SugarColumn(ColumnName = "is_client_without_iva_and_iva_include")]
        public System.Boolean? IsClientWithoutIvaAndIvaInclude { get { return this._IsClientWithoutIvaAndIvaInclude; } set { this._IsClientWithoutIvaAndIvaInclude = value; } }

        private System.Boolean? _IsClientReq;
        /// <summary>
        /// 客户是否有REQ:1 是 0 否
        /// </summary> 
        [SugarColumn(ColumnName = "is_client_req")]
        public System.Boolean? IsClientReq { get { return this._IsClientReq; } set { this._IsClientReq = value; } }

        private System.Boolean? _IsClientReqAndReqInclude;
        /// <summary>
        /// ClienteREQ的req_include是否勾选:1 是 0 否
        /// </summary> 
        [SugarColumn(ColumnName = "is_client_req_and_req_include")]
        public System.Boolean? IsClientReqAndReqInclude { get { return this._IsClientReqAndReqInclude; } set { this._IsClientReqAndReqInclude = value; } }

        private System.Decimal? _Discount;
        /// <summary>
        /// 折扣
        /// </summary> 
        [SugarColumn(ColumnName = "discount")]
        public System.Decimal? Discount { get { return this._Discount; } set { this._Discount = value; } }

        private System.Decimal? _DirectDiscount;
        /// <summary>
        /// 直接折扣
        /// </summary> 
        [SugarColumn(ColumnName = "direct_discount")]
        public System.Decimal? DirectDiscount { get { return this._DirectDiscount; } set { this._DirectDiscount = value; } }

        private System.Int16? _IvaRule;
        /// <summary>
        /// iva规则(id)
        /// </summary> 
        [SugarColumn(ColumnName = "iva_rule")]
        public System.Int16? IvaRule { get { return this._IvaRule; } set { this._IvaRule = value; } }

        private System.Int32? _IvaId;
        /// <summary>
        /// iva_id
        /// </summary> 
        [SugarColumn(ColumnName = "iva_id")]
        public System.Int32? IvaId { get { return this._IvaId; } set { this._IvaId = value; } }

        private System.Int16? _UsePrice;
        /// <summary>
        /// 使用价格：1 零售价 2 会员价 3 进货价
        /// </summary> 
        [SugarColumn(ColumnName = "use_price")]
        public System.Int16? UsePrice { get { return this._UsePrice; } set { this._UsePrice = value; } }

        private System.Decimal? _Total;
        /// <summary>
        /// 共计
        /// </summary> 
        [SugarColumn(ColumnName = "total")]
        public System.Decimal? Total { get { return this._Total; } set { this._Total = value; } }

        private System.Decimal? _TotalDiscount;
        /// <summary>
        /// 折扣总计
        /// </summary> 
        [SugarColumn(ColumnName = "total_discount")]
        public System.Decimal? TotalDiscount { get { return this._TotalDiscount; } set { this._TotalDiscount = value; } }

        private System.Decimal? _TotalVale;
        /// <summary>
        /// vale代金券总计
        /// </summary> 
        [SugarColumn(ColumnName = "total_vale")]
        public System.Decimal? TotalVale { get { return this._TotalVale; } set { this._TotalVale = value; } }

        private System.Decimal? _Profit;
        /// <summary>
        /// 利润
        /// </summary> 
        [SugarColumn(ColumnName = "profit")]
        public System.Decimal? Profit { get { return this._Profit; } set { this._Profit = value; } }

        private System.Decimal? _TotalNumber;
        /// <summary>
        /// 总数量
        /// </summary> 
        [SugarColumn(ColumnName = "total_number")]
        public System.Decimal? TotalNumber { get { return this._TotalNumber; } set { this._TotalNumber = value; } }

        private System.Decimal? _TotalCash;
        /// <summary>
        /// 总现金
        /// </summary> 
        [SugarColumn(ColumnName = "total_cash")]
        public System.Decimal? TotalCash { get { return this._TotalCash; } set { this._TotalCash = value; } }

        private System.DateTime? _AlbaranDate;
        /// <summary>
        /// albaran日期
        /// </summary> 
        [SugarColumn(ColumnName = "albaran_date")]
        public System.DateTime? AlbaranDate { get { return this._AlbaranDate; } set { this._AlbaranDate = value; } }

        private System.DateTime? _EntryTime;
        /// <summary>
        /// 录入时间
        /// </summary> 
        [SugarColumn(ColumnName = "entry_time")]
        public System.DateTime? EntryTime { get { return this._EntryTime; } set { this._EntryTime = value; } }

        private System.String _OriginDocType;
        /// <summary>
        /// 原始文件类型
        /// </summary> 
        [SugarColumn(ColumnName = "origin_doc_type")]
        public System.String OriginDocType { get { return this._OriginDocType; } set { this._OriginDocType = value; } }

        private System.Int32? _OriginDocId;
        /// <summary>
        /// 原始文件编号
        /// </summary> 
        [SugarColumn(ColumnName = "origin_doc_id")]
        public System.Int32? OriginDocId { get { return this._OriginDocId; } set { this._OriginDocId = value; } }

        private System.String _DestinationDocType;
        /// <summary>
        /// 目的文件类型
        /// </summary> 
        [SugarColumn(ColumnName = "destination_doc_type")]
        public System.String DestinationDocType { get { return this._DestinationDocType; } set { this._DestinationDocType = value; } }

        private System.Int32? _DestinationDocId;
        /// <summary>
        /// 目的文件编号
        /// </summary> 
        [SugarColumn(ColumnName = "destination_doc_id")]
        public System.Int32? DestinationDocId { get { return this._DestinationDocId; } set { this._DestinationDocId = value; } }

        private System.Int16? _ChargeType;
        /// <summary>
        /// 收费类型
        /// </summary> 
        [SugarColumn(ColumnName = "charge_type")]
        public System.Int16? ChargeType { get { return this._ChargeType; } set { this._ChargeType = value; } }

        private System.Decimal? _Cash;
        /// <summary>
        /// 收费类型
        /// </summary> 
        [SugarColumn(ColumnName = "cash")]
        public System.Decimal? Cash { get { return this._Cash; } set { this._Cash = value; } }

        private System.Int32? _AgentId;
        /// <summary>
        /// 代理商id
        /// </summary> 
        [SugarColumn(ColumnName = "agent_id")]
        public System.Int32? AgentId { get { return this._AgentId; } set { this._AgentId = value; } }

        private System.Int32? _TransporterId;
        /// <summary>
        /// 运输商ID
        /// </summary> 
        [SugarColumn(ColumnName = "transporter_id")]
        public System.Int32? TransporterId { get { return this._TransporterId; } set { this._TransporterId = value; } }

        private System.String _CarNo;
        /// <summary>
        /// 车牌号
        /// </summary> 
        [SugarColumn(ColumnName = "car_no")]
        public System.String CarNo { get { return this._CarNo; } set { this._CarNo = value; } }

        private System.Int32? _Operator;
        /// <summary>
        /// 操作员编号
        /// </summary> 
        [SugarColumn(ColumnName = "operator")]
        public System.Int32? Operator { get { return this._Operator; } set { this._Operator = value; } }

        private System.Int32? _Verifier;
        /// <summary>
        /// 审核员编号
        /// </summary> 
        [SugarColumn(ColumnName = "verifier")]
        public System.Int32? Verifier { get { return this._Verifier; } set { this._Verifier = value; } }

        private System.Int32? _StorehouseId;
        /// <summary>
        /// 仓库id
        /// </summary> 
        [SugarColumn(ColumnName = "storehouse_id")]
        public System.Int32? StorehouseId { get { return this._StorehouseId; } set { this._StorehouseId = value; } }

        private System.String _AttachedDocSymbol;
        /// <summary>
        /// 附加文件符号
        /// </summary> 
        [SugarColumn(ColumnName = "attached_doc_symbol")]
        public System.String AttachedDocSymbol { get { return this._AttachedDocSymbol; } set { this._AttachedDocSymbol = value; } }

        private System.Int32? _PrePaymentMethod;
        /// <summary>
        /// 预付款方式
        /// </summary> 
        [SugarColumn(ColumnName = "pre_payment_method")]
        public System.Int32? PrePaymentMethod { get { return this._PrePaymentMethod; } set { this._PrePaymentMethod = value; } }

        private System.String _ComputerName;
        /// <summary>
        /// 计算机名称
        /// </summary> 
        [SugarColumn(ColumnName = "computer_name")]
        public System.String ComputerName { get { return this._ComputerName; } set { this._ComputerName = value; } }

        private System.String _Hash;
        /// <summary>
        /// 哈希
        /// </summary> 
        [SugarColumn(ColumnName = "hash")]
        public System.String Hash { get { return this._Hash; } set { this._Hash = value; } }

        private System.Int32? _CifId;
        /// <summary>
        /// cif_id(到岸编号)
        /// </summary> 
        [SugarColumn(ColumnName = "cif_id")]
        public System.Int32? CifId { get { return this._CifId; } set { this._CifId = value; } }

        private System.String _SubStoreId;
        /// <summary>
        /// 子商店编号
        /// </summary> 
        [SugarColumn(ColumnName = "sub_store_id")]
        public System.String SubStoreId { get { return this._SubStoreId; } set { this._SubStoreId = value; } }

        private System.Decimal? _AbonoTotal;
        /// <summary>
        /// abono总计
        /// </summary> 
        [SugarColumn(ColumnName = "abono_total")]
        public System.Decimal? AbonoTotal { get { return this._AbonoTotal; } set { this._AbonoTotal = value; } }

        private System.Decimal? _PaymentTotal;
        /// <summary>
        /// 付款总计
        /// </summary> 
        [SugarColumn(ColumnName = "payment_total")]
        public System.Decimal? PaymentTotal { get { return this._PaymentTotal; } set { this._PaymentTotal = value; } }

        private System.Boolean? _IsCharged;
        /// <summary>
        /// 是否已收款:1 是 0 否
        /// </summary> 
        [SugarColumn(ColumnName = "is_charged")]
        public System.Boolean? IsCharged { get { return this._IsCharged; } set { this._IsCharged = value; } }

        private System.Boolean? _IsLock;
        /// <summary>
        /// 是否锁定:1 是 0 否
        /// </summary> 
        [SugarColumn(ColumnName = "is_lock")]
        public System.Boolean? IsLock { get { return this._IsLock; } set { this._IsLock = value; } }

        private System.Boolean? _IsCanceled;
        /// <summary>
        /// 是否取消：1 是 0 否 
        /// </summary> 
        [SugarColumn(ColumnName = "is_canceled")]
        public System.Boolean? IsCanceled { get { return this._IsCanceled; } set { this._IsCanceled = value; } }

        private System.String _IsModify;
        /// <summary>
        /// 是否修改:1 是 0 否
        /// </summary> 
        [SugarColumn(ColumnName = "is_modify")]
        public System.String IsModify { get { return this._IsModify; } set { this._IsModify = value; } }

        private System.Boolean? _IsSendEmail;
        /// <summary>
        /// 是否发送电子邮件：1 是 0 否
        /// </summary> 
        [SugarColumn(ColumnName = "is_send_email")]
        public System.Boolean? IsSendEmail { get { return this._IsSendEmail; } set { this._IsSendEmail = value; } }

        private System.String _Remark;
        /// <summary>
        /// 备注
        /// </summary> 
        [SugarColumn(ColumnName = "remark")]
        public System.String Remark { get { return this._Remark; } set { this._Remark = value; } }

        private System.String _AlbaranNo;
        /// <summary>
        /// 订单编号
        /// </summary> 
        [SugarColumn(ColumnName = "albaran_no")]
        public System.String AlbaranNo { get { return this._AlbaranNo; } set { this._AlbaranNo = value; } }

        private System.Int16? _AlbaranType;
        /// <summary>
        /// 订单类型：1 入库 2 出库
        /// </summary> 
        [SugarColumn(ColumnName = "albaran_type")]
        public System.Int16? AlbaranType { get { return this._AlbaranType; } set { this._AlbaranType = value; } }

        private System.Int32? _CreateTime;
        /// <summary>
        /// 创建时间
        /// </summary> 
        [SugarColumn(ColumnName = "create_time")]
        public System.Int32? CreateTime { get { return this._CreateTime; } set { this._CreateTime = value; } }

        private System.Int32? _UpdateTime;
        /// <summary>
        /// 修改时间
        /// </summary> 
        [SugarColumn(ColumnName = "update_time")]
        public System.Int32? UpdateTime { get { return this._UpdateTime; } set { this._UpdateTime = value; } }

        private System.String _VerifyStatus;
        /// <summary>
        /// albaran（入库/出库单）审核状态：1 审核 0 未审核
        /// </summary> 
        [SugarColumn(ColumnName = "verify_status")]
        public System.String VerifyStatus { get { return this._VerifyStatus; } set { this._VerifyStatus = value; } }

        private System.Int32? _VerifyTime;
        /// <summary>
        /// 审核日期
        /// </summary> 
        [SugarColumn(ColumnName = "verify_time")]
        public System.Int32? VerifyTime { get { return this._VerifyTime; } set { this._VerifyTime = value; } }

        private System.Int32? _PurchaseReceiveId;
        /// <summary>
        /// 采购收货订单Id
        /// </summary> 
        [SugarColumn(ColumnName = "purchase_receive_id")]
        public System.Int32? PurchaseReceiveId { get { return this._PurchaseReceiveId; } set { this._PurchaseReceiveId = value; } }

        private System.String _DelFlag;
        /// <summary>
        ///  删除标记：1 删除 0 未删除
        /// </summary> 
        [SugarColumn(ColumnName = "del_flag")]
        public System.String DelFlag { get { return this._DelFlag; } set { this._DelFlag = value; } }

        private System.Int32 _TenantId;
        /// <summary>
        /// 所属租户
        /// </summary> 
        [SugarColumn(ColumnName = "tenant_id")]
        public System.Int32 TenantId { get { return this._TenantId; } set { this._TenantId = value; } }
    }
}
