using SqlSugar;
using System;
using xTPLM.RFQ.Common;

namespace xTPLM.RFQ.Model.XRFQ_APP
{
    ///<summary>
    ///询价债券表
    ///</summary>
    [SugarTable("TRFQ_BND")]
    public partial class TRFQ_BND
    {
        public TRFQ_BND()
        {


        }
        /// <summary>
        /// Desc:I_CODE
        /// Default:
        /// Nullable:False
        /// </summary>           
        [SugarColumn(IsNullable = false, IsPrimaryKey = true, Length = 128)]
        public string I_CODE { get; set; }

        /// <summary>
        /// Desc:A_TYPE
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, IsPrimaryKey = true, Length = 128)]
        public string A_TYPE { get; set; }

        /// <summary>
        /// Desc:M_TYPE
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, IsPrimaryKey = true, Length = 128)]
        public string M_TYPE { get; set; }

        /// <summary>
        /// Desc:报价 价格类型:1-到期收益率  2-行权收益率 3-净价 4-全价
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, ColumnDataType = "INT")]
        public PriceTypeEnum? PRICE_TYPE { get; set; }

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
        [SugarColumn(IsNullable = true, Length = 128)]
        public string UPDATE_BY { get; set; }

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
        /// Desc:金融工具Name，债券简称
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 50)]
        public string B_NAME { get; set; }

        /// <summary>
        /// Desc:是否利率债 0:未划分，1：信用债，2：利率债
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true)]
        public decimal? IS_RATES { get; set; }

        /// <summary>
        /// Desc:发行人
        /// Default:
        /// Nullable:True
        /// </summary>       
        [SugarColumn(IsNullable = true, Length = 256)]
        public string ISSUER { get; set; }

        /// <summary>
        /// Desc:发行人
        /// Default:
        /// Nullable:True
        /// </summary>       
        [SugarColumn(IsNullable = true, Length = 256)]
        public string BONDISSUER { get; set; }

        /// <summary>
        /// 风控主体拼音全称
        /// </summary>       
        [SugarColumn(IsNullable = true, Length = 2048)]
        public string BONDISSUER_PINYIN { get; set; }

        /// <summary>
        /// 风控主体拼音简写
        /// </summary>       
        [SugarColumn(IsNullable = true, Length = 2048)]
        public string BONDISSUER_PINYIN_SHORT { get; set; }

        /// <summary>
        /// Desc:债券类别
        /// Default:
        /// Nullable:True
        /// </summary>      

        [SugarColumn(IsNullable = true, Length = 128)]
        public string P_CLASS { get; set; }

        /// <summary>
        /// Desc:万得一级分类
        /// Default:
        /// Nullable:True
        /// </summary>      

        [SugarColumn(IsNullable = true, Length = 128)]
        public string WIND_CLASS1 { get; set; }

    }
}
