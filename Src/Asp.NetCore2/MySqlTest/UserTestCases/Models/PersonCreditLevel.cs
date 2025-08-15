using SqlSugar;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestDemo.Entitys
{
    [SugarTable("credit_confirm_level")]
    public class PersonCreditLevel
    {
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true)]
        public int Id { get; set; }

        /// <summary>
        /// 身份证号码 
        ///</summary>
        [SugarColumn(ColumnName = "id_card")]
        public string IdCard { get; set; }

        /// <summary>
        /// 信用等级
        ///</summary>
        [SugarColumn(ColumnName = "level")]
        public int Level { get; set; }
    }
}
