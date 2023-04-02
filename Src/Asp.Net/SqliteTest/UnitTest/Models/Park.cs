 
using SqlSugar;
using System;

namespace WWB.Park.Entity.Tenant
{

    [SugarTable("pk_park")]
    public class Park : IAgentFilter
    {
        /// <summary>
        /// 代理id
        /// </summary>
        [SugarColumn(ColumnName = "agent_id")]
        public long AgentId { get; set; }

        /// <summary>
        /// 分组id
        /// </summary>
        [SugarColumn(ColumnName = "group_id", IsNullable = true)]
        public long? GroupId { get; set; }

        /// <summary>
        /// 小区名
        /// </summary>
        [SugarColumn(ColumnName = "name", Length = 64)]
        public string Name { get; set; }

        /// <summary>
        /// 省
        /// </summary>
        [SugarColumn(ColumnName = "province", Length = 32, IsNullable = true)]
        public string Province { get; set; }

        /// <summary>
        /// 市
        /// </summary>
        [SugarColumn(ColumnName = "city", Length = 32, IsNullable = true)]
        public string City { get; set; }

        /// <summary>
        /// 区
        /// </summary>
        [SugarColumn(ColumnName = "district", Length = 32, IsNullable = true)]
        public string District { get; set; }

        /// <summary>
        /// 详细地址
        /// </summary>
        [SugarColumn(ColumnName = "address", Length = 64, IsNullable = true)]
        public string Address { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        [SugarColumn(ColumnName = "longitude", ColumnDataType = "numeric(10,6)", IsNullable = true)]
        public decimal? Longitude { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        [SugarColumn(ColumnName = "latitude", ColumnDataType = "numeric(10,6)", IsNullable = true)]
        public decimal? Latitude { get; set; }

        /// <summary>
        /// 客服热线
        /// </summary>
        [SugarColumn(ColumnName = "hotline", Length = 20)]
        public string Hotline { get; set; }

        /// <summary>
        /// 车位数量
        /// </summary>
        [SugarColumn(ColumnName = "place_count")]
        public int PlaceCount { get; set; }

   

        /// <summary>
        /// 可选月份（多选逗号分割，例如1,3,6,12）
        /// </summary>
        [SugarColumn(ColumnName = "months", Length = 32)]
        public string Months { get; set; }

        /// <summary>
        /// 是否下发家庭成员
        /// </summary>
        [SugarColumn(ColumnName = "family_open")]
        public int FamilyOpen { get; set; }

    

        /// <summary>
        /// 是否开启线上注册 1是 0否
        /// </summary>
        [SugarColumn(ColumnName = "allow_register")]
        public int AllowRegister { get; set; }

        /// <summary>
        /// 基础电价
        /// </summary>
        [SugarColumn(ColumnName = "electric_price", ColumnDataType = "numeric(10,2)", IsNullable = true)]
        public decimal? ElectricPrice { get; set; }

        /// <summary>
        /// 工本费
        /// </summary>
        [SugarColumn(ColumnName = "deposit", IsNullable = true)]
        public int? Deposit { get; set; }

        /// <summary>
        /// 到期日期
        /// </summary>
        [SugarColumn(ColumnName = "expire_date", ColumnDataType = "date", IsNullable = true)]
        public DateTime  ExpireDate { get; set; }

      
    }
}