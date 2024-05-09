using SqlSugar.TDengine;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace OrmTest
{
    /// <summary>
    /// 历史数据表
    /// </summary>
    [SugarTable("historyValue")]
    public class TDHistoryValue : STable
    {
        /// <summary>
        /// 上传时间
        /// </summary>
        [SugarColumn(InsertServerTime = true, IsPrimaryKey = true)]
        [Description("上传时间")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 采集时间
        /// </summary>
        [Description("采集时间")]
        public DateTime CollectTime { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        [Description("设备名称")]
        public string DeviceName { get; set; }

        /// <summary>
        /// 变量名称
        /// </summary>
        [Description("变量名称")]
        public string Name { get; set; }

        /// <summary>
        /// 是否在线
        /// </summary>
        [Description("是否在线")]
        public bool IsOnline { get; set; }

        /// <summary>
        /// 变量值
        /// </summary>
        [Description("变量值")]
        [SugarColumn(Length = 18, DecimalDigits = 2)]
        public double Value { get; set; }
    }
}
