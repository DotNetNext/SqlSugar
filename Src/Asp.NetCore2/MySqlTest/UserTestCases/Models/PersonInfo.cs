using SqlSugar;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestDemo.Entitys
{
    [SugarTable("person_info")]
    public class PersonInfo
    {
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true)]
        public int Id { get; set; }

        /// <summary>
        /// 姓名 
        ///</summary>
        [SugarColumn(ColumnName = "name")]
        public string Name { get; set; }
        /// <summary>
        /// 身份证号码 
        ///</summary>
        [SugarColumn(ColumnName = "id_card")]
        public string IdCard { get; set; }
    }
}
