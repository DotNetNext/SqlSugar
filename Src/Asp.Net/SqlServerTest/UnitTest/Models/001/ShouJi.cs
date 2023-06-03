using SqlSugar;

namespace Demo
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("shouji")]
    public class ShouJi
    {
        public ShouJi()
        {
        }
        /// <summary>
        /// Desc:收集单流水号
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(ColumnDescription = "收集单流水号", Length = 16)]
        public string shouji_sysno { get; set; }
        /// <summary>
        /// Desc:收集单据号，多个逗号分隔
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(ColumnDescription = "收集单据号，多个逗号分隔", Length = 200)]
        public string shouji_sn { get; set; }
        /// <summary>
        /// Desc:收集单ID
        /// Default:newid()
        /// Nullable:False
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, ColumnDescription = "收集单GUID")]
        public string shouji_guid { get; set; }
        /// <summary>
        /// Desc:收集单ID
        /// Default:newid()
        /// Nullable:False
        /// </summary>
        [SugarColumn(ColumnDescription = "收集单GUID")]
        public string xuqin_name { get; set; }
    }
}