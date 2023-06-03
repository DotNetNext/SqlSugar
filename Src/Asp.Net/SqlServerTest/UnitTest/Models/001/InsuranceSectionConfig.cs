using SqlSugar;

namespace Demo
{
    ///<summary>
    ///畜禽分段信息
    ///</summary>
    [SugarTable("insurance_section_config")]
    public class InsuranceSectionConfig
    {
        public InsuranceSectionConfig()
        {
        }
        /// <summary>
        /// Desc:分段ID
        /// Default:
        /// Nullable:False
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnDescription = "分段ID", IsNullable = false)]
        public int insurance_section_id { get; set; }
        /// <summary>
        /// Desc:畜种细分编码
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(ColumnDescription = "畜种细分编码", Length = 10, IsNullable = true)]
        public string  category_code { get; set; }
    }
}