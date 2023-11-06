using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace OrmTest
{
    public class UCustom07
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Building>();
            var list = new List<Building>();
            list.Add(new Building()
            {
                Address = "山东济南幸福家园小区",
                BuildingCode = "yfjy-1",
                BuildingId = Guid.NewGuid() + "",
                BuildingName = "幸福家园-1",
                CardNum = "kh123",
                CommunityId = "FG4PpbtA3yPXtXTONfxAV",
                CreateTime = DateTime.Now,
                HeatArea = 20000,
                OrganizationID = Guid.NewGuid(),
                Roomcount = 200,
                StartDate = "2022-8-17",
                State = 1,
                StationPartitionId =null
            });

            list.Add(new Building()
            {
                Address = "山东济南幸福家园小区",
                BuildingCode = "yfjy-2",
                BuildingId = Guid.NewGuid()+"",
                BuildingName = "幸福家园-2",
                CardNum = "kh123",
                CommunityId = "FG4PpbtA3yPXtXTONfxAV",
                CreateTime = DateTime.Now,
                HeatArea = 20000,
                OrganizationID = Guid.NewGuid(),
                Roomcount = 200,
                StartDate = "2022-8-17",
                State = 1,
                StationPartitionId = null
            });
             db.Utilities.PageEach(list, 200,   page =>
            {
                var x =   db.Storageable(page).ToStorage();
                  x.BulkCopy();

            });
        }
    }
    /// <summary>
    /// 建筑表
    /// </summary>
    [SugarTable("building111")]
    public class Building
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [SugarColumn(ColumnName = "buildingid", IsPrimaryKey = true, ColumnDataType = "varchar(51)")]
        public string BuildingId { get; set; }

        /// <summary>
        /// 供热站分区ID
        /// </summary>
        [SugarColumn(ColumnName = "stationpartitionid", IsNullable = true, ColumnDescription = "供热站分区ID", ColumnDataType = "varchar(21)")]
        public string StationPartitionId { get; set; }


        /// <summary>
        /// 小区ID
        /// </summary>
        [SugarColumn(ColumnName = "communityid", IsNullable = true, ColumnDescription = "小区ID", ColumnDataType = "varchar(21)")]
        public string CommunityId { get; set; }


        /// <summary>
        /// 所属公司ID
        /// </summary>
        [SugarColumn(ColumnName = "organizationid", IsNullable = true, ColumnDescription = "所属公司ID")]
        public Guid OrganizationID { get; set; }

        /// <summary>
        /// 楼栋编号
        /// </summary>
        [SugarColumn(ColumnName = "buildingcode", IsNullable = true, ColumnDataType = "varchar(40)", ColumnDescription = "楼栋编号")]
        public string BuildingCode { get; set; }

        /// <summary>
        /// 楼栋名称
        /// </summary>
        [SugarColumn(ColumnName = "buildingname", IsNullable = true, ColumnDataType = "varchar(80)", ColumnDescription = "楼栋名称")]
        public string BuildingName { get; set; }

        /// <summary>
        /// 卡号
        /// </summary>
        [SugarColumn(ColumnName = "cardnum", IsNullable = true, ColumnDataType = "varchar(20)", ColumnDescription = "卡号")]
        public string CardNum { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [SugarColumn(ColumnName = "state", IsNullable = true, ColumnDescription = "状态")]
        public int State { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        [SugarColumn(ColumnName = "address", IsNullable = true, ColumnDataType = "varchar(255)", ColumnDescription = "地址")]
        public string Address { get; set; }

        /// <summary>
        /// 户数
        /// </summary>
        [SugarColumn(ColumnName = "roomcount", IsNullable = true, ColumnDescription = "户数")]
        public int Roomcount { get; set; }

        /// <summary>
        /// 供热面积
        /// </summary>
  
        [SugarColumn(ColumnName = "heatarea", IsNullable = true, ColumnDescription = "供热面积")]
        public double HeatArea { get; set; }

        /// <summary>
        /// 接入日期
        /// </summary>
 
        [SugarColumn(ColumnName = "startdate", IsNullable = true, ColumnDataType = "varchar(20)", ColumnDescription = "接入日期")]
        public string StartDate { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
 
        [SugarColumn(ColumnName = "createtime", IsNullable = true, ColumnDescription = "创建时间")]
        public DateTime CreateTime { get; set; }
    }
}
