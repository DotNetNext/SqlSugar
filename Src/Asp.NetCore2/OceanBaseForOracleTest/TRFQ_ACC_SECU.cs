using SqlSugar;
using System;
using xTPLM.RFQ.Common.Enum;

namespace xTPLM.RFQ.Model.XRFQ_APP
{
    ///<summary>
    ///询报价—内证表
    ///</summary>
    [SugarTable("TRFQ_ACC_SECU")]
    public partial class TRFQ_ACC_SECU
    {
        public TRFQ_ACC_SECU()
        {


        }
        /// <summary>
        /// Desc:内证编号
        /// Default:
        /// Nullable:False
        /// </summary>           
        [SugarColumn(IsNullable = false, IsPrimaryKey = true, Length = 32)]
        public string ACCID { get; set; }

        /// <summary>
        /// Desc:内证名称
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 512)]
        public string ACCNAME { get; set; }

        /// <summary>
        /// Desc:内资
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 32)]
        public string CASH_ACCID { get; set; }

        /// <summary>
        /// Desc:所有者
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 32)]
        public string OWNER { get; set; }

        /// <summary>
        /// Desc:证券账户状态	0:创建中	1:已启用	2:停用中	3:已停用
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true)]
        public AccountStatus? STATUS { get; set; }

        /// <summary>
        /// Desc:导入时间
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true)]
        public DateTime? IMPORTTIME { get; set; }

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
        /// 
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 100)]
        public string SHTRADER_ID { get; set; }

        /// <summary>
        /// 财务分类科目
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 50)]
        public string ACCFISCASUBJECT { get; set; }

    }
}
