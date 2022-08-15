using System;
using System.Collections.Generic;
using System.Text;
using SqlSugar;
namespace OrmTest
{
 
    [SugarTable("GJ_CN_2011_Rtd")]//_{year}{month}{day}
    [SugarIndex(null, nameof(MN), OrderByType.Asc)]
    public class GJ_CN_2011_RtdEntity
    {

        /// <summary>
        /// 记录ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, ColumnDescription = "记录ID")]
        public long Id { get; set; }

        #region 数据拆包结果
        /// <summary>
        /// 数据段长度
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "数据段长度")]
        public string? DataLen { get; set; }
        /// <summary>
        /// 请求编号
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "请求编号")]
        public string? QN { get; set; }
        /// <summary>
        /// 系统编号
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "系统编号")]
        public string? ST { get; set; }
        /// <summary>
        /// 命令编号
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "命令编号")]
        public string? CN { get; set; }
        /// <summary>
        /// 访问密码
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "访问密码")]
        public string? PW { get; set; }
        /// <summary>
        /// 设备唯一标识
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "设备唯一标识", ColumnDataType = "symbol")]
        public string? MN { get; set; }
        /// <summary>
        /// 是否拆分包及应答标志
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "是否拆分包及应答标志")]
        public string? Flag { get; set; }

        /// <summary>
        /// 指令参数：数据包时间
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "指令参数：数据包时间")]
        public string? CP_DataTime { get; set; }
        /// <summary>
        /// 指令参数：总悬浮颗粒物 TSP 空气质量 Mg/m3 N2.3标志位
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "指令参数：总悬浮颗粒物 TSP 空气质量 Mg/m3 N2.3标志位")]
        public string? CP_a34001_Flag { get; set; }
        /// <summary>
        /// 指令参数：可吸入颗粒物PM10空气质量 ug/m3 N4.1标志位
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "指令参数：可吸入颗粒物PM10空气质量 ug/m3 N4.1标志位")]
        public string? CP_a34002_Flag { get; set; }
        /// <summary>
        /// 指令参数：细微颗粒物 PM2.5 空气质量 ug/m3 N4.1标志位
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "指令参数：细微颗粒物 PM2.5 空气质量 ug/m3 N4.1标志位")]
        public string? CP_a34004_Flag { get; set; }
        /// <summary>
        /// 指令参数：温度 空气质量 ℃ N3.1标志位
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "指令参数：温度 空气质量 ℃ N3.1标志位")]
        public string? CP_a01001_Flag { get; set; }
        /// <summary>
        /// 指令参数：湿度 空气质量 %RH N3.2标志位
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "指令参数：湿度 空气质量 %RH N3.2标志位")]
        public string? CP_a01002_Flag { get; set; }
        /// <summary>
        /// 指令参数：风速 空气质量 m/s N4.1标志位
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "指令参数：风速 空气质量 m/s N4.1标志位")]
        public string? CP_a01007_Flag { get; set; }
        /// <summary>
        /// 指令参数：气压 空气质量 KPa N3.3标志位
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "指令参数：气压 空气质量 KPa N3.3标志位")]
        public string? CP_a01006_Flag { get; set; }
        /// <summary>
        /// 指令参数：噪声瞬时声级标志位
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "指令参数：噪声瞬时声级标志位")]
        public string? CP_LA_Flag { get; set; }
        /// <summary>
        /// 指令参数：风向 空气质量 [角]度 N4标志位
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "指令参数：风向 空气质量 [角]度 N4标志位")]
        public string? CP_a01008_Flag { get; set; }
        /// <summary>
        /// CRC校验码
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "CRC校验码")]
        public string? CRC { get; set; }
        #endregion

        /// <summary>
        /// 原始消息文本, ColumnDataType = "Nvarchar(MAX)"
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "原始消息文本")]
        public string? OriDatastring { get; set; }
        /// <summary>
        /// CRC计算结果
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "CRC计算结果")]
        public string? CRC_Calc { get; set; }
        /// <summary>
        /// CRC校验是否通过
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "CRC校验是否通过")]
        public bool CRCVaild { get; set; }
        /// <summary>
        /// 数据拆包是否通过
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "数据拆包是否通过")]
        public bool UnpackVaild { get; set; }
        /// <summary>
        /// 客户端IP地址
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "客户端IP地址")]
        public string? ClientIp { get; set; }
        /// <summary>
        /// 客户端端口号
        /// </summary>
        [SugarColumn(ColumnDescription = "客户端端口号")]
        public int ClientPort { get; set; }
        /// <summary>
        /// 数据接收时间
        /// </summary>
        [SugarColumn(ColumnDescription = "数据接收时间")]
   
        [TimeDbSplitField(DateType.Month)]
        public DateTime RecTime { get; set; }
        /// <summary>
        /// 数据入库时间
        /// </summary>
        [SugarColumn(ColumnDescription = "数据接收时间")]
        public DateTime CreateTime { get; set; } = DateTime.Now;




        /// <summary>
        /// 指令参数：总悬浮颗粒物 TSP 空气质量 Mg/m3 N2.3
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "指令参数：总悬浮颗粒物 TSP 空气质量 Mg/m3 N2.3")]
        public string? CP_a34001_Rtd { get; set; }
        /// <summary>
        /// 指令参数：可吸入颗粒物PM10空气质量 ug/m3 N4.1
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "指令参数：可吸入颗粒物PM10空气质量 ug/m3 N4.1")]
        public string? CP_a34002_Rtd { get; set; }
        /// <summary>
        /// 指令参数：细微颗粒物 PM2.5 空气质量 ug/m3 N4.1
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "指令参数：细微颗粒物 PM2.5 空气质量 ug/m3 N4.1")]
        public string? CP_a34004_Rtd { get; set; }
        /// <summary>
        /// 指令参数：温度 空气质量 ℃ N3.1
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "指令参数：温度 空气质量 ℃ N3.1")]
        public string? CP_a01001_Rtd { get; set; }
        /// <summary>
        /// 指令参数：湿度 空气质量 %RH N3.2
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "指令参数：湿度 空气质量 %RH N3.2")]
        public string? CP_a01002_Rtd { get; set; }
        /// <summary>
        /// 指令参数：风速 空气质量 m/s N4.1
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "指令参数：风速 空气质量 m/s N4.1")]
        public string? CP_a01007_Rtd { get; set; }
        /// <summary>
        /// 指令参数：气压 空气质量 KPa N3.3
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "指令参数：气压 空气质量 KPa N3.3")]
        public string? CP_a01006_Rtd { get; set; }
        /// <summary>
        /// 指令参数：噪声瞬时声级
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "指令参数：噪声瞬时声级")]
        public string? CP_LA_Rtd { get; set; }
        /// <summary>
        /// 指令参数：风向 空气质量 [角]度 N4
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "指令参数：风向 空气质量 [角]度 N4")]
        public string? CP_a01008_Rtd { get; set; }
    }
}
