using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TDengineTest
{
    public class ConveryTable
    {

        [SugarColumn(IsPrimaryKey = true)]

        public DateTime ts { get; set; }

        /// <summary>

        ///  设备类型 0 没定义 1 输送线 2堆垛机 3AGV

        /// </summary>

        [SugarColumn(IsOnlyIgnoreInsert = true, IsOnlyIgnoreUpdate = true)]//Tags字段禁止插入

        public int DeviceType { get; set; }

        /// <summary>

        ///  BINARY(12) 	设备编号  Tags字段禁止插入

        /// </summary>

        [SugarColumn(Length = 12, IsOnlyIgnoreInsert = true, IsOnlyIgnoreUpdate = true)]

        [MaxLength(12, ErrorMessage = "最大支持12个字符")]

        public string DeviceCode { get; set; }



        public int Action { get; set; }



        /// <summary>

        /// INT,    0	任务编号

        /// </summary>

        public int TaskNo { get; set; }

        /// <summary>

        ///         INT,	0	开始时间

        /// </summary>

        public DateTime Stime { get; set; }

        /// <summary>

        ///         BOOL,	0	结束时间

        /// </summary>

        public DateTime Etime { get; set; }

        /// <summary>

        ///  INT,	0	货物类型

        /// </summary>

        public int GoodsType { get; set; }

        /// <summary>

        ///         NCHAR(64),	0	条码

        /// </summary>

        [SugarColumn(Length = 64)]

        [MaxLength(64, ErrorMessage = "最大支持64个字符")]

        public string BarCode { get; set; }

        /// <summary>

        ///        NCHAR(24),	0	源地址

        /// </summary>

        [SugarColumn(Length = 24)]

        [MaxLength(24, ErrorMessage = "最大支持24个字符")]

        public string FromNode { get; set; }

        /// <summary>

        ///         NCHAR(24),	0	目标地址  

        /// </summary>

        [SugarColumn(Length = 24)]

        [MaxLength(24, ErrorMessage = "最大支持24个字符")]

        public string ToNode { get; set; }

        /// <summary>

        ///  FLOAT,	0	速度

        /// </summary>

        public float Speed { get; set; }

        /// <summary>

        ///   FLOAT,	0	加速度

        /// </summary>

        public float AccSpeed { get; set; }

        /// <summary>

        ///         FLOAT,	0	减速度

        /// </summary>

        public float DecSpeed { get; set; }

        /// <summary>

        ///  NCHAR(256),	0	扩展字段1

        /// </summary>

        [SugarColumn(Length = 256)]

        [MaxLength(256, ErrorMessage = "最大支持256个字符")]

        public string Field1 { get; set; }

        /// <summary>

        ///   NCHAR(256),	0	扩展字段2

        /// </summary>

        [SugarColumn(Length = 256)]

        [MaxLength(256, ErrorMessage = "最大支持256个字符")]

        public string Field2 { get; set; }

        /// <summary>

        ///  NCHAR(256),	0	扩展字段3

        /// </summary>

        [SugarColumn(Length = 256)]

        [MaxLength(256, ErrorMessage = "最大支持256个字符")]

        public string Field3 { get; set; }

        /// <summary>

        /// NCHAR(256),	0	扩展字段4

        /// </summary>

        [SugarColumn(Length = 256)]

        [MaxLength(256, ErrorMessage = "最大支持256个字符")]

        public string Field4 { get; set; }

        /// <summary>

        ///  NCHAR(256),	0	扩展字段5

        /// </summary>

        [SugarColumn(Length = 256)]

        [MaxLength(256, ErrorMessage = "最大支持256个字符")]

        public string Field5 { get; set; }

        /// <summary>

        /// NCHAR(256),	0	扩展字段6

        /// </summary>

        [SugarColumn(Length = 256)]

        [MaxLength(256, ErrorMessage = "最大支持256个字符")]

        public string Field6 { get; set; }

        /// <summary>

        ///  NCHAR(500)  0	备注

        /// </summary>

        [SugarColumn(Length = 500)]

        [MaxLength(500, ErrorMessage = "最大支持500个字符")]

        public string Remark { get; set; }





    }
}
