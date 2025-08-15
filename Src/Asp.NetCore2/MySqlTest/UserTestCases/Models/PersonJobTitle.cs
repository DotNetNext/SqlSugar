using SqlSugar;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestDemo.Entitys
{
    [SugarTable("person_job_title")]
    public class PersonJobTitle
    {
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true)]
        public int Id { get; set; }

        /// <summary>
        /// 人员Id
        /// </summary>

        [SugarColumn(ColumnName = "person_id")]
        public int PersonId { get; set; }

        /// <summary>
        ///  证书编号
        ///</summary>
        [SugarColumn(ColumnName = "cert_no")]
        public string CertNo { get; set; }
    }
}
