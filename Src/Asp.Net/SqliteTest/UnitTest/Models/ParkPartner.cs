using SqlSugar;
namespace WWB.Park.Entity.Tenant
{

    [SugarTable("pk_park_partner")]
    public class ParkPartner : AuditDeleteEntityBase<int>
    {
        /// <summary>
        /// 小区
        /// </summary>
        [SugarColumn(ColumnName = "park_id")]
        public long ParkId { get; set; }

        /// <summary>
        /// 分润方
        /// </summary>
        [SugarColumn(ColumnName = "agent_user_id")]
        public long AgentUserId { get; set; }

        /// <summary>
        /// 分润类型（1-比例）
        /// </summary>
        [SugarColumn(ColumnName = "type")]
        public int Type { get; set; }

        /// <summary>
        /// 停车分润比例
        /// </summary>
        [SugarColumn(ColumnName = "month_parking_rate")]
        public int MonthParkingRate { get; set; }

        /// <summary>
        /// 临停分润
        /// </summary>
        [SugarColumn(ColumnName = "temp_parking_rate")]
        public int TempParkingRate { get; set; }

        /// <summary>
        /// 充电分润比例
        /// </summary>
        [SugarColumn(ColumnName = "month_charge_rate")]
        public int MonthChargeRate { get; set; }

        /// <summary>
        /// 临充分润
        /// </summary>
        [SugarColumn(ColumnName = "temp_charge_rate")]
        public int TempChargeRate { get; set; }

        /// <summary>
        ///
        /// </summary>
        [SugarColumn(ColumnName = "is_admin")]
        public int IsAdmin { get; set; }
    }
}