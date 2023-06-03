using SqlSugar;

namespace Demo
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("shouji_subnum")]
    public class ShoujiSubnum
    {
        public ShoujiSubnum()
        {
        }
        /// <summary>
        /// Desc:分段ID
        /// Default:
        /// Nullable:True
        /// </summary>
        public int? insurance_section_id { get; set; }
        /// <summary>
        /// Desc:收集单ID
        /// Default:
        /// Nullable:True
        /// </summary>
        public string shouji_guid { get; set; }
        /// <summary>
        /// Desc:分段数量
        /// Default:
        /// Nullable:True
        /// </summary>
        public decimal? amount { get; set; }
        /// <summary>
        /// Desc:预估理赔金额
        /// Default:
        /// Nullable:True
        /// </summary>
        public decimal? total_money { get; set; }
        /// <summary>
        /// Desc:收集分段ID
        /// Default:newid()
        /// Nullable:False
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public string sub_shouji_guid { get; set; }
    }
}