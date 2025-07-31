using SqlSugar;

namespace WebApplication4
{
    /// <summary>
    /// 管理员信息
    /// </summary>
    [SugarTable("admin")]
    [SugarIndex("index_admin_account_id", nameof(AccountId), OrderByType.Asc )]
    public class Admin
    {
        /// <summary>
        /// Id
        /// </summary>
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        /// 关联账号
        /// </summary>
        [SugarColumn(ColumnName = "account_id")]
        public int AccountId { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [SugarColumn(ColumnName = "name", Length = 50)]
        public string Name { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        [SugarColumn(ColumnName = "mobile", IsNullable = true, Length = 20)]
        public string Mobile { get; set; }
        
    }
    
}