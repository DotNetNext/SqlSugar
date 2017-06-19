using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace SqlSugar
{
    /// <summary>
    /// ** 描述：Queryable是多表查询基类，基于拥有大量查询扩展函数
    /// ** 创始时间：2015-7-13
    /// ** 修改时间：-
    /// ** 作者：sunkaixuan
    /// ** 使用说明：
    /// </summary>
    public class Sqlable
    {
        /// <summary>
        /// 数据接口
        /// </summary>
        public SqlSugarClient DB = null;
        /// <summary>
        /// sql临时数据
        /// </summary>
        public StringBuilder Sql { get; set; }
        /// <summary>
        /// Where临时数据
        /// </summary>
        public List<string> Where = new List<string>();
        /// <summary>
        /// OrderBy临时数据
        /// </summary>
        public string OrderBy { get; set; }
        /// <summary>
        /// GroupBy临时数据
        /// </summary>
        public string GroupBy { get; set; }
        /// <summary>
        /// 参数
        /// </summary>
        public List<SqlParameter> Params = new List<SqlParameter>();
    }
}
