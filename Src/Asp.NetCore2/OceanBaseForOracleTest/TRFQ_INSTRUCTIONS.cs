using SqlSugar;
using System;
using System.Collections.Generic;
using xTPLM.Base.Extend;
using xTPLM.RFQ.Common;
using xTPLM.RFQ.Common.Enum;

namespace xTPLM.RFQ.Model.XRFQ_APP
{
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
        [SugarColumn(IsNullable = true, ColumnDataType = "INT")]
        public InstructionsStatus? STATUS { get; set; }

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
        [SugarColumn(IsNullable = true, ColumnDataType = "INT")]
        public PriceTypeEnum? PRICE_TYPE { get; set; }

        /// <summary>
        /// Desc:交易方向
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, ColumnDataType = "VARCHAR2(50 BYTE)")]
        public TradeType? TRADE_TYPE { get; set; }

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
        public RatesEnum? IS_RATES { get; set; }

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
        /// 指令类型 1:精确指令 2:模糊指令
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public InstructionsType I_TYPE { get; set; }

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
        [SugarColumn(IsNullable = true, ColumnDataType = "INT")]
        public SetDays SET_DAYS { get; set; } = SetDays.T0;

        /// <summary>
        /// Desc:指令流水号 请勿使用
        /// Default:
        /// Nullable:True
        /// </summary> 
        [Obsolete]
        [SugarColumn(IsNullable = true)]
        public string I_NO { get; set; }

        /// <summary>
        /// 到期收益率
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public decimal? YTM { get; set; }

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
        public decimal? YTM_UPPER
        {
            get
            {
                if (this.I_TYPE != InstructionsType.Vague)
                {
                    return this.YTM;
                }
                else
                {
                    return this._ytm_upper;
                }
            }
            set
            {
                this._ytm_upper = value;
            }
        }

        /// <summary>
        /// 行政收益率上限 （模式指令有效）
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public decimal? YTM_OE_UPPER
        {
            get
            {
                if (this.I_TYPE != InstructionsType.Vague)
                {
                    return this.YTM_OE;
                }
                else
                {
                    return this._ytm_oe_upper;
                }
            }
            set
            {
                this._ytm_oe_upper = value;
            }
        }

        /// <summary>
        /// 净价上限（模糊指令有效）
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public decimal? NETPRICE_UPPER
        {
            get
            {
                if (this.I_TYPE != InstructionsType.Vague)
                {
                    return this.NETPRICE;
                }
                else
                {
                    return this._netprice_upper;
                }
            }
            set
            {
                this._netprice_upper = value;
            }
        }

        /// <summary>
        /// 全价上限 （模糊指令有效）
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public decimal? PRICE_UPPER
        {
            get
            {
                if (this.I_TYPE != InstructionsType.Vague)
                {
                    return this.PRICE;
                }
                else
                {
                    return this._price_upper;
                }
            }
            set
            {
                this._price_upper = value;
            }
        }

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

        /// <summary>
        /// 作废前状态
        /// </summary>           
        [SugarColumn(IsNullable = true)]
        public InstructionsStatus? CANCEL_STATUS { get; set; }

        #region 数据库忽略字段
        /// <summary>
        /// 已下发的面额
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public decimal OrderMoneyRelease { get; set; }

        /// <summary>
        /// 剩余可用额度
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public decimal REMAIN_LIMIT { get; set; }

        /// <summary>
        /// 进度
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public decimal PROGRESS
        {
            get
            {
                if (this.ORDER_MONEY > 0)
                {
                    return Math.Round(this.OrderMoneyRelease / this.ORDER_MONEY.Value, 2) * 100;
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// 部门ID
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public int? D_ID { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string D_NAME { get; set; }

        /// <summary>
        /// 交易对手名称
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string PARTY_NAME { get; set; }

        /// <summary>
        /// 执行人（执行人有多个）
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public List<int> UserId { get; set; }

        /// <summary>
        /// 执行人姓名
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string User_Desc { get; set; }

        /// <summary>
        /// 录入人名称
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string CREATE_BY_NAME { get; set; }

        /// <summary>
        /// 是否能够编辑或者提交审批
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public bool IsEdit { get; set; }

        /// <summary>
        /// 价格类型描述
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string PRICE_TYPE_DESC
        {
            get
            {
                return this.PRICE_TYPE.GetValueOrDefault().GetDescription();
            }
        }

        /// <summary>
        /// 指令类型类型描述
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string I_TYPE_DESC => this.I_TYPE.GetDescription();

        /// <summary>
        /// 状态描述
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string STATUS_DESC
        {
            get
            {
                if (this.STATUS.HasValue)
                {
                    return this.STATUS.Value.GetDescription();
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// 交易方向描述
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string TRADE_TYPE_DESC
        {
            get
            {
                if (this.TRADE_TYPE.HasValue)
                {
                    return this.TRADE_TYPE.Value.GetDescription();
                }
                return string.Empty;
            }

        }

        /// <summary>
        /// 清算速度描述
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string SET_DAYS_DESC
        {
            get
            {
                return this.SET_DAYS.GetDescription();
            }
        }

        /// <summary>
        /// 价格范围
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string PRICE_MIN
        {
            get
            {
                string result = string.Empty;
                if (this.PRICE_TYPE.HasValue)
                {
                    if (this.I_TYPE == InstructionsType.Vague)
                    {
                        switch (this.PRICE_TYPE.Value)
                        {
                            case PriceTypeEnum.YTM:
                                result = $"{this.YTM}-{this.YTM_UPPER}";
                                break;
                            case PriceTypeEnum.YTM_OE:
                                result = $"{this.YTM_OE}-{this.YTM_OE_UPPER}";
                                break;
                            case PriceTypeEnum.NETPRICE:
                                result = $"{this.NETPRICE}-{this.NETPRICE_UPPER}";
                                break;
                            case PriceTypeEnum.PRICE:
                                result = $"{this.PRICE}-{this.PRICE_UPPER}";
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        switch (this.PRICE_TYPE.Value)
                        {
                            case PriceTypeEnum.YTM:
                                result = this.YTM.ToString();
                                break;
                            case PriceTypeEnum.YTM_OE:
                                result = this.YTM_OE.ToString();
                                break;
                            case PriceTypeEnum.NETPRICE:
                                result = this.NETPRICE.ToString();
                                break;
                            case PriceTypeEnum.PRICE:
                                result = this.PRICE.ToString();
                                break;
                            default:
                                break;
                        }
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// 内证账户名称
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string SECU_ACCNAME { get; set; }

        /// <summary>
        ///发行人
        /// </summary>       
        [SugarColumn(IsIgnore = true)]
        public string ISSUER { get; set; }

        /// <summary>
        /// 债券名称
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string B_NAME { get; set; }

        /// <summary>
        /// 交易市场描述
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string M_TYPE_DESC
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.M_TYPE))
                {
                    return MarketType.GetDescription(this.M_TYPE);
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// 关联有效询价列表（询价单状态已撤销或无记录或审批拒绝，视为无效）
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public List<int> ValidQuotaIDList { get; set; }

        #endregion

        #region 资管字段
        /// <summary>
        /// 产品ID（内资代码）
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 128)]
        public string CASH_ACCID { get; set; }

        /// <summary>
        /// 待确认操作类型
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 50)]
        public ActionType? CONFIRM_ACTION_TYPE { get; set; }


        #region 数据库忽略字段

        /// <summary>
        /// 产品名称
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string PRODUCT_NAME { get; set; }

        #endregion

        #endregion

        /// <summary>
        /// 价格处理
        /// </summary>
        public void PriceMath()
        {
            if (this.NETPRICE.HasValue)
            {
                this.NETPRICE = Math.Round(this.NETPRICE.Value, 4, MidpointRounding.AwayFromZero);
            }
            if (this.NETPRICE_UPPER.HasValue)
            {
                this.NETPRICE_UPPER = Math.Round(this.NETPRICE_UPPER.Value, 4, MidpointRounding.AwayFromZero);
            }
            if (this.YTM.HasValue)
            {
                this.YTM = Math.Round(this.YTM.Value, 4, MidpointRounding.AwayFromZero);
            }
            if (this.YTM_UPPER.HasValue)
            {
                this.YTM_UPPER = Math.Round(this.YTM_UPPER.Value, 4, MidpointRounding.AwayFromZero);
            }
            if (this.YTM_OE.HasValue)
            {
                this.YTM_OE = Math.Round(this.YTM_OE.Value, 4, MidpointRounding.AwayFromZero);
            }
            if (this.YTM_OE_UPPER.HasValue)
            {
                this.YTM_OE_UPPER = Math.Round(this.YTM_OE_UPPER.Value, 4, MidpointRounding.AwayFromZero);
            }
            if (this.PRICE.HasValue)
            {
                this.PRICE = Math.Round(this.PRICE.Value, 4, MidpointRounding.AwayFromZero);
            }
            if (this.PRICE_UPPER.HasValue)
            {
                this.PRICE_UPPER = Math.Round(this.PRICE_UPPER.Value, 4, MidpointRounding.AwayFromZero);
            }
        }

    }


    /// <summary>
    /// 指令排序对象
    /// </summary>
    public class InsOrderList
    {

        /// <summary>
        /// 债券代码
        /// </summary>
        public string I_CODE { get; set; }

        /// <summary>
        /// 资产类型
        /// </summary>
        public string A_TYPE { get; set; }

        /// <summary>
        /// 交易市场
        /// </summary>
        public string M_TYPE { get; set; }

        /// <summary>
        /// 债券名称
        /// </summary>
        public string B_NAME { get; set; }

        /// <summary>
        /// 交易市场描述
        /// </summary>
        public string M_TYPE_DESC
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.M_TYPE))
                {
                    return MarketType.GetDescription(this.M_TYPE);
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// 最小ID
        /// </summary>
        public int MinId { get; set; }

        /// <summary>
        /// 子列表
        /// </summary>
        public List<TRFQ_INSTRUCTIONS> ChildList { get; set; }
    }

}
