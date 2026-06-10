using System;
using System.Collections.Generic;
using SqlSugar;

namespace xTPLM.RFQ.Model.XRFQ_APP
{
    ///<summary>
    ///交易对手表
    ///</summary>
    [SugarTable("TRFQ_COUNTERPARTY")]
    public partial class TRFQ_COUNTERPARTY
    {
        public TRFQ_COUNTERPARTY()
        {


        }
        /// <summary>
        /// Desc:主键
        /// Default:
        /// Nullable:False
        /// </summary>           
        [SugarColumn(IsNullable = false, IsPrimaryKey = true, OracleSequenceName = "TRFQ_COUNTERPARTY$SEQ", IsIdentity = true)]
        public int P_SYSID { get; set; }

        /// <summary>
        /// Desc:名称
        /// Default:
        /// Nullable:False
        /// </summary>           
        [SugarColumn(IsNullable = false, Length = 1000)]
        public string PARTYNAME { get; set; }

        /// <summary>
        /// Desc:简称
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 200)]
        public string PARTYNAME_SHORT { get; set; }

        /// <summary>
        /// Desc:启用状态:0=停用;1=启用
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true)]
        public int? PARTYSTATUS { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public bool IsOpen
        {
            get
            {
                return this.PARTYSTATUS == 1;
            }
            set
            {
                this.PARTYSTATUS = value ? 1 : 0;
            }
        }

        /// <summary>
        /// Desc:银行间会员代码
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 50)]
        public string CNBD_MEMBERID { get; set; }

        /// <summary>
        /// Desc:外汇21位机构代码
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 50)]
        public string CNBD_ORGCODE { get; set; }

        /// <summary>
        /// Desc:所在地域
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 50)]
        public string BELONGTOAREA { get; set; }

        /// <summary>
        /// Desc:上交所固收平台交易商代码
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 32)]
        public string SHG_FIX_CUSTCODE { get; set; }

        /// <summary>
        /// Desc:新增类型：0:手动,1:自动
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 10)]
        public string PARTYSOURCE { get; set; }

        /// <summary>
        /// Desc:统一社会信用代码
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 100)]
        public string CREDITCODE { get; set; }

        /// <summary>
        /// Desc:法人代表
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 200)]
        public string LEGALPERSON { get; set; }

        /// <summary>
        /// Desc:营业执照有效日期
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 10)]
        public string EXPDATE { get; set; }

        /// <summary>
        /// Desc:备注
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 500)]
        public string REMARK { get; set; }

        /// <summary>
        /// Desc:外汇做市商类型:0 非做市商，1 做市商，2 尝试做市商-综合做市，3 尝试做市商-专项做市
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 10)]
        public string MARKETMAKER_TYPE { get; set; }

        /// <summary>
        /// Desc:最后更新人
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 32)]
        public string UPDATED_BY { get; set; }

        /// <summary>
        /// Desc:最后更新时间
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true)]
        public DateTime? UPDATED_TIME { get; set; }

        /// <summary>
        /// Desc:别名
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 1000)]
        public string ALIAS { get; set; }

        /// <summary>
        /// Desc:外部ID
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true)]
        public int? P_EXTID { get; set; }

        /// <summary>
        /// Desc:导入时间
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true)]
        public DateTime? IMPORTTIME { get; set; }

        /// <summary>
        /// Desc:所属机构
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true)]
        public int? PARENTORGANPARTYID { get; set; }

        /// <summary>
        /// Desc:客户信评等级
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 50)]
        public string CREDITLEVEL { get; set; }

        /// <summary>
        /// 净资产
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public decimal? PARTY_NET_ASSETS { get; set; }

        /// <summary>
        /// 交易对手名称_拼音首拼
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 512)]
        public string PARTYNAME_PINYIN { get; set; }

        /// <summary>
        /// 交易对手简称_拼音首拼
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 512)]
        public string PARTYNAME_SHORT_PINYIN { get; set; }

        /// <summary>
        /// 是否法人：1，法人；0，非法人
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 1)]
        public string ISLEGALPERSON { get; set; }

        /// <summary>
        /// 募集方式
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 128)]
        public string RAISEWAY { get; set; }

        #region 数据库忽略字段

        /// <summary>
        /// 对手类型描述
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string PARTY_TYPE_DESC
        {
            get
            {
                if (ISLEGALPERSON == "0")
                {
                    return "非法人户";
                }
                else if (ISLEGALPERSON == "1")
                {
                    return "法人户";
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// 所属机构名称
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string PARENTORGANPARTY { get; set; }

        /// <summary>
        /// 更新人名称
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string UPDATED_BY_NAME { get; set; }

        /// <summary>
        /// 是否满足入库标准
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public int? IS_CHECK_SUCCESS { get; set; }

        /// <summary>
        /// 是否符合入库标准(尽调表需求)
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public bool? IS_STANDARD
        {
            get
            {
                if (this.IS_CHECK_SUCCESS.HasValue)
                {
                    return this.IS_CHECK_SUCCESS == 1;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 尽调表到期日
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public DateTime? END_DATE { get; set; }

        /// <summary>
        /// 尽调表是否到期
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public bool IsEnd
        {
            get
            {
                if (END_DATE.HasValue)
                {
                    return END_DATE.Value.Date < DateTime.Now.Date;
                }
                return false;
            }
        }
        #endregion
    }
}
