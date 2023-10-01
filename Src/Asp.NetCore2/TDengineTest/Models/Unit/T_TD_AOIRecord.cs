using SqlSugar;
using SqlSugar.DbConvert;
using System;
using System.Collections.Generic;
using System.Text;

namespace TDengineTest
{

    [SugarTable("aoi_recordsuper")]
    public class T_TD_AOIRecord
    {
        [SugarColumn(IsPrimaryKey = true, SqlParameterDbType = typeof(DateTime19))]
        public DateTime fcreatetime { get; set; }
        /// <summary>
        /// 产品唯一码
        /// </summary>
        [SugarColumn(SqlParameterDbType = typeof(CommonPropertyConvert))]
        public string fsncode { get; set; }
        /// <summary>
        /// 机台号
        /// </summary>
        [SugarColumn(SqlParameterDbType = typeof(CommonPropertyConvert))]
        public string fmachineno { get; set; }
        /// <summary>
        /// 批次号
        /// </summary>
        [SugarColumn(SqlParameterDbType = typeof(CommonPropertyConvert))]
        public string lotno { get; set; }
        /// <summary>
        /// 单据号
        /// </summary>
        [SugarColumn(SqlParameterDbType = typeof(CommonPropertyConvert))]
        public string billno { get; set; }
        /// <summary>
        /// 结果
        /// </summary>
        [SugarColumn(SqlParameterDbType = typeof(CommonPropertyConvert))]
        public string fresult { get; set; }
        /// <summary>
        /// bin编号
        /// </summary>
        [SugarColumn(SqlParameterDbType = typeof(CommonPropertyConvert))]
        public string dimbin { get; set; }
        /// <summary>
        /// 工站
        /// </summary>
        [SugarColumn(SqlParameterDbType = typeof(CommonPropertyConvert))]
        public string workstation { get; set; }
        /// <summary>
        /// 料号
        /// </summary>
        [SugarColumn(SqlParameterDbType = typeof(CommonPropertyConvert))]
        public string fproduct { get; set; }
        /// <summary>
        /// 机台时间
        /// </summary>
        [SugarColumn(SqlParameterDbType = typeof(CommonPropertyConvert))]
        public string machinetime { get; set; }
        /// <summary>
        /// 唯一ID
        /// </summary>
        [SugarColumn(SqlParameterDbType = typeof(CommonPropertyConvert))]
        public string id { get; set; }
        /// <summary>
        /// 测试状态
        /// </summary>
        [SugarColumn(SqlParameterDbType = typeof(CommonPropertyConvert))]
        public string uploadtype { get; set; }
    }


}
