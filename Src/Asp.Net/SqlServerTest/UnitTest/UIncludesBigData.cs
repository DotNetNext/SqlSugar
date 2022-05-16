
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class UIncludesBigData
    {
        public static void Init()
        {
            Console.WriteLine("");
            Console.WriteLine("#### SqlQueryable Start ####");
            SqlSugarClient db = NewUnitTest.Db;
         
            var list=db.Queryable<StationEntity>().ToList();
            foreach (var item in list)
            {
                item.KeyID = Guid.NewGuid() + "";
            }
            if (db.Queryable<StationEntity>().Count() < 2000)
            {
                db.Insertable(list).ExecuteCommand();
            }
            var b = DateTime.Now;
            db.Aop.OnLogExecuting = null;
            var data1 = db.Queryable<StationEntity>()
                            .Includes(t => t.StationConfigs, m => m.BaseStationConfig)
                           .Where(t => t.DeleteTime == null)
                           .OrderBy(t => t.StationCode, OrderByType.Asc)
                           .ToList();
            var b1 = DateTime.Now.Subtract(b).TotalMilliseconds;

            Console.WriteLine("#### SqlQueryable End ####");
        }
    }


    /// <summary>
    /// 工作站信息
    /// </summary>
    [SugarTable("TB_STATION", TableDescription = "工作站表")]
    public partial class StationEntity
    {
        #region 实体成员
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, ColumnName = "F_Id")]
        public string KeyID { get; set; }
        /// <summary>
        /// 创建人编号
        ///</summary>
        [SugarColumn(ColumnName = "F_CREATORUSERID")]
        public string CreatorUserId { get; set; }
        /// <summary>
        /// 创建时间
        ///</summary>
        [SugarColumn(ColumnName = "F_CREATORTIME")]
        public DateTime? CreatorTime { get; set; }
        /// <summary>
        /// 修改人
        ///</summary>
        [SugarColumn(ColumnName = "F_LASTMODIFYUSERID")]
        public String LastModifyUserId { get; set; }
        /// <summary>
        /// 修改时间
        ///</summary>
        [SugarColumn(ColumnName = "F_LASTMODIFYTIME")]
        public DateTime? LastModifyTime { get; set; }
        /// <summary>
        /// 删除时间
        ///</summary>
        [SugarColumn(ColumnName = "F_DELETETIME")]
        public DateTime? DeleteTime { get; set; }
        /// <summary>
        /// 删除人员ID
        ///</summary>
        [SugarColumn(ColumnName = "F_DELETEUSERID")]
        public string DeleteUserId { get; set; }
        /// <summary>
        /// 产线ID
        ///</summary>
        [SugarColumn(ColumnName = "C_LINEID")]
        public string LineID { get; set; }
        /// <summary>
        /// 工作站编组ID
        ///</summary>
        [SugarColumn(ColumnName = "C_STATIONGROUPID")]
        public string StationGroupID { get; set; }
        /// <summary>
        /// 工作站对应有效地址
        ///</summary>
        [SugarColumn(ColumnName = "StationConfigs", IsIgnore = true)]
        [Navigate(NavigateType.OneToMany, nameof(StationConfigEntity.StationID))]
        public List<StationConfigEntity> StationConfigs { get; set; }
        /// <summary>
        /// 基础表ID
        ///</summary>
        [SugarColumn(ColumnName = "C_BASESTATIONID")]
        public string BaseStationID { get; set; }

        /// <summary>
        /// 站名称
        ///</summary>
        [SugarColumn(ColumnName = "C_NAME", IsIgnore = true)]
        //[Navigate(NavigateType.OneToOne, nameof(BaseStationID))]
        public string Name { get; set; }
        /// <summary>
        /// 当前状态
        ///</summary>
        [SugarColumn(ColumnName = "C_CURSTATE")]
        public int? CurState { get; set; }
        /// <summary>
        /// 工作站编号
        ///</summary>
        [SugarColumn(ColumnName = "C_STATIONCODE")]
        public int? StationCode { get; set; }
        /// <summary>
        /// 配方ID(当前系统工作站正在使用的配方)
        ///</summary>
        [SugarColumn(ColumnName = "C_RECIPEID")]
        public string RecipeID { get; set; }
        /// <summary>
        /// IP地址
        ///</summary>
        [SugarColumn(ColumnName = "C_IP")]
        public string Ip { get; set; }
        /// <summary>
        /// 站边电脑IP地址
        ///</summary>
        [SugarColumn(ColumnName = "C_PCIP")]
        public String PcIp { get; set; }
        /// <summary>
        /// 端口号
        ///</summary>
        [SugarColumn(ColumnName = "C_PORT")]
        public int? Port { get; set; }
        /// <summary>
        /// PLC类型
        ///</summary>
        [SugarColumn(ColumnName = "C_PLCTYPE")]
        public int? PlcType { get; set; }
        /// <summary>
        /// 工作站类型
        ///</summary>
        [SugarColumn(ColumnName = "C_STATIONTYPE")]
        public int? StaType { get; set; }
        /// <summary>
        /// 是否可用
        ///</summary>
        [SugarColumn(ColumnName = "ENABLEDMARK")]
        public bool? EnabledMark { get; set; }
        /// <summary>
        /// 子工作站数量
        /// </summary>
        [SugarColumn(ColumnName = "C_CHILDRENCOUNT")]
        public int ChildrenCount { get; set; }
        /// <summary>
        /// 打标数量
        /// </summary>
        [SugarColumn(ColumnName = "C_PRINTCOUNT")]
        public int PrintCount { get; set; }
        /// <summary>
        /// 图表数量
        /// </summary>
        [SugarColumn(ColumnName = "C_CHARTCOUNT")]
        public int ChartCount { get; set; }
        /// <summary>
        /// 文件数量
        /// </summary>
        [SugarColumn(ColumnName = "C_FILECOUNT")]
        public int FileCount { get; set; }
        #endregion


    }

    /// <summary>
    /// 工作站地址配置表
    /// </summary>
    [SugarTable("TB_STATIONCONFIG", TableDescription = "工作站地址配置表")]
    public partial class StationConfigEntity
    {
        #region 实体成员
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, ColumnName = "F_Id")]
        public string KeyID { get; set; }
        /// <summary>
        /// 创建人编号
        ///</summary>
        [SugarColumn(ColumnName = "F_CREATORUSERID")]
        public string CreatorUserId { get; set; }
        /// <summary>
        /// 创建时间
        ///</summary>
        [SugarColumn(ColumnName = "F_CREATORTIME")]
        public DateTime? CreatorTime { get; set; }
        /// <summary>
        /// 修改人
        ///</summary>
        [SugarColumn(ColumnName = "F_LASTMODIFYUSERID")]
        public String LastModifyUserId { get; set; }
        /// <summary>
        /// 修改时间
        ///</summary>
        [SugarColumn(ColumnName = "F_LASTMODIFYTIME")]
        public DateTime? LastModifyTime { get; set; }
        /// <summary>
        /// 删除时间
        ///</summary>
        [SugarColumn(ColumnName = "F_DELETETIME")]
        public DateTime? DeleteTime { get; set; }
        /// <summary>
        /// 删除人员ID
        ///</summary>
        [SugarColumn(ColumnName = "F_DELETEUSERID")]
        public string DeleteUserId { get; set; }
        /// <summary>
        /// 工作站ID
        ///</summary>
        [SugarColumn(ColumnName = "C_STATIONID")]
        public string StationID { get; set; }
        /// <summary>
        /// 工作站站地址基本配置信息ID
        ///</summary>
        [SugarColumn(ColumnName = "C_BASESTATIONCONFIGID")]
        public string BaseStationConfigID { get; set; }
        /// <summary>
        /// 工作站站地址基本配置信息
        ///</summary>
        [SugarColumn(ColumnName = "BASESTATIONCONFIG", IsIgnore = true)]
        [Navigate(NavigateType.OneToOne, nameof(BaseStationConfigID))]
        public BaseStationConfigEntity BaseStationConfig { get; set; }
        ///// <summary>
        ///// 地址对应的请求信息
        /////</summary>
        //[SugarColumn(ColumnName = "StationConfigRequests", IsIgnore = true)]
        //[Navigate(NavigateType.OneToMany, nameof(StationConfigRequestEntity.StationConfigID))]
        //public List<StationConfigRequestEntity> StationConfigRequests { get; set; }
        /// <summary>
        /// 配置编号
        ///</summary>
        [SugarColumn(ColumnName = "C_CONFIGCODE")]
        public int? ConfigCode { get; set; }
        /// <summary>
        /// DB地址
        ///</summary>
        [SugarColumn(ColumnName = "C_DB")]
        public int? Db { get; set; }
        /// <summary>
        /// 偏移量
        ///</summary>
        [SugarColumn(ColumnName = "C_OFFSET")]
        public decimal? Offset { get; set; }
        /// <summary>
        /// 地址类型
        ///</summary>
        [SugarColumn(ColumnName = "C_ADDRTYPE")]
        public int? AddrType { get; set; }
        /// <summary>
        /// 使用类型
        ///</summary>
        [SugarColumn(ColumnName = "C_USETYPE")]
        public int? UseType { get; set; }
        /// <summary>
        /// 访问权限
        ///</summary>
        [SugarColumn(ColumnName = "C_DUTYCONTROL")]
        public int? DutyControl { get; set; }
        /// <summary>
        /// 描述
        ///</summary>
        [SugarColumn(ColumnName = "C_DESC")]
        public String Desc { get; set; }
        /// <summary>
        /// 响应地址(存储的为基础ID)
        ///</summary>
        [SugarColumn(ColumnName = "C_REPLYID")]
        public String ReplyID { get; set; }
        /// <summary>
        /// 配件对应地址
        ///</summary>
        [SugarColumn(ColumnName = "C_CMDID")]
        public String CmdID { get; set; }
        /// <summary>
        /// 保存参数绑定列名(用于后台，不能用于前端)
        ///</summary>
        [SugarColumn(ColumnName = "C_SAVECOL")]
        public String SaveCol { get; set; }
        /// <summary>
        /// 响应异常值
        ///</summary>
        [SugarColumn(ColumnName = "C_REPLYEXCEPTION")]
        public int? ReplyException { get; set; }
        /// <summary>
        /// 是否可用
        ///</summary>
        [SugarColumn(ColumnName = "ENABLEDMARK")]
        public bool? EnabledMark { get; set; }
        /// <summary>
        /// 序列
        ///</summary>
        [SugarColumn(ColumnName = "C_INDEX")]
        public int? Index { get; set; }
        /// <summary>
        /// 读取优先级
        ///</summary>
        [SugarColumn(ColumnName = "C_PRIORITY")]
        public int? Priority { get; set; }
        /// <summary>
        /// 当前值(用于仿真使用)
        ///</summary>
        [SugarColumn(ColumnName = "F_CURVALUE")]
        public string CurValue { get; set; }
        #endregion



    }

    /// <summary>
    /// 基本工作站配置信息
    ///</summary>
    [SugarTable("TB_BASESTATIONCONFIG", TableDescription = "基本工作站站配置")]
    public partial class BaseStationConfigEntity
    {
        #region 实体成员
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, ColumnName = "F_Id")]
        public string KeyID { get; set; }
        /// <summary>
        /// 创建人编号
        ///</summary>
        [SugarColumn(ColumnName = "F_CREATORUSERID")]
        public string CreatorUserId { get; set; }
        /// <summary>
        /// 创建时间
        ///</summary>
        [SugarColumn(ColumnName = "F_CREATORTIME")]
        public DateTime? CreatorTime { get; set; }
        /// <summary>
        /// 修改人
        ///</summary>
        [SugarColumn(ColumnName = "F_LASTMODIFYUSERID")]
        public String LastModifyUserId { get; set; }
        /// <summary>
        /// 修改时间
        ///</summary>
        [SugarColumn(ColumnName = "F_LASTMODIFYTIME")]
        public DateTime? LastModifyTime { get; set; }
        /// <summary>
        /// 删除时间
        ///</summary>
        [SugarColumn(ColumnName = "F_DELETETIME")]
        public DateTime? DeleteTime { get; set; }
        /// <summary>
        /// 删除人员ID
        ///</summary>
        [SugarColumn(ColumnName = "F_DELETEUSERID")]
        public string DeleteUserId { get; set; }
        /// <summary>
        /// 工作站ID
        ///</summary>
        [SugarColumn(ColumnName = "C_BASESTATIONID")]
        public String BaseStationID { get; set; }
        /// <summary>
        /// 地址名称
        ///</summary>
        [SugarColumn(ColumnName = "C_ADDRNAME")]
        public String AddrName { get; set; }
        /// <summary>
        /// 注释
        ///</summary>
        [SugarColumn(ColumnName = "C_DESC")]
        public String Desc { get; set; }
        /// <summary>
        /// 初始配置编号
        ///</summary>
        [SugarColumn(ColumnName = "C_CONFIGCODE")]
        public int? ConfigCode { get; set; }
        /// <summary>
        /// 初始DB地址
        ///</summary>
        [SugarColumn(ColumnName = "C_DB")]
        public int? Db { get; set; }
        /// <summary>
        /// 初始偏移量
        ///</summary>
        [SugarColumn(ColumnName = "C_OFFSET")]
        public decimal? Offset { get; set; }
        /// <summary>
        /// 地址类型
        ///</summary>
        [SugarColumn(ColumnName = "C_ADDRTYPE")]
        public int? AddrType { get; set; }
        /// <summary>
        /// 使用类型
        ///</summary>
        [SugarColumn(ColumnName = "C_USETYPE")]
        public int? UseType { get; set; }
        /// <summary>
        /// 访问权限
        ///</summary>
        [SugarColumn(ColumnName = "C_DUTYCONTROL")]
        public int? DutyControl { get; set; }
        /// <summary>
        /// 是否可用
        ///</summary>
        [SugarColumn(ColumnName = "ENABLEDMARK")]
        public bool? EnabledMark { get; set; }
        #endregion
    }
}
