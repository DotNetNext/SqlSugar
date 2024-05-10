using SqlSugar;

namespace xTPLM.RFQ.Model.XRFQ_APP
{
    ///<summary>
    ///指令表
    ///</summary>
    [SugarTable("TRFQ_INSTRUCTIONS")]
    public partial class TRFQ_INSTRUCTIONS
    {
        private decimal? _ytm_upper;
        private decimal? _ytm_oe_upper;
        private decimal? _netprice_upper;
        private decimal? _price_upper;

        public TRFQ_INSTRUCTIONS()
        {


        }
        /// <summary>
        /// Desc:指令编号
        /// Default:
        /// Nullable:False
        /// </summary>           
        [SugarColumn(IsNullable = false, IsPrimaryKey = true, OracleSequenceName = "TRFQ_INSTRUCTIONS$SEQ", IsIdentity = true)]
        public int I_ID { get; set; }

        /// <summary>
        /// Desc:指令名称
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 128)]
        public string I_NAME { get; set; }

        /// <summary>
        /// Desc:截止时间
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true)]
        public DateTime? END_TIME { get; set; }

        /// <summary>
        /// Desc:备注
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 1024)]
        public string REMARK { get; set; }

        /// <summary>
        /// Desc:创建人ID
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true)]
        public int? CREATE_BY { get; set; }

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
        [SugarColumn(IsNullable = true)]
        public int? UPDATE_BY { get; set; }
        /// <summary>
        /// Desc:状态
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true)]
        public int? STATUS { get; set; }

        /// <summary>
        /// Desc:债券代码
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 100)]
        public string I_CODE { get; set; }

        /// <summary>
        /// Desc:资产类型
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 100)]
        public string A_TYPE { get; set; }

        /// <summary>
        /// Desc:交易市场
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 100)]
        public string M_TYPE { get; set; }

        /// <summary>
        /// Desc:面额（万元）
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true)]
        public decimal? ORDER_MONEY { get; set; }

        /// <summary>
        /// Desc:价格类型
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true)]
        public int? PRICE_TYPE { get; set; }

        /// <summary>
        /// Desc:交易方向
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true)]
        public string TRADE_TYPE { get; set; }

        /// <summary>
        /// Desc:交易对手
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true)]
        public int? PARTY_ID { get; set; }

        /// <summary>
        /// Desc:是否城投 0:否,1:是
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true)]
        public int? IS_CITY_INVESTMENT { get; set; }

        /// <summary>
        /// Desc:是否永续债，0：否；1：是
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true)]
        public int? PERPETUAL { get; set; }

        /// <summary>
        /// Desc:是否利率债 0:未划分，1：信用债，2：利率债
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true)]
        public int? IS_RATES { get; set; }

        /// <summary>
        /// Desc:最小交易日期
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true)]
        public DateTime? ORDER_DATE_MIN { get; set; }

        /// <summary>
        /// Desc:最大交易日期
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true)]
        public DateTime? ORDER_DATE_MAX { get; set; }

        /// <summary>
        /// Desc:交易市场。（非债券交易市场，该字段而是作为筛选，限制指令债券的交易市场）
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 100)]
        public string MARKET { get; set; }

        /// <summary>
        /// Desc:外部ID（提交给外部系统审批，返回主键）
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true)]
        public int? SYSID_EXT { get; set; }

        /// <summary>
        /// Desc:内证账户
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 64)]
        public string SECU_ACCID { get; set; }

        /// <summary>
        /// 提交审批IR用户
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 40)]
        public string SUBMIT_IRUSER { get; set; }

        /// <summary>
        /// 久期
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public decimal? MODIFIED_D { get; set; }

        /// <summary>
        /// 产品类型
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 100)]
        public string P_CLASS { get; set; }

        /// <summary>
        /// 剩余期限
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public decimal? LAST_TERM { get; set; }

        /// <summary>
        /// 期限类型
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 100)]
        public string LAST_TERM_TYPE { get; set; }

        /// <summary>
        /// 提示
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 4000)]
        public string SUBMIT_MESSAGE { get; set; }

        /// <summary>
        /// 到期收益率
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public decimal? YTM { get; set; }

        /// <summary>
        /// 指令类型 1:精确指令 2:模糊指令
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public int I_TYPE { get; set; } = 1;

        /// <summary>
        /// 交易日期
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public DateTime? ORDER_DATE { get; set; }

        /// <summary>
        /// Desc:清算速度
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true)]
        public int SET_DAYS { get; set; }

        /// <summary>
        /// Desc:指令流水号 请勿使用
        /// Default:
        /// Nullable:True
        /// </summary> 
        [Obsolete]
        [SugarColumn(IsNullable = true)]
        public string I_NO { get; set; }

        /// <summary>
        /// 行权收益率
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public decimal? YTM_OE { get; set; }

        /// <summary>
        /// 净价
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public decimal? NETPRICE { get; set; }

        /// <summary>
        /// 全价
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public decimal? PRICE { get; set; }

        /// <summary>
        /// 到期收益率上限 （模糊指令有效）
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public decimal? YTM_UPPER { get; set; }

        /// <summary>
        /// 行政收益率上限 （模式指令有效）
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public decimal? YTM_OE_UPPER { get; set; }

        /// <summary>
        /// 净价上限（模糊指令有效）
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public decimal? NETPRICE_UPPER { get; set; }

        /// <summary>
        /// 全价上限 （模糊指令有效）
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public decimal? PRICE_UPPER { get; set; }


        /// <summary>
        /// 提交审批时间
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public DateTime? SUBMIT_TIME { get; set; }

        /// <summary>
        /// 上交所固收平台约定号
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 20)]
        public string SHG_AGREENUM { get; set; }

        /// <summary>
        /// 上交所固收平台对手方交易员代码
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 50)]
        public string SHG_TRADER_CP { get; set; }

        /// <summary>
        /// 上交所固收平台对手方席位号
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 50)]
        public string SHG_SEATNO_CP { get; set; }

        /// <summary>
        /// 数据来源
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 256)]
        public string SOURCE_TYPE { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public DateTime? CREATE_TIME { get; set; }

        /// <summary>
        /// 外部交易标识
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 128)]
        public string EXT_TRADE_ID { get; set; }
    }

}
