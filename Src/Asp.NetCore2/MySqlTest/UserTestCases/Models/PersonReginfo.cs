using SqlSugar;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestDemo.Entitys
{
    [SugarTable("person_reginfo")]
    public class PersonReginfo
    {
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true)]
        public int Id { get; set; }

        /// <summary>
        /// 人员Id
        /// </summary>

        [SugarColumn(ColumnName = "person_id")]
        public int PersonId { get; set; }

        /// <summary>
        /// 注册证书编号 
        ///</summary>
        [SugarColumn(ColumnName = "cert_num")]
        public string CertNum { get; set; }
    }
}
