using SqlSugar;

namespace TestDemo.Entitys
{
    public class PersonPageModelDto
    {
        public int Id { get; set; }

        /// <summary>
        /// 姓名 
        ///</summary>
        public string Name { get; set; }
        /// <summary>
        /// 身份证号码 
        ///</summary>
        [SugarColumn(ColumnName = "IdCard")]
        public string IdCard { get; set; }

        /// <summary>
        ///  
        ///</summary>
        public string CertNum { get; set; }
    }
}
