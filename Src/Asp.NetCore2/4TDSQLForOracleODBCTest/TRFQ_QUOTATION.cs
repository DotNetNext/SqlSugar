using Newtonsoft.Json;
using SqlSugar;

namespace xTPLM.RFQ.Model.XRFQ_APP
{
    ///<summary>
    ///询价记录表
    ///</summary>
    [SugarTable("TRFQ_QUOTATION")]
    public partial class TRFQ_QUOTATION
    {
        /// <summary>
        /// 询价序列名称
        /// </summary>
        public const string SequenceName = "TRFQ_QUOTATION$SEQ";
        public TRFQ_QUOTATION()
        {


        }
        /// <summary>
        /// Desc:主键序号自增序列
        /// Default:
        /// Nullable:False
        /// </summary>           
        [SugarColumn(IsNullable = false, IsPrimaryKey = true, OracleSequenceName = SequenceName, IsIdentity = true)]
        public int Q_SYSID { get; set; }

        /// <summary>
        /// Desc:QQ号（由于对接QTrade，所以当CONTACTNUMBER_TYPE=2时，此字段存储QT号）
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 50)]
        public string QQ_NUMBER { get; set; }

        /// <summary>
        /// Desc:本方交易员
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 100)]
        public string SELF_TRADER { get; set; }

        /// <summary>
        /// Desc:执行市场
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 50)]
        public string EXE_MARKET { get; set; }

        /// <summary>
        /// Desc:金融工具Code
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 100)]
        public string I_CODE { get; set; }

        /// <summary>
        /// Desc:金融工具A_TYPE
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 100)]
        public string A_TYPE { get; set; }

        /// <summary>
        /// Desc:金融工具M_TYPE
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 100)]
        public string M_TYPE { get; set; }

        /// <summary>
        /// Desc:面额（万）
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true)]
        public decimal? ORDER_MONEY { get; set; }

        /// <summary>
        /// 实际面额
        /// 基于剩余本金计算
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public decimal? ACTUAL_AMOUNT { get; set; }

        /// <summary>
        /// Desc:到期收益率
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true)]
        public decimal? BND_YTM { get; set; }

        /// <summary>
        /// Desc:行权收益率
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true)]
        public decimal? BND_YTM_OE { get; set; }

        /// <summary>
        /// Desc:净价
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true)]
        public decimal? BND_NETPRICE { get; set; }

        /// <summary>
        /// Desc:全价
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true)]
        public decimal? BND_PRICE { get; set; }

        /// <summary>
        /// Desc:交易方向
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 50)]
        public int? TRADER_TYPE { get; set; }

        /// <summary>
        /// Desc:交易日期
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true)]
        public DateTime? ORDER_DATE { get; set; }

        /// <summary>
        /// Desc:结算日期
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true)]
        public DateTime? SETDATE { get; set; }

        /// <summary>
        /// Desc:清算速度
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, ColumnDataType = "INT")]
        public int? SET_DAYS { get; set; }

        /// <summary>
        /// Desc:交易对手
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true)]
        public int? PARTY_ID { get; set; }

        /// <summary>
        /// Desc:实际交易对手
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true)]
        public int? PARTYID_ACTUAL { get; set; }

        /// <summary>
        /// Desc:对方交易员
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true)]
        public int? TRADER_CP { get; set; }

        /// <summary>
        /// Desc:报价发起方
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 1)]
        public int? QUOTE_SPONSOR { get; set; }

        /// <summary>
        /// Desc:预期报价方式
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, ColumnName = "DEAL_TYPE", ColumnDataType = "INT")]
        public int? DEAL_TYPE { get; set; }

        /// <summary>
        /// Desc:中介机构
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 50)]
        public string INTERMEDIATION { get; set; }

        /// <summary>
        /// Desc:上交所固收平台约定号
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 20)]
        public string SHG_AGREENUM { get; set; }

        /// <summary>
        /// Desc:上交所固收平台对手方交易员代码
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 50)]
        public string SHG_TRADER_CP { get; set; }

        /// <summary>
        /// Desc:上交所固收平台对手方席位号
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 50)]
        public string SHG_SEATNO_CP { get; set; }

        /// <summary>
        /// Desc:导入时间
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true)]
        public DateTime? IMPORT_TIME { get; set; }

        /// <summary>
        /// Desc:交易备注
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 1000)]
        public string REMARK { get; set; }

        /// <summary>
        /// Desc:交易是否删除：0未删除 1删除
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true)]
        public int IS_DELETE { get; set; } = 0;

        /// <summary>
        /// Desc:询价状态
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(ColumnName = "Q_STATUS", ColumnDataType = "INT")]
        public int? Q_STATUS { get; set; }


        /// <summary>
        /// Desc:反向报价(和本表的主键关联)
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true)]
        public int? SYSID_INVERSE { get; set; }

        /// <summary>
        /// Desc:交易系统交易
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true)]
        public int? SYSID_EXT { get; set; }

        /// <summary>
        /// Desc:公开级别，0:个人，1：小组，2：公共
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true)]
        public int? Q_PUBLICLEVEL { get; set; }

        /// <summary>
        /// Desc:报价来源,
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, ColumnDataType = "INT")]
        public int? Q_BID_SOURCE { get; set; }

        /// <summary>
        /// Desc:外部交易标识
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 128)]
        public string EXT_TRADE_ID { get; set; }

        /// <summary>
        /// Desc:最后一次修改时间
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true)]
        public DateTime? UPDATE_TIME { get; set; }

        /// <summary>
        /// Desc:最后一次修改人
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 32)]
        public string UPDATE_BY { get; set; }

        /// <summary>
        /// Desc:外部联系账号类型:1-QQ号，2-QT号
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true)]
        public int? CONTACTNUMBER_TYPE { get; set; }

        /// <summary>
        /// Desc:价格类型： 1-到期收益率 2-行权收益率 3-净价 4-全价
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, ColumnDataType = "INT")]
        public int? PRICE_TYPE { get; set; }

        /// <summary>
        /// Desc:下达状态 0:未下达，5：下达中，10:下达成功，-1：下达失败
        /// Default:
        /// Nullable:False
        /// </summary>           
        [SugarColumn(IsNullable = false, ColumnName = "RELEASE_STATUS")]
        public int RELEASE_STATUS { get; set; }

        /// <summary>
        /// Desc:下达信息
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 1024)]
        public string RELEASE_MESSAGE { get; set; }

        /// <summary>
        /// Desc:下达时间
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true)]
        public DateTime? RELEASE_TIME { get; set; }

        /// <summary>
        /// Desc:内政账户
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 64)]
        public string SECU_ACCID { get; set; }

        /// <summary>
        /// Desc:本方操作员
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 64)]
        public int? SELF_OPERATOR { get; set; }

        /// <summary>
        /// Desc:交易执行员
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 64)]
        public string EXECUTOR { get; set; }

        /// <summary>
        /// Desc:xIR交易组合ID
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 64)]
        public string GRPID { get; set; }

        /// <summary>
        /// Desc:xIR交易组合名称
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 512)]
        public string GRPNAME { get; set; }

        /// <summary>
        /// Desc:xIR交易子组合ID
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 64)]
        public string CHILDGROUPID { get; set; }

        /// <summary>
        /// Desc:xIR交易子组合名称
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 512)]
        public string CHILDGROUPNAME { get; set; }

        /// <summary>
        /// Desc:自定义组合类型
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 64)]
        public string GRPTYPE_CUSTOM { get; set; }

        /// <summary>
        /// Desc:自定义子组合类型
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 64)]
        public string CHILDGRPTYPE_CUSTOM { get; set; }

        /// <summary>
        /// Desc:临时交易对手
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 128)]
        public string PARTYNAMETEMPORITY { get; set; }

        /// <summary>
        /// Desc:创建时间
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true)]
        public DateTime? CREATE_TIME { get; set; }

        /// <summary>
        /// Desc:中介费用
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true)]
        public decimal? INTERMEDIARY_FEE { get; set; }

        /// <summary>
        /// Desc:报价执行员
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 128)]
        public string EXECUTETRADER { get; set; }

        /// <summary>
        /// XIR交易系统状态
        /// </summary>           
        [SugarColumn(IsNullable = true)]
        public int? TRADESYSTEM_STATUS { get; set; }

        /// <summary>
        /// 对手方交易商代码
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 256)]
        public string DEALER_CODE { get; set; }

        /// <summary>
        /// 对手方交易主体代码
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 256)]
        public string TRADEBODY_CODE { get; set; }

        /// <summary>
        /// 拆分前询价ID（用于记录该询价是否是拆分询价，并记录拆分前的询价ID）
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int? ORI_QID { get; set; }

        /// <summary>
        /// 首期结算状态 #该字段目前没有落地，当作数据库忽略字段使用
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int? FST_STATE { get; set; }

        /// <summary>
        /// 到期结算状态 #该字段目前没有落地，当作数据库忽略字段使用
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int? END_STATE { get; set; }

        /// <summary>
        /// 应计利息
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public decimal? AIAMOUNT { get; set; }

        /// <summary>
        /// 结算金额
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public decimal? ORDAMOUNT { get; set; }

        /// <summary>
        /// 外部成交编号
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string EXTORDID { get; set; }

        /// <summary>
        /// 原始文本
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 2000)]
        public string ORIGNAL_TEXT { get; set; }

        /// <summary>
        /// 实际交易对手信息
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 200)]
        public string ACTUALPARTY_LINKMAN { get; set; }

        /// <summary>
        /// 本方交易商
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 512)]
        public string SELF_DEALER { get; set; }

        /// <summary>
        /// 本方交易主体
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 512)]
        public string SELF_INVESTOR { get; set; }

        /// <summary>
        /// 对手方主体类型
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 512)]
        public string CP_INVESTOR_TYPE { get; set; }

        /// <summary>
        /// 投资经理
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 1024)]
        public int? INVESTMENT_MANAGER { get; set; }

        /// <summary>
        /// 询价备注
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 4000)]
        public string QUOTA_REMARK { get; set; }

        /// <summary>
        /// 是否内部询价(交易) 1:是
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int? IS_INTERNAL { get; set; }

        /// <summary>
        /// 对手方内部证券账户
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 256)]
        public string SECU_INT_CP { get; set; }

        /// <summary>
        /// 内部对手交易员
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 512)]
        public string SECU_TRADER_CP { get; set; }

        /// <summary>
        /// 数据来源 系统简称（接口导入时，存储导入的系统标识）
        /// </summary>
        public string SOURCE_TYPE { get; set; }

        /// <summary>
        /// 预计收益率
        /// </summary>
        public decimal? EXPECTED_YIELD { get; set; }

        /// <summary>
        /// 单张应计利息
        /// </summary>
        public decimal? UNIT_AIAMOUNT { get; set; }

        /// <summary>
        /// 报价途径
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 64)]
        public string QUOTECHANNEL { get; set; }

        /// <summary>
        /// 当前审批角色
        /// </summary>
        public string APPROVALUSER { get; set; }

        #region 资管字段
        /// <summary>
        /// 产品ID（内资代码）
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 128)]
        public string CASH_ACCID { get; set; }


        #region 数据库忽略字段

        /// <summary>
        /// 产品名称
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string PRODUCT_NAME { get; set; }

        #endregion

        #endregion

        /// <summary>
        /// 将询价价格设置为有效价格
        /// </summary>
        public void QuotaPriceAuth()
        {
            //收益率不能小于0，不能大于10000
            if (this.BND_YTM >= 10000 || this.BND_YTM < 0)
            {
                this.BND_YTM = 0;
            }

            //收益率不能小于0，不能大于10000
            if (this.BND_YTM_OE >= 10000 || this.BND_YTM_OE < 0)
            {
                this.BND_YTM_OE = 0;
            }

            //全价不能小于0
            if (this.BND_PRICE < 0)
            {
                this.BND_PRICE = 0;
            }

            //净价不能小于0
            if (this.BND_NETPRICE < 0)
            {
                this.BND_NETPRICE = 0;
            }
        }



    }
}
