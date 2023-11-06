using SqlSugar;
namespace WWB.Park.Entity.Tenant
{
    [SugarTable("pk_agent_user")]
    public class AgentUser :   IAgentFilter
    {
        /// <summary>
        /// 代理ID
        /// </summary>
        [SugarColumn(ColumnName = "agent_id")]
        public virtual long AgentId { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        [SugarColumn(ColumnName = "user_id")]
        public long UserId { get; set; }

        /// <summary>
        /// 是否超级管理员
        /// </summary>
        [SugarColumn(ColumnName = "is_admin")]
        public bool IsAdmin { get; set; }
    }
}